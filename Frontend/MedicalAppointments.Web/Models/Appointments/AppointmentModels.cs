using MedicalAppointments.Web.Models.Enums;

namespace MedicalAppointments.Web.Models.Appointments;

public sealed class AppointmentDto
{
    public Guid Id { get; init; }

    public Guid PatientId { get; init; }

    public required string PatientName { get; init; }

    public Guid DoctorId { get; init; }

    public required string DoctorName { get; init; }

    public int SpecialtyId { get; init; }

    public string? SpecialtyName { get; init; }

    public DateTimeOffset AppointmentDateTime { get; init; }

    public AppointmentStatus Status { get; init; }

    public string? Reason { get; init; }

    public DateTimeOffset CreatedAt { get; init; }
}

public sealed class ScheduleAppointmentRequest
{
    public Guid DoctorId { get; init; }

    public DateTimeOffset AppointmentDateTime { get; init; }

    public string? Reason { get; init; }
}

public sealed class RescheduleAppointmentRequest
{
    public DateTimeOffset AppointmentDateTime { get; init; }
}
