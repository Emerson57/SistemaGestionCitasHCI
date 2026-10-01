namespace MedicalAppointments.Application.DTOs.Availability;

public sealed class CreateDoctorAvailabilityRequest
{
    public Guid DoctorId { get; init; }

    public DateOnly Date { get; init; }

    public TimeOnly StartTime { get; init; }

    public TimeOnly EndTime { get; init; }
}
