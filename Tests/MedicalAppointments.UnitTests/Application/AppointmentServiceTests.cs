using MedicalAppointments.Application.DTOs.Appointments;
using MedicalAppointments.Application.Features.Appointments;
using MedicalAppointments.Domain.Entities;
using MedicalAppointments.Domain.Enums;
using MedicalAppointments.UnitTests.Application.Fakes;

namespace MedicalAppointments.UnitTests.Application;

public class AppointmentServiceTests
{
    private static readonly DateTimeOffset Now = new(2026, 10, 1, 12, 0, 0, TimeSpan.Zero);
    private static readonly DateTimeOffset AppointmentTime = Now.AddDays(3).AddHours(10);

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
        specialtyRepository.Seed(Specialty.Create(1, "General Medicine", null, Now));

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
