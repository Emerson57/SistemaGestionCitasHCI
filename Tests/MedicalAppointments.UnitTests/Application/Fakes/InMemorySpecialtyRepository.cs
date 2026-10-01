using MedicalAppointments.Application.Abstractions.Persistence;
using MedicalAppointments.Domain.Entities;

namespace MedicalAppointments.UnitTests.Application.Fakes;

public sealed class InMemorySpecialtyRepository : ISpecialtyRepository
{
    private readonly List<Specialty> _specialties = [];

    public Task AddAsync(Specialty specialty, CancellationToken cancellationToken)
    {
        _specialties.Add(specialty);
        return Task.CompletedTask;
    }

    public Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken)
    {
        var exists = _specialties.Any(s =>
            string.Equals(s.Name, name.Trim(), StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(exists);
    }

    public Task<IReadOnlyList<Specialty>> GetActiveAsync(CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyList<Specialty>>(_specialties.Where(s => s.IsActive).ToList());

    public Task<IReadOnlyList<Specialty>> GetAllAsync(CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyList<Specialty>>(_specialties.ToList());

    public Task<Specialty?> GetByIdAsync(int id, CancellationToken cancellationToken) =>
        Task.FromResult(_specialties.FirstOrDefault(s => s.Id == id));

    public void Update(Specialty specialty)
    {
    }

    public void Seed(Specialty specialty) => _specialties.Add(specialty);
}
