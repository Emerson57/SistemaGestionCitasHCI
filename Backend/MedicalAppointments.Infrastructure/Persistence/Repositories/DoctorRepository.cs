using MedicalAppointments.Application.Abstractions.Persistence;
using MedicalAppointments.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MedicalAppointments.Infrastructure.Persistence.Repositories;

public sealed class DoctorRepository(ApplicationDbContext context) : IDoctorRepository
{
    public async Task<Doctor?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        await context.Doctors
            .AsNoTracking()
            .Include(d => d.Specialty)
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);

    public async Task<Doctor?> GetByUserIdAsync(string userId, CancellationToken cancellationToken) =>
        await context.Doctors
            .Include(d => d.Specialty)
            .FirstOrDefaultAsync(d => d.UserId == userId, cancellationToken);

    public async Task<IReadOnlyList<Doctor>> GetAllAsync(CancellationToken cancellationToken) =>
        await context.Doctors
            .AsNoTracking()
            .Include(d => d.Specialty)
            .OrderBy(d => d.FullName)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Doctor>> GetBySpecialtyAsync(
        int specialtyId,
        CancellationToken cancellationToken) =>
        await context.Doctors
            .AsNoTracking()
            .Include(d => d.Specialty)
            .Where(d => d.SpecialtyId == specialtyId)
            .OrderBy(d => d.FullName)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(Doctor doctor, CancellationToken cancellationToken) =>
        await context.Doctors.AddAsync(doctor, cancellationToken);

    public void Update(Doctor doctor) =>
        context.Doctors.UpdateIfDetached(doctor);
}
