using MedicalAppointments.Domain.Exceptions;

namespace MedicalAppointments.Domain.Entities;

public class Doctor
{
    private readonly List<DoctorAvailability> _availabilities = [];
    private readonly List<Appointment> _appointments = [];

    private Doctor()
    {
        UserId = string.Empty;
        FullName = string.Empty;
        ProfessionalLicense = string.Empty;
    }

    private Doctor(
        Guid id,
        string userId,
        int specialtyId,
        string fullName,
        string professionalLicense,
        DateTimeOffset createdAt)
    {
        Id = id;
        UserId = userId;
        SpecialtyId = specialtyId;
        FullName = fullName;
        ProfessionalLicense = professionalLicense;
        IsActive = true;
        CreatedAt = createdAt;
    }

    public Guid Id { get; private set; }

    public string UserId { get; private set; }

    public int SpecialtyId { get; private set; }

    public string FullName { get; private set; }

    public string ProfessionalLicense { get; private set; }

    public bool IsActive { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset? UpdatedAt { get; private set; }

    public Specialty? Specialty { get; private set; }

    public IReadOnlyCollection<DoctorAvailability> Availabilities => _availabilities.AsReadOnly();

    public IReadOnlyCollection<Appointment> Appointments => _appointments.AsReadOnly();

    public static Doctor Create(
        Guid id,
        string userId,
        int specialtyId,
        string fullName,
        string professionalLicense,
        DateTimeOffset createdAt)
    {
        if (id == Guid.Empty)
        {
            throw new DomainException("Doctor id cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new DomainException("User id cannot be empty.");
        }

        if (specialtyId <= 0)
        {
            throw new DomainException("Specialty id must be greater than zero.");
        }

        if (string.IsNullOrWhiteSpace(fullName))
        {
            throw new DomainException("Full name cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(professionalLicense))
        {
            throw new DomainException("Professional license cannot be empty.");
        }

        return new Doctor(
            id,
            userId.Trim(),
            specialtyId,
            fullName.Trim(),
            professionalLicense.Trim(),
            createdAt);
    }

    public void UpdateProfile(
        string fullName,
        string professionalLicense,
        int specialtyId,
        DateTimeOffset updatedAt)
    {
        if (string.IsNullOrWhiteSpace(fullName))
        {
            throw new DomainException("Full name cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(professionalLicense))
        {
            throw new DomainException("Professional license cannot be empty.");
        }

        FullName = fullName.Trim();
        ProfessionalLicense = professionalLicense.Trim();
        ChangeSpecialty(specialtyId, updatedAt);
    }

    public void ChangeSpecialty(int specialtyId, DateTimeOffset updatedAt)
    {
        if (specialtyId <= 0)
        {
            throw new DomainException("Specialty id must be greater than zero.");
        }

        SpecialtyId = specialtyId;
        Specialty = null;
        UpdatedAt = updatedAt;
    }

    public void Deactivate(DateTimeOffset updatedAt)
    {
        IsActive = false;
        UpdatedAt = updatedAt;
    }

    internal void AssignSpecialty(Specialty specialty)
    {
        Specialty = specialty;
        SpecialtyId = specialty.Id;
    }

    internal void RegisterAvailability(DoctorAvailability availability)
    {
        _availabilities.Add(availability);
    }

    internal void RegisterAppointment(Appointment appointment)
    {
        _appointments.Add(appointment);
    }
}
