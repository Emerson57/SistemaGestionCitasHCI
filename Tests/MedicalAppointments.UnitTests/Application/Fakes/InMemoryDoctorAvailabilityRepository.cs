using MedicalAppointments.Application.Abstractions.Persistence;
using MedicalAppointments.Domain.Entities;
using MedicalAppointments.Domain.Enums;

namespace MedicalAppointments.UnitTests.Application.Fakes;

public sealed class InMemoryDoctorAvailabilityRepository : IDoctorAvailabilityRepository
{
    private readonly List<DoctorAvailability> _slots = [];

    public Task AddAsync(DoctorAvailability availability, CancellationToken cancellationToken)
    {
        _slots.Add(availability);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<DoctorAvailability>> GetAvailableByDoctorAsync(
        Guid doctorId,
        CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyList<DoctorAvailability>>(
            _slots.Where(s => s.DoctorId == doctorId && s.Status == AvailabilityStatus.Available).ToList());

    public Task<DoctorAvailability?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        Task.FromResult(_slots.FirstOrDefault(s => s.Id == id));

    public Task<bool> HasOverlappingSlotAsync(
        Guid doctorId,
        DateOnly date,
        TimeOnly startTime,
        TimeOnly endTime,
        Guid? excludeAvailabilityId,
        CancellationToken cancellationToken)
    {
        var overlap = _slots.Any(s =>
            s.DoctorId == doctorId
            && s.Date == date
            && (excludeAvailabilityId is null || s.Id != excludeAvailabilityId)
            && startTime < s.EndTime
            && endTime > s.StartTime);

        return Task.FromResult(overlap);
    }

    public void Update(DoctorAvailability availability)
    {
    }

    public void Seed(DoctorAvailability availability) => _slots.Add(availability);
}
