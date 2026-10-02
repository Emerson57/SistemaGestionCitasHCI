using MedicalAppointments.Application.DTOs.Appointments;
using System.Net.Http.Json;
using MedicalAppointments.Domain.Enums;
using MedicalAppointments.Infrastructure.Persistence;
using MedicalAppointments.IntegrationTests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MedicalAppointments.IntegrationTests;

[Collection(IntegrationTestCollection.Name)]
public sealed class AppointmentPersistenceIntegrationTests(IntegrationTestFixture fixture)
{
    [Fact]
    public async Task Confirm_InsertsStatusHistoryRow_WithoutUpdatingExistingRows()
    {
        var (patientClient, doctor, appointmentTime, doctorEmail) = await IntegrationTestScenarios.CreatePatientDoctorScenarioAsync(fixture);
        var schedule = await patientClient.PostAsJsonAsync(
            "/api/appointments",
            new ScheduleAppointmentRequest { DoctorId = doctor.Id, AppointmentDateTime = appointmentTime },
            IntegrationTestScenarios.JsonOptions);
        schedule.EnsureSuccessStatusCode();
        var appointment = await schedule.Content.ReadFromJsonAsync<AppointmentDto>(IntegrationTestScenarios.JsonOptions);

        using (var scope = fixture.Factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            Assert.Equal(0, await db.AppointmentStatusHistories.CountAsync(h => h.AppointmentId == appointment!.Id));
        }

        using var doctorClient = fixture.Factory.CreateClient();
        var (_, doctorAuth) = await ApiTestClient.LoginAsync(doctorClient, doctorEmail, "DoctorPassword123!");
        ApiTestClient.Authorize(doctorClient, doctorAuth!.AccessToken!);
        var confirm = await doctorClient.PutAsync($"/api/appointments/{appointment!.Id}/confirm", null);
        confirm.EnsureSuccessStatusCode();

        using var verifyScope = fixture.Factory.Services.CreateScope();
        var verifyDb = verifyScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var histories = await verifyDb.AppointmentStatusHistories
            .AsNoTracking()
            .Where(h => h.AppointmentId == appointment.Id)
            .OrderBy(h => h.ChangedAt)
            .ToListAsync();

        Assert.Single(histories);
        Assert.Equal(AppointmentHistoryAction.StatusChange, histories[0].Action);
        Assert.Equal(AppointmentStatus.Scheduled, histories[0].PreviousStatus);
        Assert.Equal(AppointmentStatus.Confirmed, histories[0].NewStatus);

        var storedAppointment = await verifyDb.Appointments
            .AsNoTracking()
            .SingleAsync(a => a.Id == appointment.Id);
        Assert.Equal(AppointmentStatus.Confirmed, storedAppointment.Status);

        var firstHistorySnapshot = histories[0];

        var complete = await doctorClient.PutAsync($"/api/appointments/{appointment.Id}/complete", null);
        complete.EnsureSuccessStatusCode();

        var historiesAfterComplete = await verifyDb.AppointmentStatusHistories
            .AsNoTracking()
            .Where(h => h.AppointmentId == appointment.Id)
            .OrderBy(h => h.ChangedAt)
            .ToListAsync();

        Assert.Equal(2, historiesAfterComplete.Count);
        Assert.Equal(firstHistorySnapshot.Id, historiesAfterComplete[0].Id);
        Assert.Equal(firstHistorySnapshot.PreviousStatus, historiesAfterComplete[0].PreviousStatus);
        Assert.Equal(firstHistorySnapshot.NewStatus, historiesAfterComplete[0].NewStatus);
        Assert.Equal(firstHistorySnapshot.ChangedAt, historiesAfterComplete[0].ChangedAt);
    }

    [Fact]
    public async Task Cancel_PersistsCancelledStatusHistoryAndReleasesAvailability()
    {
        var (patientClient, doctor, appointmentTime, _) = await IntegrationTestScenarios.CreatePatientDoctorScenarioAsync(fixture);
        var schedule = await patientClient.PostAsJsonAsync(
            "/api/appointments",
            new ScheduleAppointmentRequest { DoctorId = doctor.Id, AppointmentDateTime = appointmentTime },
            IntegrationTestScenarios.JsonOptions);
        schedule.EnsureSuccessStatusCode();
        var appointment = await schedule.Content.ReadFromJsonAsync<AppointmentDto>(IntegrationTestScenarios.JsonOptions);

        var cancel = await patientClient.DeleteAsync($"/api/appointments/{appointment!.Id}");
        cancel.EnsureSuccessStatusCode();

        using var scope = fixture.Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var stored = await db.Appointments.AsNoTracking().SingleAsync(a => a.Id == appointment.Id);
        Assert.Equal(AppointmentStatus.Cancelled, stored.Status);

        var history = await db.AppointmentStatusHistories
            .AsNoTracking()
            .SingleAsync(h => h.AppointmentId == appointment.Id);
        Assert.Equal(AppointmentHistoryAction.StatusChange, history.Action);
        Assert.Equal(AppointmentStatus.Scheduled, history.PreviousStatus);
        Assert.Equal(AppointmentStatus.Cancelled, history.NewStatus);

        var slot = await db.DoctorAvailabilities
            .AsNoTracking()
            .SingleAsync(a => a.DoctorId == doctor.Id);
        Assert.Equal(AvailabilityStatus.Available, slot.Status);
    }

    [Fact]
    public async Task Reschedule_PersistsNewDateAvailabilityAndHistory()
    {
        var (patientClient, doctor, appointmentTime, _) = await IntegrationTestScenarios.CreatePatientDoctorScenarioAsync(fixture);
        var schedule = await patientClient.PostAsJsonAsync(
            "/api/appointments",
            new ScheduleAppointmentRequest { DoctorId = doctor.Id, AppointmentDateTime = appointmentTime },
            IntegrationTestScenarios.JsonOptions);
        schedule.EnsureSuccessStatusCode();
        var appointment = await schedule.Content.ReadFromJsonAsync<AppointmentDto>(IntegrationTestScenarios.JsonOptions);

        var newTime = appointmentTime.AddHours(1);
        var reschedule = await patientClient.PutAsJsonAsync(
            $"/api/appointments/{appointment!.Id}/reschedule",
            new RescheduleAppointmentRequest { AppointmentDateTime = newTime },
            IntegrationTestScenarios.JsonOptions);
        reschedule.EnsureSuccessStatusCode();

        using var scope = fixture.Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var stored = await db.Appointments.AsNoTracking().SingleAsync(a => a.Id == appointment.Id);
        Assert.Equal(newTime, stored.AppointmentDateTime);
        Assert.Equal(AppointmentStatus.Scheduled, stored.Status);

        var history = await db.AppointmentStatusHistories
            .AsNoTracking()
            .SingleAsync(h => h.AppointmentId == appointment.Id);
        Assert.Equal(AppointmentHistoryAction.Reschedule, history.Action);
        Assert.Equal(AppointmentStatus.Scheduled, history.PreviousStatus);
        Assert.Equal(AppointmentStatus.Rescheduled, history.NewStatus);
        Assert.Equal(appointmentTime, history.PreviousAppointmentDateTime);
        Assert.Equal(newTime, history.NewAppointmentDateTime);

        var slot = await db.DoctorAvailabilities
            .AsNoTracking()
            .SingleAsync(a => a.DoctorId == doctor.Id);
        Assert.Equal(AvailabilityStatus.Reserved, slot.Status);
    }

}
