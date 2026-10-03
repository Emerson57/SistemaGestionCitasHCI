using MedicalAppointments.Web.Auth;
using MedicalAppointments.Web.Services.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;

namespace MedicalAppointments.Web.Extensions;

public static class AuthEndpointExtensions
{
    public static WebApplication MapAuthEndpoints(this WebApplication app)
    {
        app.MapGet("/auth/complete/{ticketId}", async (
            string ticketId,
            string? returnUrl,
            ISignInTicketStore tickets,
            IApiSessionCookieStore apiSessionCookie,
            HttpContext httpContext) =>
        {
            var response = tickets.ConsumeTicket(ticketId);
            if (response is null)
            {
                return Results.Redirect("/login?expired=1");
            }

            var principal = JwtAuthenticationStateProvider.CreatePrincipal(response);
            await httpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = false,
                    AllowRefresh = true
                });

            apiSessionCookie.SetFromAuthenticationResponse(response);

            var safeReturnUrl = ReturnUrlValidator.NormalizeLocalPath(returnUrl);
            if (ReturnUrlValidator.IsSafeLocalReturnUrl(safeReturnUrl))
            {
                return Results.Redirect(safeReturnUrl!);
            }

            var destination = response.Role switch
            {
                _ when string.Equals(response.Role, AppRoles.Administrator, StringComparison.Ordinal) => "/admin",
                _ when string.Equals(response.Role, AppRoles.Doctor, StringComparison.Ordinal) => "/medico",
                _ => "/paciente"
            };

            return Results.Redirect(destination);
        }).AllowAnonymous();

        app.MapGet("/auth/sign-out", async (HttpContext httpContext, IApiSessionCookieStore apiSessionCookie, string? returnUrl) =>
        {
            apiSessionCookie.Delete();
            await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            var safeReturnUrl = ReturnUrlValidator.NormalizeLocalPath(returnUrl);
            if (ReturnUrlValidator.IsSafeLocalReturnUrl(safeReturnUrl))
            {
                return Results.Redirect(safeReturnUrl!);
            }

            return Results.Redirect("/");
        }).AllowAnonymous();

        return app;
    }
}
