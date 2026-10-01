using MedicalAppointments.Domain.Entities;

namespace MedicalAppointments.Application.Abstractions.Persistence;

public interface IDoctorRepository
{
    Task<Doctor?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<Doctor?> GetByUserIdAsync(string userId, CancellationToken cancellationToken);

    Task<IReadOnlyList<Doctor>> GetAllAsync(CancellationToken cancellationToken);

    Task<IReadOnlyList<Doctor>> GetBySpecialtyAsync(int specialtyId, CancellationToken cancellationToken);

    Task AddAsync(Doctor doctor, CancellationToken cancellationToken);

    void Update(Doctor doctor);
}
