using MedicalAppointments.Domain.Enums;

namespace MedicalAppointments.Application.DTOs.Availability;

public sealed class DoctorAvailabilityDto
{
    public Guid Id { get; init; }

    public Guid DoctorId { get; init; }

    public DateOnly Date { get; init; }

    public TimeOnly StartTime { get; init; }

    public TimeOnly EndTime { get; init; }

    public AvailabilityStatus Status { get; init; }
}
