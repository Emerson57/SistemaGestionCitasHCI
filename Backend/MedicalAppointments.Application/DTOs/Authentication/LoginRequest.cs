namespace MedicalAppointments.Application.DTOs.Authentication;

public sealed class LoginRequest
{
    public required string Email { get; init; }

    public required string Password { get; init; }
}
