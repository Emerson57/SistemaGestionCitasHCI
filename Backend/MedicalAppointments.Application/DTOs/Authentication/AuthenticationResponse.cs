namespace MedicalAppointments.Application.DTOs.Authentication;

public sealed class AuthenticationResponse
{
    public required string UserId { get; init; }

    public required string FullName { get; init; }

    public required string Email { get; init; }

    public required string Role { get; init; }

    public string? AccessToken { get; init; }

    public DateTimeOffset? ExpiresAt { get; init; }
}
