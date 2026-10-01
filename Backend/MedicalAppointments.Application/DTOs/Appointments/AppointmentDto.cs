using MedicalAppointments.Domain.Enums;

namespace MedicalAppointments.Application.DTOs.Appointments;

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
