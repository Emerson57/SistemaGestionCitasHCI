using MedicalAppointments.Domain.Entities;

namespace MedicalAppointments.Application.Abstractions.Persistence;

public interface ISpecialtyRepository
{
    Task<Specialty?> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<IReadOnlyList<Specialty>> GetAllAsync(CancellationToken cancellationToken);

    Task<IReadOnlyList<Specialty>> GetActiveAsync(CancellationToken cancellationToken);

    Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken);

    Task AddAsync(Specialty specialty, CancellationToken cancellationToken);

    void Update(Specialty specialty);
}
