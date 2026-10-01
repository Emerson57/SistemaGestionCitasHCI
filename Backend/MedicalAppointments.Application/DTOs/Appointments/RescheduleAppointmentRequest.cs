namespace MedicalAppointments.Application.DTOs.Appointments;

public sealed class RescheduleAppointmentRequest
{
    public DateTimeOffset AppointmentDateTime { get; init; }
}
