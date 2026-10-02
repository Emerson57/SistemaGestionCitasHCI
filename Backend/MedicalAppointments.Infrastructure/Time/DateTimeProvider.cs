using MedicalAppointments.Application.Abstractions.Time;

namespace MedicalAppointments.Infrastructure.Time;

public sealed class DateTimeProvider : IDateTimeProvider
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
