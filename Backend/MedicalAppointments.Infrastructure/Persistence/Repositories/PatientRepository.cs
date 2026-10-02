using MedicalAppointments.Application.Abstractions.Persistence;
using MedicalAppointments.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MedicalAppointments.Infrastructure.Persistence.Repositories;

public sealed class PatientRepository(ApplicationDbContext context) : IPatientRepository
{
    public async Task<Patient?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        await context.Patients
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public async Task<Patient?> GetByUserIdAsync(string userId, CancellationToken cancellationToken) =>
        await context.Patients
            .FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);

    public async Task<IReadOnlyList<Patient>> GetAllAsync(CancellationToken cancellationToken) =>
        await context.Patients
            .AsNoTracking()
            .OrderBy(p => p.FullName)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(Patient patient, CancellationToken cancellationToken) =>
        await context.Patients.AddAsync(patient, cancellationToken);

    public void Update(Patient patient) =>
        context.Patients.UpdateIfDetached(patient);
}
