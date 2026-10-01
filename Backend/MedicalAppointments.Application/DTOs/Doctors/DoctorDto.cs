namespace MedicalAppointments.Application.DTOs.Doctors;

public sealed class DoctorDto
{
    public Guid Id { get; init; }

    public required string FullName { get; init; }

    public required string ProfessionalLicense { get; init; }

    public int SpecialtyId { get; init; }

    public string? SpecialtyName { get; init; }

    public bool IsActive { get; init; }
}
