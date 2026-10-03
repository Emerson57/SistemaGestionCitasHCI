using MedicalAppointments.Web.Models.Authentication;

namespace MedicalAppointments.Web.Services.Authentication;

/// <summary>
/// Copies the HttpOnly API session cookie into ProtectedSessionStorage once the circuit is interactive.
/// </summary>
public interface IAuthSessionSyncService
{
    Task SyncBrowserStorageFromServerCookieAsync(CancellationToken cancellationToken = default);
}

public sealed class AuthSessionSyncService(
    IApiSessionCookieStore apiSessionCookie,
    ProtectedSessionTokenStorage sessionStorage) : IAuthSessionSyncService
{
    public async Task SyncBrowserStorageFromServerCookieAsync(CancellationToken cancellationToken = default)
    {
        var fromCookie = apiSessionCookie.GetCurrent();
        if (fromCookie is null || string.IsNullOrWhiteSpace(fromCookie.AccessToken))
        {
            return;
        }

        var existing = await sessionStorage.GetSessionAsync(cancellationToken);
        if (existing is not null && !string.IsNullOrWhiteSpace(existing.AccessToken))
        {
            return;
        }

        await sessionStorage.SetSessionAsync(fromCookie, cancellationToken);
    }
}
