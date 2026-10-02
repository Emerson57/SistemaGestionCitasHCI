using MedicalAppointments.Web.Models.Enums;

namespace MedicalAppointments.Web.Models.Availability;

public sealed class DoctorAvailabilityDto
{
    public Guid Id { get; init; }

    public Guid DoctorId { get; init; }

    public DateOnly Date { get; init; }

    public TimeOnly StartTime { get; init; }

    public TimeOnly EndTime { get; init; }

    public AvailabilityStatus Status { get; init; }
}

public sealed class CreateMyDoctorAvailabilityRequest
{
    public DateOnly Date { get; init; }

    public TimeOnly StartTime { get; init; }

    public TimeOnly EndTime { get; init; }
}
