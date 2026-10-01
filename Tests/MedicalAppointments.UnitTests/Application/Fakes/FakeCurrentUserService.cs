using MedicalAppointments.Application.Abstractions.Identity;

namespace MedicalAppointments.UnitTests.Application.Fakes;

public sealed class FakeCurrentUserService : ICurrentUserService
{
    public string? UserId { get; set; }

    public string? Role { get; set; }

    public bool IsAuthenticated { get; set; }
}
