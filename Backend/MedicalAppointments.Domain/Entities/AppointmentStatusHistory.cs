using MedicalAppointments.Domain.Enums;
using MedicalAppointments.Domain.Exceptions;

namespace MedicalAppointments.Domain.Entities;

public class AppointmentStatusHistory
{
    private AppointmentStatusHistory()
    {
        ChangedBy = string.Empty;
    }

    internal AppointmentStatusHistory(
        Guid id,
        Guid appointmentId,
        AppointmentStatus previousStatus,
        AppointmentStatus newStatus,
        DateTimeOffset changedAt,
        string changedBy)
    {
        Id = id;
        AppointmentId = appointmentId;
        PreviousStatus = previousStatus;
        NewStatus = newStatus;
        ChangedAt = changedAt;
        ChangedBy = changedBy;
    }

    public Guid Id { get; private set; }

    public Guid AppointmentId { get; private set; }

    public AppointmentStatus PreviousStatus { get; private set; }

    public AppointmentStatus NewStatus { get; private set; }

    public DateTimeOffset ChangedAt { get; private set; }

    public string ChangedBy { get; private set; }

    public Appointment? Appointment { get; private set; }

    internal static AppointmentStatusHistory Create(
        Guid id,
        Guid appointmentId,
        AppointmentStatus previousStatus,
        AppointmentStatus newStatus,
        DateTimeOffset changedAt,
        string changedBy)
    {
        if (id == Guid.Empty)
        {
            throw new DomainException("Status history id cannot be empty.");
        }

        if (appointmentId == Guid.Empty)
        {
            throw new DomainException("Appointment id cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(changedBy))
        {
            throw new DomainException("Changed by cannot be empty.");
        }

        return new AppointmentStatusHistory(
            id,
            appointmentId,
            previousStatus,
            newStatus,
            changedAt,
            changedBy.Trim());
    }

    internal void AssignAppointment(Appointment appointment)
    {
        Appointment = appointment;
        AppointmentId = appointment.Id;
    }
}
