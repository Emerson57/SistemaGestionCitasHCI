using System.Security.Claims;
using MedicalAppointments.Web.Auth;
using MedicalAppointments.Web.Models.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;

namespace MedicalAppointments.Web.Services.Authentication;

public sealed class JwtAuthenticationStateProvider(
    ITokenStorageService tokenStorage,
    IHttpContextAccessor httpContextAccessor) : AuthenticationStateProvider
{
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var session = await tokenStorage.GetSessionAsync();
        if (session is not null && !string.IsNullOrWhiteSpace(session.AccessToken))
        {
            if (session.User.ExpiresAt is not { } expiresAt || expiresAt > DateTimeOffset.UtcNow)
            {
                return new AuthenticationState(CreatePrincipal(session.User));
            }

            await tokenStorage.RemoveSessionAsync();
        }

        var cookieUser = httpContextAccessor.HttpContext?.User;
        if (cookieUser?.Identity?.IsAuthenticated == true)
        {
            return new AuthenticationState(cookieUser);
        }

        return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
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
