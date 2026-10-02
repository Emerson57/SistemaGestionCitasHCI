using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MedicalAppointments.Application.Abstractions.Identity;
using MedicalAppointments.Application.Abstractions.Time;
using MedicalAppointments.Infrastructure.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace MedicalAppointments.Infrastructure.Identity;

public sealed class JwtTokenService(
    IOptions<JwtOptions> jwtOptions,
    IDateTimeProvider dateTimeProvider) : ITokenService
{
    public IssuedAccessToken CreateAccessToken(
        string userId,
        string email,
        string fullName,
        string role)
    {
        var options = jwtOptions.Value;
        var expiresAt = dateTimeProvider.UtcNow.AddMinutes(options.ExpirationMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId),
            new(JwtRegisteredClaimNames.Email, email),
            new(JwtRegisteredClaimNames.Name, fullName),
            new("role", role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.SigningKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: options.Issuer,
            audience: options.Audience,
            claims: claims,
            notBefore: dateTimeProvider.UtcNow.UtcDateTime,
            expires: expiresAt.UtcDateTime,
            signingCredentials: credentials);

        return new IssuedAccessToken
        {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
            ExpiresAt = expiresAt
        };
    }
}
