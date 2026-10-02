using MedicalAppointments.Domain.Entities;

namespace MedicalAppointments.Application.Abstractions.Persistence;

public interface IDoctorAvailabilityRepository
{
    Task<DoctorAvailability?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyList<DoctorAvailability>> GetAvailableByDoctorAsync(
        Guid doctorId,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<DoctorAvailability>> GetAllByDoctorAsync(
        Guid doctorId,
        CancellationToken cancellationToken);

    Task<bool> HasOverlappingSlotAsync(
        Guid doctorId,
        DateOnly date,
        TimeOnly startTime,
        TimeOnly endTime,
        Guid? excludeAvailabilityId,
        CancellationToken cancellationToken);

    Task AddAsync(DoctorAvailability availability, CancellationToken cancellationToken);

    void Update(DoctorAvailability availability);
}
