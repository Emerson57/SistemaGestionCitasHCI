using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using MedicalAppointments.Application.DTOs.Appointments;
using MedicalAppointments.Application.DTOs.Authentication;
using MedicalAppointments.Application.DTOs.Availability;
using MedicalAppointments.Application.DTOs.Specialties;
using MedicalAppointments.Infrastructure.Persistence;
using MedicalAppointments.IntegrationTests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MedicalAppointments.IntegrationTests;

[Collection(IntegrationTestCollection.Name)]
public sealed class ApiIntegrationTests(IntegrationTestFixture fixture)
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    [Fact]
    public async Task RegisterPatient_Succeeds()
    {
        using var client = fixture.Factory.CreateClient();
        var email = $"patient.{Guid.NewGuid():N}@test.local";

        var (response, body) = await ApiTestClient.RegisterPatientAsync(client, email, "PatientPassword123!");

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(body?.AccessToken);
    }

    [Fact]
    public async Task RegisterPatient_CreatesPatientLinkedToIdentityUser()
    {
        using var client = fixture.Factory.CreateClient();
        var email = $"linked.{Guid.NewGuid():N}@test.local";

        var (response, body) = await ApiTestClient.RegisterPatientAsync(client, email, "PatientPassword123!");

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(body?.UserId);

        await using var scope = fixture.Factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var patient = await db.Patients.SingleOrDefaultAsync(p => p.UserId == body!.UserId);

        Assert.NotNull(patient);
        Assert.True(patient!.IsActive);
    }

    [Fact]
    public async Task RegisterPatient_DuplicateEmailFails()
    {
        using var client = fixture.Factory.CreateClient();
        var email = $"dup.{Guid.NewGuid():N}@test.local";

        await ApiTestClient.RegisterPatientAsync(client, email, "PatientPassword123!");
        var (response, _) = await ApiTestClient.RegisterPatientAsync(client, email, "PatientPassword123!");

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task Login_SucceedsWithCorrectPassword()
    {
        using var client = fixture.Factory.CreateClient();
        var email = $"login.{Guid.NewGuid():N}@test.local";
        await ApiTestClient.RegisterPatientAsync(client, email, "PatientPassword123!");

        var (response, body) = await ApiTestClient.LoginAsync(client, email, "PatientPassword123!");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(body?.AccessToken);
    }

    [Fact]
    public async Task Login_FailsWithIncorrectPassword()
    {
        using var client = fixture.Factory.CreateClient();
        var email = $"badlogin.{Guid.NewGuid():N}@test.local";
        await ApiTestClient.RegisterPatientAsync(client, email, "PatientPassword123!");

        var (response, _) = await ApiTestClient.LoginAsync(client, email, "WrongPassword123!");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task AnonymousProtectedEndpoint_ReturnsUnauthorized()
    {
        using var client = fixture.Factory.CreateClient();
        var response = await client.GetAsync("/api/patients/me");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task PatientCannotCreateSpecialty()
    {
        using var client = fixture.Factory.CreateClient();
        var email = $"patient-admin.{Guid.NewGuid():N}@test.local";
        var (_, auth) = await ApiTestClient.RegisterPatientAsync(client, email, "PatientPassword123!");
        ApiTestClient.Authorize(client, auth!.AccessToken!);

        var response = await client.PostAsJsonAsync(
            "/api/specialties",
            new CreateSpecialtyRequest { Name = "Unauthorized Specialty" },
            JsonOptions);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task PatientCannotAccessAnotherPatientById()
    {
        using var client = fixture.Factory.CreateClient();
        var (_, auth) = await ApiTestClient.RegisterPatientAsync(client, $"p1.{Guid.NewGuid():N}@test.local", "PatientPassword123!");
        ApiTestClient.Authorize(client, auth!.AccessToken!);

        var me = await client.GetAsync("/api/patients/me");
        me.EnsureSuccessStatusCode();
        var profile = await me.Content.ReadFromJsonAsync<Application.DTOs.Patients.PatientDto>(JsonOptions);

        var response = await client.GetAsync($"/api/patients/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.NotEqual(profile!.Id, Guid.Empty);
    }

    [Fact]
    public async Task DoctorCannotConfirmAnotherDoctorsAppointment()
    {
        var adminClient = await CreateAdminClientAsync();
        var specialtyId = await ApiTestClient.CreateSpecialtyAsync(adminClient, $"Spec-{Guid.NewGuid():N}");

        var doctorAEmail = $"doctorA.{Guid.NewGuid():N}@test.local";
        var doctorBEmail = $"doctorB.{Guid.NewGuid():N}@test.local";
        var doctorA = await ApiTestClient.CreateDoctorAsync(adminClient, specialtyId, doctorAEmail, $"LIC-{Guid.NewGuid():N}");
        await ApiTestClient.CreateDoctorAsync(adminClient, specialtyId, doctorBEmail, $"LIC-{Guid.NewGuid():N}");

        var appointmentTime = IntegrationTestScenarios.BuildFutureAppointmentTime();
        await IntegrationTestScenarios.SeedDoctorAvailabilityAsync(fixture,doctorAEmail, appointmentTime);

        using var patientClient = fixture.Factory.CreateClient();
        var (_, patientAuth) = await ApiTestClient.RegisterPatientAsync(
            patientClient,
            $"patient.{Guid.NewGuid():N}@test.local",
            "PatientPassword123!");
        ApiTestClient.Authorize(patientClient, patientAuth!.AccessToken!);

        var scheduleResponse = await patientClient.PostAsJsonAsync(
            "/api/appointments",
            new ScheduleAppointmentRequest
            {
                DoctorId = doctorA.Id,
                AppointmentDateTime = appointmentTime
            },
            JsonOptions);
        scheduleResponse.EnsureSuccessStatusCode();
        var appointment = await scheduleResponse.Content.ReadFromJsonAsync<AppointmentDto>(JsonOptions);

        using var doctorBClient = fixture.Factory.CreateClient();
        var (_, doctorBAuth) = await ApiTestClient.LoginAsync(doctorBClient, doctorBEmail, "DoctorPassword123!");
        ApiTestClient.Authorize(doctorBClient, doctorBAuth!.AccessToken!);

        var confirmResponse = await doctorBClient.PutAsync($"/api/appointments/{appointment!.Id}/confirm", null);
        Assert.Equal(HttpStatusCode.Forbidden, confirmResponse.StatusCode);
    }

    [Fact]
    public async Task AdministratorCanCreateSpecialty()
    {
        using var adminClient = await CreateAdminClientAsync();
        var response = await adminClient.PostAsJsonAsync(
            "/api/specialties",
            new CreateSpecialtyRequest { Name = $"Cardiology-{Guid.NewGuid():N}" },
            JsonOptions);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task AuthenticatedUserCanListSpecialties()
    {
        using var client = fixture.Factory.CreateClient();
        var (_, auth) = await ApiTestClient.RegisterPatientAsync(client, $"list.{Guid.NewGuid():N}@test.local", "PatientPassword123!");
        ApiTestClient.Authorize(client, auth!.AccessToken!);

        var response = await client.GetAsync("/api/specialties");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task PatientCanScheduleValidAppointment()
    {
        var adminClient = await CreateAdminClientAsync();
        var specialtyId = await ApiTestClient.CreateSpecialtyAsync(adminClient, $"Spec-{Guid.NewGuid():N}");
        var doctorEmail = $"doctor.{Guid.NewGuid():N}@test.local";
        var doctor = await ApiTestClient.CreateDoctorAsync(adminClient, specialtyId, doctorEmail, $"LIC-{Guid.NewGuid():N}");
        var appointmentTime = IntegrationTestScenarios.BuildFutureAppointmentTime();
        await IntegrationTestScenarios.SeedDoctorAvailabilityAsync(fixture,doctorEmail, appointmentTime);

        using var patientClient = fixture.Factory.CreateClient();
        var (_, auth) = await ApiTestClient.RegisterPatientAsync(patientClient, $"sched.{Guid.NewGuid():N}@test.local", "PatientPassword123!");
        ApiTestClient.Authorize(patientClient, auth!.AccessToken!);

        var response = await patientClient.PostAsJsonAsync(
            "/api/appointments",
            new ScheduleAppointmentRequest { DoctorId = doctor.Id, AppointmentDateTime = appointmentTime },
            JsonOptions);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task OccupiedSlotProducesConflict()
    {
        var adminClient = await CreateAdminClientAsync();
        var specialtyId = await ApiTestClient.CreateSpecialtyAsync(adminClient, $"Spec-{Guid.NewGuid():N}");
        var doctorEmail = $"doctor.{Guid.NewGuid():N}@test.local";
        var doctor = await ApiTestClient.CreateDoctorAsync(adminClient, specialtyId, doctorEmail, $"LIC-{Guid.NewGuid():N}");
        var appointmentTime = IntegrationTestScenarios.BuildFutureAppointmentTime();
        await IntegrationTestScenarios.SeedDoctorAvailabilityAsync(fixture,doctorEmail, appointmentTime);

        using var patientOne = fixture.Factory.CreateClient();
        var (_, authOne) = await ApiTestClient.RegisterPatientAsync(patientOne, $"p1.{Guid.NewGuid():N}@test.local", "PatientPassword123!");
        ApiTestClient.Authorize(patientOne, authOne!.AccessToken!);
        var first = await patientOne.PostAsJsonAsync(
            "/api/appointments",
            new ScheduleAppointmentRequest { DoctorId = doctor.Id, AppointmentDateTime = appointmentTime },
            JsonOptions);
        first.EnsureSuccessStatusCode();

        using var patientTwo = fixture.Factory.CreateClient();
        var (_, authTwo) = await ApiTestClient.RegisterPatientAsync(patientTwo, $"p2.{Guid.NewGuid():N}@test.local", "PatientPassword123!");
        ApiTestClient.Authorize(patientTwo, authTwo!.AccessToken!);
        var second = await patientTwo.PostAsJsonAsync(
            "/api/appointments",
            new ScheduleAppointmentRequest { DoctorId = doctor.Id, AppointmentDateTime = appointmentTime },
            JsonOptions);

        Assert.Equal(HttpStatusCode.Conflict, second.StatusCode);
    }

    [Fact]
    public async Task PatientCanListOwnAppointments()
    {
        var (patientClient, doctor, appointmentTime, _) = await IntegrationTestScenarios.CreatePatientDoctorScenarioAsync(fixture);
        await patientClient.PostAsJsonAsync(
            "/api/appointments",
            new ScheduleAppointmentRequest { DoctorId = doctor.Id, AppointmentDateTime = appointmentTime },
            JsonOptions);

        var response = await patientClient.GetAsync("/api/appointments/my");
        response.EnsureSuccessStatusCode();
        var appointments = await response.Content.ReadFromJsonAsync<List<AppointmentDto>>(JsonOptions);
        Assert.NotNull(appointments);
        Assert.Single(appointments!);
    }

    [Fact]
    public async Task PatientCannotCancelAnotherPatientsAppointment()
    {
        var adminClient = await CreateAdminClientAsync();
        var specialtyId = await ApiTestClient.CreateSpecialtyAsync(adminClient, $"Spec-{Guid.NewGuid():N}");
        var doctorEmail = $"doctor.{Guid.NewGuid():N}@test.local";
        var doctor = await ApiTestClient.CreateDoctorAsync(adminClient, specialtyId, doctorEmail, $"LIC-{Guid.NewGuid():N}");
        var appointmentTime = IntegrationTestScenarios.BuildFutureAppointmentTime();
        await IntegrationTestScenarios.SeedDoctorAvailabilityAsync(fixture,doctorEmail, appointmentTime);

        using var ownerClient = fixture.Factory.CreateClient();
        var (_, ownerAuth) = await ApiTestClient.RegisterPatientAsync(ownerClient, $"owner.{Guid.NewGuid():N}@test.local", "PatientPassword123!");
        ApiTestClient.Authorize(ownerClient, ownerAuth!.AccessToken!);
        var schedule = await ownerClient.PostAsJsonAsync(
            "/api/appointments",
            new ScheduleAppointmentRequest { DoctorId = doctor.Id, AppointmentDateTime = appointmentTime },
            JsonOptions);
        var appointment = await schedule.Content.ReadFromJsonAsync<AppointmentDto>(JsonOptions);

        using var otherClient = fixture.Factory.CreateClient();
        var (_, otherAuth) = await ApiTestClient.RegisterPatientAsync(otherClient, $"other.{Guid.NewGuid():N}@test.local", "PatientPassword123!");
        ApiTestClient.Authorize(otherClient, otherAuth!.AccessToken!);
        var cancel = await otherClient.DeleteAsync($"/api/appointments/{appointment!.Id}");

        Assert.Equal(HttpStatusCode.Forbidden, cancel.StatusCode);
    }

    [Fact]
    public async Task CancellingOwnAppointmentReleasesAvailability()
    {
        var (patientClient, doctor, appointmentTime, _) = await IntegrationTestScenarios.CreatePatientDoctorScenarioAsync(fixture);
        var schedule = await patientClient.PostAsJsonAsync(
            "/api/appointments",
            new ScheduleAppointmentRequest { DoctorId = doctor.Id, AppointmentDateTime = appointmentTime },
            JsonOptions);
        var appointment = await schedule.Content.ReadFromJsonAsync<AppointmentDto>(JsonOptions);

        var cancel = await patientClient.DeleteAsync($"/api/appointments/{appointment!.Id}");
        Assert.Equal(HttpStatusCode.NoContent, cancel.StatusCode);

        var availability = await patientClient.GetAsync($"/api/doctors/{doctor.Id}/availability");
        availability.EnsureSuccessStatusCode();
        var slots = await availability.Content.ReadFromJsonAsync<List<DoctorAvailabilityDto>>(JsonOptions);
        Assert.NotNull(slots);
        Assert.NotEmpty(slots!);
    }

    [Fact]
    public async Task ReschedulingReleasesOldSlotAndReservesNewSlot()
    {
        var (patientClient, doctor, appointmentTime, _) = await IntegrationTestScenarios.CreatePatientDoctorScenarioAsync(fixture);
        var schedule = await patientClient.PostAsJsonAsync(
            "/api/appointments",
            new ScheduleAppointmentRequest { DoctorId = doctor.Id, AppointmentDateTime = appointmentTime },
            JsonOptions);
        var appointment = await schedule.Content.ReadFromJsonAsync<AppointmentDto>(JsonOptions);

        var newTime = appointmentTime.AddHours(1);
        var reschedule = await patientClient.PutAsJsonAsync(
            $"/api/appointments/{appointment!.Id}/reschedule",
            new RescheduleAppointmentRequest { AppointmentDateTime = newTime },
            JsonOptions);

        Assert.Equal(HttpStatusCode.OK, reschedule.StatusCode);
    }

    [Fact]
    public async Task DoctorCanViewOwnAgenda()
    {
        var adminClient = await CreateAdminClientAsync();
        var specialtyId = await ApiTestClient.CreateSpecialtyAsync(adminClient, $"Spec-{Guid.NewGuid():N}");
        var doctorEmail = $"doctor.{Guid.NewGuid():N}@test.local";
        var doctor = await ApiTestClient.CreateDoctorAsync(adminClient, specialtyId, doctorEmail, $"LIC-{Guid.NewGuid():N}");
        var appointmentTime = IntegrationTestScenarios.BuildFutureAppointmentTime();
        await IntegrationTestScenarios.SeedDoctorAvailabilityAsync(fixture,doctorEmail, appointmentTime);

        using var patientClient = fixture.Factory.CreateClient();
        var (_, patientAuth) = await ApiTestClient.RegisterPatientAsync(patientClient, $"patient.{Guid.NewGuid():N}@test.local", "PatientPassword123!");
        ApiTestClient.Authorize(patientClient, patientAuth!.AccessToken!);
        await patientClient.PostAsJsonAsync(
            "/api/appointments",
            new ScheduleAppointmentRequest { DoctorId = doctor.Id, AppointmentDateTime = appointmentTime },
            JsonOptions);

        using var doctorClient = fixture.Factory.CreateClient();
        var (_, doctorAuth) = await ApiTestClient.LoginAsync(doctorClient, doctorEmail, "DoctorPassword123!");
        ApiTestClient.Authorize(doctorClient, doctorAuth!.AccessToken!);

        var agenda = await doctorClient.GetAsync("/api/doctors/me/agenda");
        Assert.Equal(HttpStatusCode.OK, agenda.StatusCode);
    }

    [Fact]
    public async Task DoctorCannotAccessAnotherDoctorsAgenda()
    {
        var adminClient = await CreateAdminClientAsync();
        var specialtyId = await ApiTestClient.CreateSpecialtyAsync(adminClient, $"Spec-{Guid.NewGuid():N}");
        var doctorAEmail = $"doctorA.{Guid.NewGuid():N}@test.local";
        var doctorBEmail = $"doctorB.{Guid.NewGuid():N}@test.local";
        var appointmentTime = IntegrationTestScenarios.BuildFutureAppointmentTime();
        var doctorA = await ApiTestClient.CreateDoctorAsync(adminClient, specialtyId, doctorAEmail, $"LIC-{Guid.NewGuid():N}");
        await ApiTestClient.CreateDoctorAsync(adminClient, specialtyId, doctorBEmail, $"LIC-{Guid.NewGuid():N}");
        await IntegrationTestScenarios.SeedDoctorAvailabilityAsync(fixture,doctorAEmail, appointmentTime);

        using var patientClient = fixture.Factory.CreateClient();
        var (_, patientAuth) = await ApiTestClient.RegisterPatientAsync(patientClient, $"patient.{Guid.NewGuid():N}@test.local", "PatientPassword123!");
        ApiTestClient.Authorize(patientClient, patientAuth!.AccessToken!);
        await patientClient.PostAsJsonAsync(
            "/api/appointments",
            new ScheduleAppointmentRequest { DoctorId = doctorA.Id, AppointmentDateTime = appointmentTime },
            JsonOptions);

        using var doctorBClient = fixture.Factory.CreateClient();
        var (_, doctorBAuth) = await ApiTestClient.LoginAsync(doctorBClient, doctorBEmail, "DoctorPassword123!");
        ApiTestClient.Authorize(doctorBClient, doctorBAuth!.AccessToken!);

        var agenda = await doctorBClient.GetAsync("/api/doctors/me/agenda");
        var body = await agenda.Content.ReadFromJsonAsync<List<AppointmentDto>>(JsonOptions);
        Assert.Equal(HttpStatusCode.OK, agenda.StatusCode);
        Assert.NotNull(body);
        Assert.Empty(body!);
    }

    [Fact]
    public async Task DoctorCanConfirmOwnAppointment()
    {
        var adminClient = await CreateAdminClientAsync();
        var specialtyId = await ApiTestClient.CreateSpecialtyAsync(adminClient, $"Spec-{Guid.NewGuid():N}");
        var doctorEmail = $"doctor.{Guid.NewGuid():N}@test.local";
        var doctor = await ApiTestClient.CreateDoctorAsync(adminClient, specialtyId, doctorEmail, $"LIC-{Guid.NewGuid():N}");
        var appointmentTime = IntegrationTestScenarios.BuildFutureAppointmentTime();
        await IntegrationTestScenarios.SeedDoctorAvailabilityAsync(fixture,doctorEmail, appointmentTime);

        using var patientClient = fixture.Factory.CreateClient();
        var (_, patientAuth) = await ApiTestClient.RegisterPatientAsync(patientClient, $"patient.{Guid.NewGuid():N}@test.local", "PatientPassword123!");
        ApiTestClient.Authorize(patientClient, patientAuth!.AccessToken!);
        var schedule = await patientClient.PostAsJsonAsync(
            "/api/appointments",
            new ScheduleAppointmentRequest { DoctorId = doctor.Id, AppointmentDateTime = appointmentTime },
            JsonOptions);
        var appointment = await schedule.Content.ReadFromJsonAsync<AppointmentDto>(JsonOptions);

        using var doctorClient = fixture.Factory.CreateClient();
        var (_, doctorAuth) = await ApiTestClient.LoginAsync(doctorClient, doctorEmail, "DoctorPassword123!");
        ApiTestClient.Authorize(doctorClient, doctorAuth!.AccessToken!);

        var confirm = await doctorClient.PutAsync($"/api/appointments/{appointment!.Id}/confirm", null);
        Assert.Equal(HttpStatusCode.OK, confirm.StatusCode);
    }

    private async Task<HttpClient> CreateAdminClientAsync()
    {
        var client = fixture.Factory.CreateClient();
        var (loginResponse, auth) = await ApiTestClient.LoginAsync(client, fixture.AdminEmail, fixture.AdminPassword);
        loginResponse.EnsureSuccessStatusCode();
        Assert.NotNull(auth?.AccessToken);
        ApiTestClient.Authorize(client, auth.AccessToken);
        return client;
    }

}
