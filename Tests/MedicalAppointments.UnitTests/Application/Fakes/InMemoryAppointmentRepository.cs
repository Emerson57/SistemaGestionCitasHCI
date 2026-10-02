using MedicalAppointments.Application.Abstractions.Persistence;
using MedicalAppointments.Domain.Entities;
using MedicalAppointments.Domain.Enums;

namespace MedicalAppointments.UnitTests.Application.Fakes;

public sealed class InMemoryAppointmentRepository : IAppointmentRepository
{
    private readonly List<Appointment> _appointments = [];

    public Task AddAsync(Appointment appointment, CancellationToken cancellationToken)
    {
        _appointments.Add(appointment);
        return Task.CompletedTask;
    }

    public Task<bool> ExistsActiveAppointmentForSlotAsync(
        Guid doctorId,
        DateTimeOffset appointmentDateTime,
        Guid? excludeAppointmentId,
        CancellationToken cancellationToken)
    {
        var exists = _appointments.Any(a =>
            a.DoctorId == doctorId
            && a.AppointmentDateTime == appointmentDateTime
            && (excludeAppointmentId is null || a.Id != excludeAppointmentId)
            && a.Status is AppointmentStatus.Scheduled
                or AppointmentStatus.Confirmed);

        return Task.FromResult(exists);
    }

    public Task<Appointment?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        Task.FromResult(_appointments.FirstOrDefault(a => a.Id == id));

    public Task<IReadOnlyList<Appointment>> GetByDoctorAsync(Guid doctorId, CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyList<Appointment>>(_appointments.Where(a => a.DoctorId == doctorId).ToList());

    public Task<IReadOnlyList<Appointment>> GetByPatientAsync(Guid patientId, CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyList<Appointment>>(_appointments.Where(a => a.PatientId == patientId).ToList());

    public Task<IReadOnlyList<Appointment>> GetHistoryByPatientAsync(
        Guid patientId,
        CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyList<Appointment>>(
            _appointments.Where(a =>
                a.PatientId == patientId
                && a.Status is AppointmentStatus.Completed or AppointmentStatus.Cancelled).ToList());

    public void Update(Appointment appointment)
    {
    }

    public void Seed(Appointment appointment) => _appointments.Add(appointment);
}
