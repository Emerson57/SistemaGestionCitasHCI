using MedicalAppointments.Domain.Entities;

namespace MedicalAppointments.Application.Abstractions.Persistence;

public interface IPatientRepository
{
    Task<Patient?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<Patient?> GetByUserIdAsync(string userId, CancellationToken cancellationToken);

    Task<IReadOnlyList<Patient>> GetAllAsync(CancellationToken cancellationToken);

    Task AddAsync(Patient patient, CancellationToken cancellationToken);

    void Update(Patient patient);
}
