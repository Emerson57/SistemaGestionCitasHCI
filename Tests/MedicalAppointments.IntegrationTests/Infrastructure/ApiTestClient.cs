using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using MedicalAppointments.Application.DTOs.Authentication;
using MedicalAppointments.Application.DTOs.Doctors;
using MedicalAppointments.Application.DTOs.Specialties;
using MedicalAppointments.Domain.Enums;

namespace MedicalAppointments.IntegrationTests.Infrastructure;

public static class ApiTestClient
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public static async Task<(HttpResponseMessage Response, AuthenticationResponse? Body)> RegisterPatientAsync(
        HttpClient client,
        string email,
        string password)
    {
        var request = new RegisterPatientRequest
        {
            FullName = "Integration Patient",
            Email = email,
            Password = password,
            BirthDate = new DateOnly(1995, 5, 5),
            Address = "Test Address",
            PhoneNumber = "555-0101",
            Sex = Sex.Female,
            Disability = null,
            MaritalStatus = MaritalStatus.Single
        };

        var response = await client.PostAsJsonAsync("/api/auth/register", request, JsonOptions);
        AuthenticationResponse? body = null;
        if (response.IsSuccessStatusCode)
        {
            body = await ReadJsonAsync<AuthenticationResponse>(response);
        }

        return (response, body);
    }

    public static async Task<(HttpResponseMessage Response, AuthenticationResponse? Body)> LoginAsync(
        HttpClient client,
        string email,
        string password)
    {
        var response = await client.PostAsJsonAsync(
            "/api/auth/login",
            new LoginRequest { Email = email, Password = password },
            JsonOptions);

        AuthenticationResponse? body = null;
        if (response.IsSuccessStatusCode)
        {
            body = await ReadJsonAsync<AuthenticationResponse>(response);
        }

        return (response, body);
    }

    public static void Authorize(HttpClient client, string token)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public static void ClearAuthorization(HttpClient client) =>
        client.DefaultRequestHeaders.Authorization = null;

    public static async Task<int> CreateSpecialtyAsync(HttpClient adminClient, string name)
    {
        var response = await adminClient.PostAsJsonAsync(
            "/api/specialties",
            new CreateSpecialtyRequest { Name = name, Description = "Integration specialty" },
            JsonOptions);

        response.EnsureSuccessStatusCode();
        var specialty = await ReadJsonAsync<SpecialtyDto>(response);
        return specialty!.Id;
    }

    public static async Task<DoctorDto> CreateDoctorAsync(
        HttpClient adminClient,
        int specialtyId,
        string email,
        string license)
    {
        var response = await adminClient.PostAsJsonAsync(
            "/api/doctors",
            new CreateDoctorRequest
            {
                FullName = "Integration Doctor",
                Email = email,
                InitialPassword = "DoctorPassword123!",
                ProfessionalLicense = license,
                SpecialtyId = specialtyId
            },
            JsonOptions);

        response.EnsureSuccessStatusCode();
        return (await ReadJsonAsync<DoctorDto>(response))!;
    }

    private static async Task<T?> ReadJsonAsync<T>(HttpResponseMessage response)
    {
        if (response.Content.Headers.ContentLength == 0)
        {
            return default;
        }

        await using var stream = await response.Content.ReadAsStreamAsync();
        return await JsonSerializer.DeserializeAsync<T>(stream, JsonOptions);
    }
}
