using System.Text.Json;
using MedicalAppointments.Web.Models.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;

namespace MedicalAppointments.Web.Services.Authentication;

/// <summary>
/// HttpOnly cookie mirror of the API session so Bearer tokens survive full page loads
/// and are available before ProtectedSessionStorage interop is ready.
/// </summary>
public interface IApiSessionCookieStore
{
    void SetFromAuthenticationResponse(AuthenticationResponse response);

    StoredAuthSession? GetCurrent();

    void Delete();
}

public sealed class ApiSessionCookieStore(
    IHttpContextAccessor httpContextAccessor,
    IDataProtectionProvider dataProtectionProvider) : IApiSessionCookieStore
{
    public const string CookieName = "MedicalAppointments.ApiSession";

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly IDataProtector _protector =
        dataProtectionProvider.CreateProtector("MedicalAppointments.ApiSession.v1");

    public void SetFromAuthenticationResponse(AuthenticationResponse response)
    {
        if (string.IsNullOrWhiteSpace(response.AccessToken))
        {
            return;
        }

        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext is null || httpContext.Response.HasStarted)
        {
            return;
        }

        var session = new StoredAuthSession
        {
            User = response,
            AccessToken = response.AccessToken
        };

        var protectedPayload = _protector.Protect(JsonSerializer.Serialize(session, JsonOptions));
        var options = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Lax,
            IsEssential = true,
            Path = "/"
        };

        if (response.ExpiresAt is { } expiresAt)
        {
            options.Expires = expiresAt.UtcDateTime;
        }

        try
        {
            httpContext.Response.Cookies.Append(CookieName, protectedPayload, options);
        }
        catch
        {
            // Blazor circuits may not allow cookie writes; /auth/complete sets the API session cookie.
        }
    }

    public StoredAuthSession? GetCurrent()
    {
        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext is null
            || !httpContext.Request.Cookies.TryGetValue(CookieName, out var protectedPayload)
            || string.IsNullOrWhiteSpace(protectedPayload))
        {
            return null;
        }

        try
        {
            var json = _protector.Unprotect(protectedPayload);
            return JsonSerializer.Deserialize<StoredAuthSession>(json, JsonOptions);
        }
        catch
        {
            return null;
        }
    }

    public void Delete()
    {
        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext is null || httpContext.Response.HasStarted)
        {
            return;
        }

        try
        {
            httpContext.Response.Cookies.Delete(CookieName, new CookieOptions { Path = "/" });
        }
        catch
        {
            // Ignore when response headers are no longer writable.
        }
    }
}
