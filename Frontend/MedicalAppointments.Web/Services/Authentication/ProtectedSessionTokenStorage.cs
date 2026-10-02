using MedicalAppointments.Web.Models.Authentication;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace MedicalAppointments.Web.Services.Authentication;

/// <summary>
/// Persists auth session in encrypted browser session storage (per browser tab/session).
/// Tradeoff: survives navigation and circuit reconnect within the same browser session,
/// but is not shared across devices. Passwords are never stored.
/// </summary>
public sealed class ProtectedSessionTokenStorage(ProtectedSessionStorage sessionStorage) : ITokenStorageService
{
    private const string StorageKey = "auth.session";

    public async Task<StoredAuthSession?> GetSessionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await sessionStorage.GetAsync<StoredAuthSession>(StorageKey);
            return result.Success ? result.Value : null;
        }
        catch
        {
            return null;
        }
    }

    public async Task SetSessionAsync(StoredAuthSession session, CancellationToken cancellationToken = default) =>
        await sessionStorage.SetAsync(StorageKey, session);

    public async Task RemoveSessionAsync(CancellationToken cancellationToken = default) =>
        await sessionStorage.DeleteAsync(StorageKey);
}
