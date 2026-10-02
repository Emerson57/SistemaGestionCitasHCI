namespace MedicalAppointments.Application.DTOs.Availability;

public sealed class CreateMyDoctorAvailabilityRequest
{
    public DateOnly Date { get; init; }

    public TimeOnly StartTime { get; init; }

    public TimeOnly EndTime { get; init; }
}
