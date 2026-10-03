using MedicalAppointments.Web.Models.Authentication;

namespace MedicalAppointments.Web.Services.Authentication;

/// <summary>
/// Browser session storage first; HttpOnly API session cookie when interop is not ready.
/// </summary>
public sealed class CompositeTokenStorageService(
    ITokenStorageService browserTokenStorage,
    IApiSessionCookieStore apiSessionCookie) : ITokenStorageService
{
    public async Task<StoredAuthSession?> GetSessionAsync(CancellationToken cancellationToken = default)
    {
        var session = await browserTokenStorage.GetSessionAsync(cancellationToken);
        if (session is not null && !string.IsNullOrWhiteSpace(session.AccessToken))
        {
            return session;
        }

        return apiSessionCookie.GetCurrent();
    }

    public async Task SetSessionAsync(StoredAuthSession session, CancellationToken cancellationToken = default)
    {
        await browserTokenStorage.SetSessionAsync(session, cancellationToken);
        apiSessionCookie.SetFromAuthenticationResponse(session.User);
    }

    public async Task RemoveSessionAsync(CancellationToken cancellationToken = default)
    {
        await browserTokenStorage.RemoveSessionAsync(cancellationToken);
        // HttpOnly cookies are cleared on GET /auth/sign-out (Blazor circuits cannot write headers after render).
    }
}
