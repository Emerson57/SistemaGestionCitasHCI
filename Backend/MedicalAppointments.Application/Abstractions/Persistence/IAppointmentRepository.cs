using MedicalAppointments.Domain.Entities;

namespace MedicalAppointments.Application.Abstractions.Persistence;

public interface IAppointmentRepository
{
    Task<Appointment?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyList<Appointment>> GetByPatientAsync(Guid patientId, CancellationToken cancellationToken);

    Task<IReadOnlyList<Appointment>> GetHistoryByPatientAsync(Guid patientId, CancellationToken cancellationToken);

    Task<IReadOnlyList<Appointment>> GetByDoctorAsync(Guid doctorId, CancellationToken cancellationToken);

    Task<bool> ExistsActiveAppointmentForSlotAsync(
        Guid doctorId,
        DateTimeOffset appointmentDateTime,
        Guid? excludeAppointmentId,
        CancellationToken cancellationToken);

    Task AddAsync(Appointment appointment, CancellationToken cancellationToken);

    void Update(Appointment appointment);
}
