using Microsoft.AspNetCore.Components.Authorization;

namespace MedicalAppointments.Web.Services.Authentication;

public interface IAuthSessionReadiness
{
    /// <summary>
    /// True when the user is anonymous or an access token can be resolved for API calls.
    /// </summary>
    Task<bool> IsApiTokenAvailableAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Waits until token storage/cookie sync can supply a token, or the user is not authenticated.
    /// </summary>
    Task WaitForApiTokenAsync(CancellationToken cancellationToken = default);
}

public sealed class AuthSessionReadiness(
    ITokenStorageService tokenStorage,
    AuthenticationStateProvider authenticationStateProvider) : IAuthSessionReadiness
{
    private const int MaxAttempts = 12;

    public async Task<bool> IsApiTokenAvailableAsync(CancellationToken cancellationToken = default)
    {
        var session = await tokenStorage.GetSessionAsync(cancellationToken);
        return !string.IsNullOrWhiteSpace(session?.AccessToken);
    }

    public async Task WaitForApiTokenAsync(CancellationToken cancellationToken = default)
    {
        for (var attempt = 0; attempt < MaxAttempts; attempt++)
        {
            if (await IsApiTokenAvailableAsync(cancellationToken))
            {
                return;
            }

            var state = await authenticationStateProvider.GetAuthenticationStateAsync();
            if (state.User.Identity?.IsAuthenticated != true)
            {
                return;
            }

            await Task.Yield();
        }
    }
}
