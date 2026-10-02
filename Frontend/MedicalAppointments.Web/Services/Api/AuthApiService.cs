using MedicalAppointments.Web.Models.Authentication;

namespace MedicalAppointments.Web.Services.Api;

public interface IAuthApiService
{
    Task<AuthenticationResponse> RegisterAsync(RegisterPatientRequest request, CancellationToken cancellationToken = default);

    Task<AuthenticationResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
}

public sealed class AuthApiService(HttpClient httpClient) : IAuthApiService
{
    public async Task<AuthenticationResponse> RegisterAsync(
        RegisterPatientRequest request,
        CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsJsonAsync("api/auth/register", request, ApiJson.Options, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            throw await ApiResponseHelper.CreateExceptionAsync(response, cancellationToken);
        }

        return (await response.Content.ReadFromJsonAsync<AuthenticationResponse>(ApiJson.Options, cancellationToken))!;
    }

    public async Task<AuthenticationResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsJsonAsync("api/auth/login", request, ApiJson.Options, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            throw await ApiResponseHelper.CreateExceptionAsync(response, cancellationToken);
        }

        return (await response.Content.ReadFromJsonAsync<AuthenticationResponse>(ApiJson.Options, cancellationToken))!;
    }
}

internal static class ApiResponseHelper
{
    public static async Task<ApiException> CreateExceptionAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        ApiProblemDetails? problem = null;
        try
        {
            problem = await response.Content.ReadFromJsonAsync<ApiProblemDetails>(ApiJson.Options, cancellationToken);
        }
        catch
        {
            // ignored
        }

        var detail = problem?.Detail ?? problem?.Title ?? response.ReasonPhrase ?? "Error";
        return new ApiException(detail, (int)response.StatusCode, problem);
    }
}
