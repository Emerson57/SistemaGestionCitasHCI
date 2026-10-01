namespace MedicalAppointments.Application.DTOs.Appointments;

public sealed class ScheduleAppointmentRequest
{
    public Guid DoctorId { get; init; }

    public DateTimeOffset AppointmentDateTime { get; init; }

    public string? Reason { get; init; }
}
