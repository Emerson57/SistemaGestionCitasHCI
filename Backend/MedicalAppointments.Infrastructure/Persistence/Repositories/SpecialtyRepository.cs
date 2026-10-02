using MedicalAppointments.Application.Abstractions.Persistence;
using MedicalAppointments.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MedicalAppointments.Infrastructure.Persistence.Repositories;

public sealed class SpecialtyRepository(ApplicationDbContext context) : ISpecialtyRepository
{
    public async Task<Specialty?> GetByIdAsync(int id, CancellationToken cancellationToken) =>
        await context.Specialties
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Specialty>> GetAllAsync(CancellationToken cancellationToken) =>
        await context.Specialties
            .AsNoTracking()
            .OrderBy(s => s.Name)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Specialty>> GetActiveAsync(CancellationToken cancellationToken) =>
        await context.Specialties
            .AsNoTracking()
            .Where(s => s.IsActive)
            .OrderBy(s => s.Name)
            .ToListAsync(cancellationToken);

    public async Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken)
    {
        var normalized = name.Trim();
        var normalizedLower = normalized.ToLowerInvariant();
        return await context.Specialties
            .AsNoTracking()
            .AnyAsync(s => s.Name.ToLower() == normalizedLower, cancellationToken);
    }

    public async Task AddAsync(Specialty specialty, CancellationToken cancellationToken) =>
        await context.Specialties.AddAsync(specialty, cancellationToken);

    public void Update(Specialty specialty) =>
        context.Specialties.UpdateIfDetached(specialty);
}
