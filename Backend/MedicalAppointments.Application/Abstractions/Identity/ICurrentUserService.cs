namespace MedicalAppointments.Application.Abstractions.Identity;

public interface ICurrentUserService
{
    string? UserId { get; }

    string? Role { get; }

    bool IsAuthenticated { get; }
}
