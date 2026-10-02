using MedicalAppointments.Domain.Enums;
using MedicalAppointments.Domain.Exceptions;

namespace MedicalAppointments.Domain.Entities;

public class Appointment
{
    private readonly List<AppointmentStatusHistory> _statusHistory = [];

    private Appointment()
    {
    }

    private Appointment(
        Guid id,
        Guid patientId,
        Guid doctorId,
        DateTimeOffset appointmentDateTime,
        string? reason,
        DateTimeOffset createdAt)
    {
        Id = id;
        PatientId = patientId;
        DoctorId = doctorId;
        AppointmentDateTime = appointmentDateTime;
        Status = AppointmentStatus.Scheduled;
        Reason = reason;
        CreatedAt = createdAt;
    }

    public Guid Id { get; private set; }

    public Guid PatientId { get; private set; }

    public Guid DoctorId { get; private set; }

    public DateTimeOffset AppointmentDateTime { get; private set; }

    public AppointmentStatus Status { get; private set; }

    public string? Reason { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset? UpdatedAt { get; private set; }

    public Patient? Patient { get; private set; }

    public Doctor? Doctor { get; private set; }

    public IReadOnlyCollection<AppointmentStatusHistory> StatusHistory => _statusHistory.AsReadOnly();

    public static Appointment Create(
        Guid id,
        Guid patientId,
        Guid doctorId,
        DateTimeOffset appointmentDateTime,
        string? reason,
        DateTimeOffset createdAt)
    {
        if (id == Guid.Empty)
        {
            throw new DomainException("Appointment id cannot be empty.");
        }

        if (patientId == Guid.Empty)
        {
            throw new DomainException("Patient id cannot be empty.");
        }

        if (doctorId == Guid.Empty)
        {
            throw new DomainException("Doctor id cannot be empty.");
        }

        return new Appointment(id, patientId, doctorId, appointmentDateTime, reason?.Trim(), createdAt);
    }

    public void Confirm(string changedBy, DateTimeOffset changedAt)
    {
        ChangeStatus(AppointmentStatus.Confirmed, changedBy, changedAt);
    }

    public void Cancel(string changedBy, DateTimeOffset changedAt)
    {
        ChangeStatus(AppointmentStatus.Cancelled, changedBy, changedAt);
    }

    public void Complete(string changedBy, DateTimeOffset changedAt)
    {
        ChangeStatus(AppointmentStatus.Completed, changedBy, changedAt);
    }

    public void Reschedule(DateTimeOffset newAppointmentDateTime, string changedBy, DateTimeOffset changedAt)
    {
        if (string.IsNullOrWhiteSpace(changedBy))
        {
            throw new DomainException("Changed by cannot be empty.");
        }

        if (newAppointmentDateTime == default)
        {
            throw new DomainException("Appointment date and time must be specified.");
        }

        if (Status is not AppointmentStatus.Scheduled and not AppointmentStatus.Confirmed)
        {
            throw new DomainException($"Cannot reschedule appointment in {Status} status.");
        }

        var activeStatus = Status;
        var previousDateTime = AppointmentDateTime;
        AppointmentDateTime = newAppointmentDateTime;
        UpdatedAt = changedAt;

        var historyEntry = AppointmentStatusHistory.CreateReschedule(
            Guid.NewGuid(),
            Id,
            activeStatus,
            previousDateTime,
            newAppointmentDateTime,
            changedAt,
            changedBy);

        historyEntry.AssignAppointment(this);
        _statusHistory.Add(historyEntry);
    }

    internal void AssignPatient(Patient patient)
    {
        Patient = patient;
        PatientId = patient.Id;
    }

    internal void AssignDoctor(Doctor doctor)
    {
        Doctor = doctor;
        DoctorId = doctor.Id;
    }

    private void ChangeStatus(AppointmentStatus newStatus, string changedBy, DateTimeOffset changedAt)
    {
        if (string.IsNullOrWhiteSpace(changedBy))
        {
            throw new DomainException("Changed by cannot be empty.");
        }

        if (!IsTransitionAllowed(Status, newStatus))
        {
            throw new DomainException(
                $"Cannot transition appointment from {Status} to {newStatus}.");
        }

        var previousStatus = Status;
        Status = newStatus;
        UpdatedAt = changedAt;

        var historyEntry = AppointmentStatusHistory.CreateStatusChange(
            Guid.NewGuid(),
            Id,
            previousStatus,
            newStatus,
            changedAt,
            changedBy);

        historyEntry.AssignAppointment(this);
        _statusHistory.Add(historyEntry);
    }

    private static bool IsTransitionAllowed(AppointmentStatus current, AppointmentStatus next)
    {
        if (current == next)
        {
            return false;
        }

        return current switch
        {
            AppointmentStatus.Scheduled => next is AppointmentStatus.Confirmed
                or AppointmentStatus.Cancelled,
            AppointmentStatus.Confirmed => next is AppointmentStatus.Completed
                or AppointmentStatus.Cancelled,
            AppointmentStatus.Completed => false,
            AppointmentStatus.Cancelled => false,
            AppointmentStatus.Rescheduled => false,
            _ => false
        };
    }
}
