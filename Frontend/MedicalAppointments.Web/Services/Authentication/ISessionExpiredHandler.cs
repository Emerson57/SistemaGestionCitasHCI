using Microsoft.AspNetCore.Components;

namespace MedicalAppointments.Web.Services.Authentication;

public interface ISessionExpiredHandler
{
    /// <summary>
    /// Clears session when the API rejected a request that included a Bearer token.
    /// </summary>
    Task HandleSessionExpiredAsync();
}

public sealed class SessionExpiredHandler(
    JwtAuthenticationStateProvider authStateProvider,
    NavigationManager navigationManager) : ISessionExpiredHandler
{
    public async Task HandleSessionExpiredAsync()
    {
        try
        {
            await authStateProvider.SignOutAsync();
        }
        catch
        {
            // Browser storage only; cookies cleared via sign-out endpoint.
        }

        navigationManager.NavigateTo(
            "/auth/sign-out?returnUrl=" + Uri.EscapeDataString("/login?expired=1"),
            forceLoad: true);
    }
}
