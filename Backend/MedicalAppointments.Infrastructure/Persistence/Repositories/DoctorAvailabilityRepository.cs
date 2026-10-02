using MedicalAppointments.Application.Abstractions.Persistence;
using MedicalAppointments.Domain.Entities;
using MedicalAppointments.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace MedicalAppointments.Infrastructure.Persistence.Repositories;

public sealed class DoctorAvailabilityRepository(ApplicationDbContext context) : IDoctorAvailabilityRepository
{
    public async Task<DoctorAvailability?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        await context.DoctorAvailabilities
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

    public async Task<IReadOnlyList<DoctorAvailability>> GetAvailableByDoctorAsync(
        Guid doctorId,
        CancellationToken cancellationToken) =>
        await context.DoctorAvailabilities
            .AsNoTracking()
            .Where(a => a.DoctorId == doctorId && a.Status == AvailabilityStatus.Available)
            .OrderBy(a => a.Date)
            .ThenBy(a => a.StartTime)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<DoctorAvailability>> GetAllByDoctorAsync(
        Guid doctorId,
        CancellationToken cancellationToken) =>
        await context.DoctorAvailabilities
            .Where(a => a.DoctorId == doctorId)
            .OrderBy(a => a.Date)
            .ThenBy(a => a.StartTime)
            .ToListAsync(cancellationToken);

    public async Task<bool> HasOverlappingSlotAsync(
        Guid doctorId,
        DateOnly date,
        TimeOnly startTime,
        TimeOnly endTime,
        Guid? excludeAvailabilityId,
        CancellationToken cancellationToken) =>
        await context.DoctorAvailabilities
            .AsNoTracking()
            .AnyAsync(
                a => a.DoctorId == doctorId
                    && a.Date == date
                    && a.Status != AvailabilityStatus.Unavailable
                    && (excludeAvailabilityId == null || a.Id != excludeAvailabilityId)
                    && a.StartTime < endTime
                    && a.EndTime > startTime,
                cancellationToken);

    public async Task AddAsync(DoctorAvailability availability, CancellationToken cancellationToken) =>
        await context.DoctorAvailabilities.AddAsync(availability, cancellationToken);

    public void Update(DoctorAvailability availability) =>
        context.DoctorAvailabilities.UpdateIfDetached(availability);
}
