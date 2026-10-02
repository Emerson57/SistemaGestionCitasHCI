using MedicalAppointments.Domain.Entities;
using MedicalAppointments.Domain.Enums;
using MedicalAppointments.Domain.Exceptions;

namespace MedicalAppointments.UnitTests.Domain;

public class AppointmentTests
{
    private static readonly DateTimeOffset Now = new(2026, 10, 1, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Create_WithValidValues_Succeeds()
    {
        var patientId = Guid.NewGuid();
        var doctorId = Guid.NewGuid();
        var appointmentDateTime = Now.AddDays(1);

        var appointment = Appointment.Create(
            Guid.NewGuid(),
            patientId,
            doctorId,
            appointmentDateTime,
            "Routine checkup",
            Now);

        Assert.Equal(patientId, appointment.PatientId);
        Assert.Equal(doctorId, appointment.DoctorId);
        Assert.Equal(appointmentDateTime, appointment.AppointmentDateTime);
        Assert.Equal(AppointmentStatus.Scheduled, appointment.Status);
        Assert.Equal("Routine checkup", appointment.Reason);
        Assert.Empty(appointment.StatusHistory);
    }

    [Fact]
    public void Create_WithEmptyPatientId_ThrowsDomainException()
    {
        var exception = Assert.Throws<DomainException>(() =>
            Appointment.Create(
                Guid.NewGuid(),
                Guid.Empty,
                Guid.NewGuid(),
                Now.AddDays(1),
                null,
                Now));

        Assert.Contains("Patient id", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Confirm_FromScheduled_Succeeds()
    {
        var appointment = CreateScheduledAppointment();

        appointment.Confirm("patient-user", Now);

        Assert.Equal(AppointmentStatus.Confirmed, appointment.Status);
    }

    [Fact]
    public void Cancel_FromScheduled_Succeeds()
    {
        var appointment = CreateScheduledAppointment();

        appointment.Cancel("patient-user", Now);

        Assert.Equal(AppointmentStatus.Cancelled, appointment.Status);
    }

    [Fact]
    public void Confirm_FromCancelled_ThrowsDomainException()
    {
        var appointment = CreateScheduledAppointment();
        appointment.Cancel("patient-user", Now);

        var exception = Assert.Throws<DomainException>(() =>
            appointment.Confirm("patient-user", Now.AddMinutes(1)));

        Assert.Contains("Cannot transition", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void StatusChange_CreatesHistoryEntry()
    {
        var appointment = CreateScheduledAppointment();

        appointment.Confirm("admin-user", Now);

        Assert.Single(appointment.StatusHistory);
        var entry = appointment.StatusHistory.First();
        Assert.Equal(AppointmentStatus.Scheduled, entry.PreviousStatus);
        Assert.Equal(AppointmentStatus.Confirmed, entry.NewStatus);
        Assert.Equal("admin-user", entry.ChangedBy);
        Assert.Equal(appointment.Id, entry.AppointmentId);
    }

    [Fact]
    public void Reschedule_FromScheduled_KeepsScheduledStatus()
    {
        var appointment = CreateScheduledAppointment();
        var newDateTime = Now.AddDays(5);

        appointment.Reschedule(newDateTime, "patient-user", Now);

        Assert.Equal(AppointmentStatus.Scheduled, appointment.Status);
    }

    [Fact]
    public void Reschedule_FromConfirmed_KeepsConfirmedStatus()
    {
        var appointment = CreateScheduledAppointment();
        appointment.Confirm("doctor-user", Now);
        var newDateTime = Now.AddDays(6);

        appointment.Reschedule(newDateTime, "patient-user", Now.AddMinutes(1));

        Assert.Equal(AppointmentStatus.Confirmed, appointment.Status);
    }

    [Fact]
    public void Reschedule_UpdatesAppointmentDateTime()
    {
        var appointment = CreateScheduledAppointment();
        var newDateTime = Now.AddDays(7);

        appointment.Reschedule(newDateTime, "patient-user", Now);

        Assert.Equal(newDateTime, appointment.AppointmentDateTime);
    }

    [Fact]
    public void Reschedule_CreatesHistoryEntry()
    {
        var appointment = CreateScheduledAppointment();
        var originalDateTime = appointment.AppointmentDateTime;
        var newDateTime = Now.AddDays(8);

        appointment.Reschedule(newDateTime, "patient-user", Now);

        Assert.Single(appointment.StatusHistory);
        var entry = appointment.StatusHistory.First();
        Assert.Equal(AppointmentHistoryAction.Reschedule, entry.Action);
        Assert.Equal(AppointmentStatus.Scheduled, entry.PreviousStatus);
        Assert.Equal(AppointmentStatus.Rescheduled, entry.NewStatus);
        Assert.Equal(originalDateTime, entry.PreviousAppointmentDateTime);
        Assert.Equal(newDateTime, entry.NewAppointmentDateTime);
    }

    [Fact]
    public void Reschedule_FromCancelled_ThrowsDomainException()
    {
        var appointment = CreateScheduledAppointment();
        appointment.Cancel("patient-user", Now);

        var exception = Assert.Throws<DomainException>(() =>
            appointment.Reschedule(Now.AddDays(3), "patient-user", Now.AddMinutes(1)));

        Assert.Contains("Cannot reschedule", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Reschedule_FromCompleted_ThrowsDomainException()
    {
        var appointment = CreateScheduledAppointment();
        appointment.Confirm("doctor-user", Now);
        appointment.Complete("doctor-user", Now.AddMinutes(1));

        var exception = Assert.Throws<DomainException>(() =>
            appointment.Reschedule(Now.AddDays(3), "patient-user", Now.AddMinutes(2)));

        Assert.Contains("Cannot reschedule", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    private static Appointment CreateScheduledAppointment()
    {
        return Appointment.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Now.AddDays(2),
            null,
            Now);
    }
}
