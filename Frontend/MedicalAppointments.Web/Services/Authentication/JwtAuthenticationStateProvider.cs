using System.Security.Claims;
using MedicalAppointments.Web.Auth;
using MedicalAppointments.Web.Models.Authentication;
using Microsoft.AspNetCore.Components.Authorization;

namespace MedicalAppointments.Web.Services.Authentication;

public sealed class JwtAuthenticationStateProvider(ITokenStorageService tokenStorage) : AuthenticationStateProvider
{
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var session = await tokenStorage.GetSessionAsync();
        if (session is null || string.IsNullOrWhiteSpace(session.AccessToken))
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        if (session.User.ExpiresAt is { } expiresAt && expiresAt <= DateTimeOffset.UtcNow)
        {
            await tokenStorage.RemoveSessionAsync();
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        return new AuthenticationState(CreatePrincipal(session.User));
    }

    public async Task SignInAsync(AuthenticationResponse response, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(response.AccessToken))
        {
            throw new InvalidOperationException("Authentication response did not include an access token.");
        }

        await tokenStorage.SetSessionAsync(new StoredAuthSession
        {
            User = response,
            AccessToken = response.AccessToken
        }, cancellationToken);

        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public async Task SignOutAsync(CancellationToken cancellationToken = default)
    {
        await tokenStorage.RemoveSessionAsync(cancellationToken);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public static ClaimsPrincipal CreatePrincipal(AuthenticationResponse user)
    {
        var claims = new List<Claim>
        {
            new(AppClaimTypes.UserId, user.UserId),
            new(AppClaimTypes.Email, user.Email),
            new(AppClaimTypes.Name, user.FullName),
            new(ClaimTypes.Role, user.Role),
            new(AppClaimTypes.Role, user.Role)
        };

        var identity = new ClaimsIdentity(claims, authenticationType: "Bearer");
        return new ClaimsPrincipal(identity);
    }
}
