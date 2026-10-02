namespace MedicalAppointments.Application.Abstractions.Identity;

public interface ITokenService
{
    IssuedAccessToken CreateAccessToken(
        string userId,
        string email,
        string fullName,
        string role);
}

public sealed class IssuedAccessToken
{
    public required string AccessToken { get; init; }

    public DateTimeOffset ExpiresAt { get; init; }
}
