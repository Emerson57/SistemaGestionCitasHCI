using MedicalAppointments.Domain.Enums;
using MedicalAppointments.Domain.Exceptions;

namespace MedicalAppointments.Domain.Entities;

public class AppointmentStatusHistory
{
    private AppointmentStatusHistory()
    {
        ChangedBy = string.Empty;
    }

    private AppointmentStatusHistory(
        Guid id,
        Guid appointmentId,
        AppointmentHistoryAction action,
        AppointmentStatus previousStatus,
        AppointmentStatus newStatus,
        DateTimeOffset changedAt,
        string changedBy,
        DateTimeOffset? previousAppointmentDateTime,
        DateTimeOffset? newAppointmentDateTime)
    {
        Id = id;
        AppointmentId = appointmentId;
        Action = action;
        PreviousStatus = previousStatus;
        NewStatus = newStatus;
        ChangedAt = changedAt;
        ChangedBy = changedBy;
        PreviousAppointmentDateTime = previousAppointmentDateTime;
        NewAppointmentDateTime = newAppointmentDateTime;
    }

    public Guid Id { get; private set; }

    public Guid AppointmentId { get; private set; }

    public AppointmentHistoryAction Action { get; private set; }

    public AppointmentStatus PreviousStatus { get; private set; }

    public AppointmentStatus NewStatus { get; private set; }

    public DateTimeOffset ChangedAt { get; private set; }

    public string ChangedBy { get; private set; }

    public DateTimeOffset? PreviousAppointmentDateTime { get; private set; }

    public DateTimeOffset? NewAppointmentDateTime { get; private set; }

    public Appointment? Appointment { get; private set; }

    internal static AppointmentStatusHistory CreateStatusChange(
        Guid id,
        Guid appointmentId,
        AppointmentStatus previousStatus,
        AppointmentStatus newStatus,
        DateTimeOffset changedAt,
        string changedBy)
    {
        ValidateCommon(id, appointmentId, changedBy);

        return new AppointmentStatusHistory(
            id,
            appointmentId,
            AppointmentHistoryAction.StatusChange,
            previousStatus,
            newStatus,
            changedAt,
            changedBy.Trim(),
            previousAppointmentDateTime: null,
            newAppointmentDateTime: null);
    }

    internal static AppointmentStatusHistory CreateReschedule(
        Guid id,
        Guid appointmentId,
        AppointmentStatus activeStatus,
        DateTimeOffset previousAppointmentDateTime,
        DateTimeOffset newAppointmentDateTime,
        DateTimeOffset changedAt,
        string changedBy)
    {
        ValidateCommon(id, appointmentId, changedBy);

        return new AppointmentStatusHistory(
            id,
            appointmentId,
            AppointmentHistoryAction.Reschedule,
            activeStatus,
            AppointmentStatus.Rescheduled,
            changedAt,
            changedBy.Trim(),
            previousAppointmentDateTime,
            newAppointmentDateTime);
    }

    internal void AssignAppointment(Appointment appointment)
    {
        Appointment = appointment;
        AppointmentId = appointment.Id;
    }

    private static void ValidateCommon(Guid id, Guid appointmentId, string changedBy)
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
    }
}
