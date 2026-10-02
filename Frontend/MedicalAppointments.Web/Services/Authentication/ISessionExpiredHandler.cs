using Microsoft.AspNetCore.Components;

namespace MedicalAppointments.Web.Services.Authentication;

public interface ISessionExpiredHandler
{
    Task HandleSessionExpiredAsync();
}

public sealed class SessionExpiredHandler(
    JwtAuthenticationStateProvider authStateProvider,
    NavigationManager navigationManager) : ISessionExpiredHandler
{
    public async Task HandleSessionExpiredAsync()
    {
        await authStateProvider.SignOutAsync();
        navigationManager.NavigateTo("/login?expired=1", forceLoad: true);
    }
}
