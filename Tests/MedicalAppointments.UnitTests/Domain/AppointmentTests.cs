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
