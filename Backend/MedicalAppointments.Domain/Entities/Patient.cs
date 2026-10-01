using MedicalAppointments.Domain.Enums;
using MedicalAppointments.Domain.Exceptions;

namespace MedicalAppointments.Domain.Entities;

public class Patient
{
    private readonly List<Appointment> _appointments = [];

    private Patient()
    {
        UserId = string.Empty;
        FullName = string.Empty;
        Address = string.Empty;
        PhoneNumber = string.Empty;
    }

    private Patient(
        Guid id,
        string userId,
        string fullName,
        DateOnly birthDate,
        string address,
        string phoneNumber,
        Sex sex,
        string? disability,
        MaritalStatus maritalStatus,
        DateTimeOffset createdAt)
    {
        Id = id;
        UserId = userId;
        FullName = fullName;
        BirthDate = birthDate;
        Address = address;
        PhoneNumber = phoneNumber;
        Sex = sex;
        Disability = disability;
        MaritalStatus = maritalStatus;
        IsActive = true;
        CreatedAt = createdAt;
    }

    public Guid Id { get; private set; }

    public string UserId { get; private set; }

    public string FullName { get; private set; }

    public DateOnly BirthDate { get; private set; }

    public string Address { get; private set; }

    public string PhoneNumber { get; private set; }

    public Sex Sex { get; private set; }

    public string? Disability { get; private set; }

    public MaritalStatus MaritalStatus { get; private set; }

    public bool IsActive { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset? UpdatedAt { get; private set; }

    public IReadOnlyCollection<Appointment> Appointments => _appointments.AsReadOnly();

    public static Patient Create(
        Guid id,
        string userId,
        string fullName,
        DateOnly birthDate,
        string address,
        string phoneNumber,
        Sex sex,
        string? disability,
        MaritalStatus maritalStatus,
        DateTimeOffset createdAt)
    {
        if (id == Guid.Empty)
        {
            throw new DomainException("Patient id cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new DomainException("User id cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(fullName))
        {
            throw new DomainException("Full name cannot be empty.");
        }

        return new Patient(
            id,
            userId.Trim(),
            fullName.Trim(),
            birthDate,
            address.Trim(),
            phoneNumber.Trim(),
            sex,
            disability?.Trim(),
            maritalStatus,
            createdAt);
    }

    public void UpdateProfile(
        string fullName,
        DateOnly birthDate,
        string address,
        string phoneNumber,
        Sex sex,
        string? disability,
        MaritalStatus maritalStatus,
        DateTimeOffset updatedAt)
    {
        if (string.IsNullOrWhiteSpace(fullName))
        {
            throw new DomainException("Full name cannot be empty.");
        }

        FullName = fullName.Trim();
        BirthDate = birthDate;
        Address = address.Trim();
        PhoneNumber = phoneNumber.Trim();
        Sex = sex;
        Disability = disability?.Trim();
        MaritalStatus = maritalStatus;
        UpdatedAt = updatedAt;
    }

    public void Deactivate(DateTimeOffset updatedAt)
    {
        IsActive = false;
        UpdatedAt = updatedAt;
    }

    internal void RegisterAppointment(Appointment appointment)
    {
        _appointments.Add(appointment);
    }
}
