using MedicalAppointments.Application.Abstractions.Time;

namespace MedicalAppointments.UnitTests.Application.Fakes;

public sealed class FakeDateTimeProvider(DateTimeOffset utcNow) : IDateTimeProvider
{
    public DateTimeOffset UtcNow { get; } = utcNow;
}
