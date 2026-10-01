using MedicalAppointments.Domain.Exceptions;

namespace MedicalAppointments.Domain.Entities;

public class Specialty
{
    private readonly List<Doctor> _doctors = [];

    private Specialty()
    {
        Name = string.Empty;
    }

    private Specialty(int id, string name, string? description, DateTimeOffset createdAt)
    {
        Id = id;
        Name = name;
        Description = description;
        IsActive = true;
        CreatedAt = createdAt;
    }

    public int Id { get; private set; }

    public string Name { get; private set; }

    public string? Description { get; private set; }

    public bool IsActive { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset? UpdatedAt { get; private set; }

    public IReadOnlyCollection<Doctor> Doctors => _doctors.AsReadOnly();

    public static Specialty Create(int id, string name, string? description, DateTimeOffset createdAt)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Specialty name cannot be empty.");
        }

        return new Specialty(id, name.Trim(), description?.Trim(), createdAt);
    }

    public void Update(string name, string? description, DateTimeOffset updatedAt)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Specialty name cannot be empty.");
        }

        Name = name.Trim();
        Description = description?.Trim();
        UpdatedAt = updatedAt;
    }

    public void Deactivate(DateTimeOffset updatedAt)
    {
        IsActive = false;
        UpdatedAt = updatedAt;
    }

    internal void RegisterDoctor(Doctor doctor)
    {
        _doctors.Add(doctor);
    }
}
