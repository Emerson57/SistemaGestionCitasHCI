using System.Net.Http.Json;
using System.Text.Json;
using MedicalAppointments.Web.Services.Authentication;

namespace MedicalAppointments.Web.Services.Api;

public abstract class ApiClientBase(
    HttpClient httpClient,
    ISessionExpiredHandler sessionExpiredHandler)
{
    protected static JsonSerializerOptions JsonOptions => ApiJson.Options;

    protected async Task<T?> GetAsync<T>(string url, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.GetAsync(url, cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<T>(JsonOptions, cancellationToken);
    }

    protected async Task<T?> PostAsync<T>(string url, object? body, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsJsonAsync(url, body, JsonOptions, cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
        if (response.Content.Headers.ContentLength == 0)
        {
            return default;
        }

        return await response.Content.ReadFromJsonAsync<T>(JsonOptions, cancellationToken);
    }

    protected async Task PostAsync(string url, object? body, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsJsonAsync(url, body, JsonOptions, cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
    }

    protected async Task<T?> PutAsync<T>(string url, object? body, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PutAsJsonAsync(url, body, JsonOptions, cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
        if (response.Content.Headers.ContentLength == 0)
        {
            return default;
        }

        return await response.Content.ReadFromJsonAsync<T>(JsonOptions, cancellationToken);
    }

    protected async Task PutAsync(string url, object? body, CancellationToken cancellationToken = default)
    {
        using var response = body is null
            ? await httpClient.PutAsync(url, null, cancellationToken)
            : await httpClient.PutAsJsonAsync(url, body, JsonOptions, cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
    }

    protected async Task DeleteAsync(string url, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.DeleteAsync(url, cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
    }

    private async Task EnsureSuccessAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        ApiProblemDetails? problem = null;
        try
        {
            problem = await response.Content.ReadFromJsonAsync<ApiProblemDetails>(JsonOptions, cancellationToken);
        }
        catch
        {
            // Ignore parse failures.
        }

        var detail = problem?.Detail ?? problem?.Title ?? response.ReasonPhrase ?? "Error";
        var exception = new ApiException(detail, (int)response.StatusCode, problem);

        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            await sessionExpiredHandler.HandleSessionExpiredAsync();
        }

        throw exception;
    }
}
