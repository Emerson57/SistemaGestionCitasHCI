using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using MedicalAppointments.Application.Abstractions.Identity;

namespace MedicalAppointments.Api.Services;

public sealed class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public string? UserId =>
        httpContextAccessor.HttpContext?.User.FindFirstValue(JwtRegisteredClaimNames.Sub);

    public string? Role =>
        httpContextAccessor.HttpContext?.User.FindFirstValue("role");

    public bool IsAuthenticated =>
        httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated == true;
}
