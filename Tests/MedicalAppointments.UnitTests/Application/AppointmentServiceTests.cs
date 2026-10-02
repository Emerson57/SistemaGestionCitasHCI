using MedicalAppointments.Application.DTOs.Appointments;
using MedicalAppointments.Application.Features.Appointments;
using MedicalAppointments.Domain.Entities;
using MedicalAppointments.Domain.Enums;
using MedicalAppointments.UnitTests.Application.Fakes;

namespace MedicalAppointments.UnitTests.Application;

public class AppointmentServiceTests
{
    private static readonly DateTimeOffset Now = new(2026, 10, 1, 12, 0, 0, TimeSpan.Zero);
    private static readonly DateTimeOffset AppointmentTime = new(2026, 10, 4, 10, 0, 0, TimeSpan.Zero);

    [Fact]
    public async Task ScheduleAsync_RejectsUnauthenticatedUser()
    {
        var context = CreateContext();
        context.CurrentUser.IsAuthenticated = false;

        var result = await context.Service.ScheduleAsync(
            new ScheduleAppointmentRequest
            {
                DoctorId = Guid.NewGuid(),
                AppointmentDateTime = AppointmentTime
            },
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("Unauthorized", result.Error!.Code);
    }

    [Fact]
    public async Task ScheduleAsync_RejectsDoctorNotFound()
    {
        var context = CreateContext();
        SeedPatient(context, "patient-1", "patient-1");
        AuthenticatePatient(context, "patient-1");

        var result = await context.Service.ScheduleAsync(
            new ScheduleAppointmentRequest
            {
                DoctorId = Guid.NewGuid(),
                AppointmentDateTime = AppointmentTime
            },
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("NotFound", result.Error!.Code);
    }

    [Fact]
    public async Task ScheduleAsync_RejectsInactiveDoctor()
    {
        var context = CreateContext();
        SeedPatient(context, "patient-1", "patient-1");
        AuthenticatePatient(context, "patient-1");

        var doctorId = Guid.NewGuid();
        var doctor = Doctor.Create(doctorId, "doctor-user", 1, "Dr. Inactive", "LIC-1", Now);
        doctor.Deactivate(Now);
        context.DoctorRepository.Seed(doctor);

        var result = await context.Service.ScheduleAsync(
            new ScheduleAppointmentRequest
            {
                DoctorId = doctorId,
                AppointmentDateTime = AppointmentTime
            },
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("Validation", result.Error!.Code);
    }

    [Fact]
    public async Task ScheduleAsync_RejectsOccupiedSlot()
    {
        var context = CreateContext();
        var doctorId = SeedDoctorWithAvailability(context);
        var patient = SeedPatient(context, "patient-1", "user-1");

        AuthenticatePatient(context, "user-1");

        var existing = Appointment.Create(
            Guid.NewGuid(),
            patient.Id,
            doctorId,
            AppointmentTime,
            null,
            Now);

        context.AppointmentRepository.Seed(existing);

        var result = await context.Service.ScheduleAsync(
            new ScheduleAppointmentRequest
            {
                DoctorId = doctorId,
                AppointmentDateTime = AppointmentTime
            },
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("Conflict", result.Error!.Code);
    }

    [Fact]
    public async Task ScheduleAsync_SchedulesValidAppointment()
    {
        var context = CreateContext();
        var doctorId = SeedDoctorWithAvailability(context);
        SeedPatient(context, "patient-1", "user-1");
        AuthenticatePatient(context, "user-1");

        var result = await context.Service.ScheduleAsync(
            new ScheduleAppointmentRequest
            {
                DoctorId = doctorId,
                AppointmentDateTime = AppointmentTime,
                Reason = "Checkup"
            },
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(AppointmentStatus.Scheduled, result.Value!.Status);
        Assert.Equal("Checkup", result.Value.Reason);
    }

    [Fact]
    public async Task GetMyAppointmentsAsync_ReturnsOnlyCurrentPatientAppointments()
    {
        var context = CreateContext();
        var doctorId = SeedDoctorWithAvailability(context);

        var patientA = SeedPatient(context, "patient-a", "user-a");
        var patientB = SeedPatient(context, "patient-b", "user-b");

        context.AppointmentRepository.Seed(Appointment.Create(
            Guid.NewGuid(), patientA.Id, doctorId, AppointmentTime, null, Now));
        context.AppointmentRepository.Seed(Appointment.Create(
            Guid.NewGuid(), patientB.Id, doctorId, AppointmentTime.AddHours(1), null, Now));

        AuthenticatePatient(context, "user-a");

        var result = await context.Service.GetMyAppointmentsAsync(CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value!);
        Assert.Equal(patientA.Id, result.Value![0].PatientId);
    }

    [Fact]
    public async Task RescheduleAsync_KeepsScheduledStatusAfterReschedule()
    {
        var context = CreateContext();
        var doctorId = SeedDoctorWithAvailability(context);
        SeedPatient(context, "patient-1", "user-1");
        AuthenticatePatient(context, "user-1");

        var scheduleResult = await context.Service.ScheduleAsync(
            new ScheduleAppointmentRequest
            {
                DoctorId = doctorId,
                AppointmentDateTime = AppointmentTime,
                Reason = "Initial"
            },
            CancellationToken.None);

        Assert.True(scheduleResult.IsSuccess);

        var newDateTime = new DateTimeOffset(2026, 10, 4, 11, 0, 0, TimeSpan.Zero);
        var rescheduleResult = await context.Service.RescheduleAsync(
            scheduleResult.Value!.Id,
            new RescheduleAppointmentRequest { AppointmentDateTime = newDateTime },
            CancellationToken.None);

        Assert.True(rescheduleResult.IsSuccess);
        Assert.Equal(AppointmentStatus.Scheduled, rescheduleResult.Value!.Status);
        Assert.Equal(newDateTime, rescheduleResult.Value.AppointmentDateTime);
    }

    [Fact]
    public async Task CancelAsync_ReleasesReservedAvailability()
    {
        var context = CreateContext();
        var doctorId = SeedDoctorWithAvailability(context);
        SeedPatient(context, "patient-1", "user-1");
        AuthenticatePatient(context, "user-1");

        var scheduleResult = await context.Service.ScheduleAsync(
            new ScheduleAppointmentRequest
            {
                DoctorId = doctorId,
                AppointmentDateTime = AppointmentTime
            },
            CancellationToken.None);

        Assert.True(scheduleResult.IsSuccess);

        var cancelResult = await context.Service.CancelAsync(scheduleResult.Value!.Id, CancellationToken.None);
        Assert.True(cancelResult.IsSuccess);

        var slots = await context.AvailabilityRepository.GetAllByDoctorAsync(doctorId, CancellationToken.None);
        Assert.All(slots, slot => Assert.Equal(AvailabilityStatus.Available, slot.Status));
    }

    [Fact]
    public async Task CancelAsync_CancelledAppointmentAppearsInHistory()
    {
        var context = CreateContext();
        var doctorId = SeedDoctorWithAvailability(context);
        SeedPatient(context, "patient-1", "user-1");
        AuthenticatePatient(context, "user-1");

        var scheduleResult = await context.Service.ScheduleAsync(
            new ScheduleAppointmentRequest
            {
                DoctorId = doctorId,
                AppointmentDateTime = AppointmentTime
            },
            CancellationToken.None);

        await context.Service.CancelAsync(scheduleResult.Value!.Id, CancellationToken.None);

        var history = await context.Service.GetMyHistoryAsync(CancellationToken.None);
        Assert.True(history.IsSuccess);
        Assert.Single(history.Value!);
        Assert.Equal(AppointmentStatus.Cancelled, history.Value![0].Status);
    }

    [Fact]
    public async Task ConfirmAsync_ForbidsAnotherDoctorsAppointment()
    {
        var context = CreateContext();
        var doctorAId = Guid.NewGuid();
        var doctorBId = Guid.NewGuid();
        context.DoctorRepository.Seed(Doctor.Create(doctorAId, "doctor-a", 1, "Dr. A", "LIC-A", Now));
        context.DoctorRepository.Seed(Doctor.Create(doctorBId, "doctor-b", 1, "Dr. B", "LIC-B", Now));

        var patient = SeedPatient(context, "patient-1", "user-1");
        var appointment = Appointment.Create(
            Guid.NewGuid(),
            patient.Id,
            doctorAId,
            AppointmentTime,
            null,
            Now);
        context.AppointmentRepository.Seed(appointment);

        AuthenticateDoctor(context, "doctor-b");

        var result = await context.Service.ConfirmAsync(appointment.Id, CancellationToken.None);
        Assert.True(result.IsFailure);
        Assert.Equal("Forbidden", result.Error!.Code);
    }

    [Fact]
    public async Task CancelAsync_PreventsCancellationOfAnotherPatientsAppointment()
    {
        var context = CreateContext();
        var doctorId = SeedDoctorWithAvailability(context);

        var owner = SeedPatient(context, "owner", "owner-user");
        var other = SeedPatient(context, "other", "other-user");

        var appointment = Appointment.Create(
            Guid.NewGuid(),
            owner.Id,
            doctorId,
            AppointmentTime,
            null,
            Now);

        context.AppointmentRepository.Seed(appointment);

        AuthenticatePatient(context, "other-user");

        var result = await context.Service.CancelAsync(appointment.Id, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("Forbidden", result.Error!.Code);
    }

    private static TestContext CreateContext()
    {
        var specialtyRepository = new InMemorySpecialtyRepository();
        var specialty = Specialty.Create("General Medicine", null, Now);
        specialty.AssignIdentity(1);
        specialtyRepository.Seed(specialty);

        return new TestContext
        {
            SpecialtyRepository = specialtyRepository,
            DoctorRepository = new InMemoryDoctorRepository(),
            PatientRepository = new InMemoryPatientRepository(),
            AvailabilityRepository = new InMemoryDoctorAvailabilityRepository(),
            AppointmentRepository = new InMemoryAppointmentRepository(),
            CurrentUser = new FakeCurrentUserService(),
            UnitOfWork = new FakeUnitOfWork(),
            DateTimeProvider = new FakeDateTimeProvider(Now)
        };
    }

    private static Guid SeedDoctorWithAvailability(TestContext context)
    {
        var doctorId = Guid.NewGuid();
        context.DoctorRepository.Seed(
            Doctor.Create(doctorId, "doctor-user", 1, "Dr. Active", "LIC-ACTIVE", Now));

        var date = DateOnly.FromDateTime(AppointmentTime.UtcDateTime);
        var start = new TimeOnly(9, 0);
        var end = new TimeOnly(17, 0);

        context.AvailabilityRepository.Seed(
            DoctorAvailability.Create(Guid.NewGuid(), doctorId, date, start, end, Now));

        return doctorId;
    }

    private static Patient SeedPatient(TestContext context, string _, string userId)
    {
        var patient = Patient.Create(
            Guid.NewGuid(),
            userId,
            "Patient Name",
            new DateOnly(1990, 1, 1),
            "Address",
            "555-0100",
            Sex.Female,
            null,
            MaritalStatus.Single,
            Now);

        context.PatientRepository.Seed(patient);
        return patient;
    }

    private static void AuthenticatePatient(TestContext context, string userId)
    {
        context.CurrentUser.IsAuthenticated = true;
        context.CurrentUser.UserId = userId;
        context.CurrentUser.Role = "Patient";
    }

    private static void AuthenticateDoctor(TestContext context, string userId)
    {
        context.CurrentUser.IsAuthenticated = true;
        context.CurrentUser.UserId = userId;
        context.CurrentUser.Role = "Doctor";
    }

    private sealed class TestContext
    {
        public required InMemorySpecialtyRepository SpecialtyRepository { get; init; }

        public required InMemoryDoctorRepository DoctorRepository { get; init; }

        public required InMemoryPatientRepository PatientRepository { get; init; }

        public required InMemoryDoctorAvailabilityRepository AvailabilityRepository { get; init; }

        public required InMemoryAppointmentRepository AppointmentRepository { get; init; }

        public required FakeCurrentUserService CurrentUser { get; init; }

        public required FakeUnitOfWork UnitOfWork { get; init; }

        public required FakeDateTimeProvider DateTimeProvider { get; init; }

        public AppointmentService Service => new(
            AppointmentRepository,
            PatientRepository,
            DoctorRepository,
            SpecialtyRepository,
            AvailabilityRepository,
            CurrentUser,
            UnitOfWork,
            DateTimeProvider);
    }
}
