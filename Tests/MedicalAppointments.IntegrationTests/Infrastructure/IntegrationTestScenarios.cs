using System.Net.Http.Json;
using System.Text.Json;
using MedicalAppointments.Application.DTOs.Availability;
using MedicalAppointments.Application.DTOs.Doctors;

namespace MedicalAppointments.IntegrationTests.Infrastructure;

public static class IntegrationTestScenarios
{
    public static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public static async Task<HttpClient> CreateAdminClientAsync(IntegrationTestFixture fixture)
    {
        var client = fixture.Factory.CreateClient();
        var (loginResponse, auth) = await ApiTestClient.LoginAsync(client, fixture.AdminEmail, fixture.AdminPassword);
        loginResponse.EnsureSuccessStatusCode();
        Assert.NotNull(auth?.AccessToken);
        ApiTestClient.Authorize(client, auth.AccessToken);
        return client;
    }

    public static async Task<(HttpClient PatientClient, DoctorDto Doctor, DateTimeOffset AppointmentTime, string DoctorEmail)> CreatePatientDoctorScenarioAsync(
        IntegrationTestFixture fixture)
    {
        var adminClient = await CreateAdminClientAsync(fixture);
        var specialtyId = await ApiTestClient.CreateSpecialtyAsync(adminClient, $"Spec-{Guid.NewGuid():N}");
        var doctorEmail = $"doctor.{Guid.NewGuid():N}@test.local";
        var doctor = await ApiTestClient.CreateDoctorAsync(adminClient, specialtyId, doctorEmail, $"LIC-{Guid.NewGuid():N}");
        var appointmentTime = BuildFutureAppointmentTime();
        await SeedDoctorAvailabilityAsync(fixture, doctorEmail, appointmentTime);

        var patientClient = fixture.Factory.CreateClient();
        var (_, auth) = await ApiTestClient.RegisterPatientAsync(
            patientClient,
            $"patient.{Guid.NewGuid():N}@test.local",
            "PatientPassword123!");
        ApiTestClient.Authorize(patientClient, auth!.AccessToken!);
        return (patientClient, doctor, appointmentTime, doctorEmail);
    }

    public static async Task SeedDoctorAvailabilityAsync(
        IntegrationTestFixture fixture,
        string doctorEmail,
        DateTimeOffset appointmentTime)
    {
        using var doctorClient = fixture.Factory.CreateClient();
        var (_, auth) = await ApiTestClient.LoginAsync(doctorClient, doctorEmail, "DoctorPassword123!");
        ApiTestClient.Authorize(doctorClient, auth!.AccessToken!);

        var date = DateOnly.FromDateTime(appointmentTime.UtcDateTime);
        var response = await doctorClient.PostAsJsonAsync(
            "/api/doctors/me/availability",
            new CreateMyDoctorAvailabilityRequest
            {
                Date = date,
                StartTime = new TimeOnly(8, 0),
                EndTime = new TimeOnly(18, 0)
            },
            JsonOptions);

        response.EnsureSuccessStatusCode();
    }

    public static DateTimeOffset BuildFutureAppointmentTime()
    {
        var utc = DateTimeOffset.UtcNow.AddDays(14);
        return new DateTimeOffset(utc.Year, utc.Month, utc.Day, 10, 0, 0, TimeSpan.Zero);
    }
}
