using MedicalAppointments.Application.Abstractions.Persistence;
using MedicalAppointments.Domain.Entities;

namespace MedicalAppointments.UnitTests.Application.Fakes;

public sealed class InMemoryPatientRepository : IPatientRepository
{
    private readonly List<Patient> _patients = [];

    public Task AddAsync(Patient patient, CancellationToken cancellationToken)
    {
        _patients.Add(patient);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<Patient>> GetAllAsync(CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyList<Patient>>(_patients.ToList());

    public Task<Patient?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        Task.FromResult(_patients.FirstOrDefault(p => p.Id == id));

    public Task<Patient?> GetByUserIdAsync(string userId, CancellationToken cancellationToken) =>
        Task.FromResult(_patients.FirstOrDefault(p => p.UserId == userId));

    public void Update(Patient patient)
    {
    }

    public void Seed(Patient patient) => _patients.Add(patient);
}
