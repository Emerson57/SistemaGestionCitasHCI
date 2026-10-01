using MedicalAppointments.Application.Abstractions.Persistence;
using MedicalAppointments.Domain.Entities;

namespace MedicalAppointments.UnitTests.Application.Fakes;

public sealed class InMemoryDoctorRepository : IDoctorRepository
{
    private readonly List<Doctor> _doctors = [];

    public Task AddAsync(Doctor doctor, CancellationToken cancellationToken)
    {
        _doctors.Add(doctor);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<Doctor>> GetAllAsync(CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyList<Doctor>>(_doctors.ToList());

    public Task<Doctor?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        Task.FromResult(_doctors.FirstOrDefault(d => d.Id == id));

    public Task<Doctor?> GetByUserIdAsync(string userId, CancellationToken cancellationToken) =>
        Task.FromResult(_doctors.FirstOrDefault(d => d.UserId == userId));

    public Task<IReadOnlyList<Doctor>> GetBySpecialtyAsync(int specialtyId, CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyList<Doctor>>(_doctors.Where(d => d.SpecialtyId == specialtyId).ToList());

    public void Update(Doctor doctor)
    {
    }

    public void Seed(Doctor doctor) => _doctors.Add(doctor);
}
