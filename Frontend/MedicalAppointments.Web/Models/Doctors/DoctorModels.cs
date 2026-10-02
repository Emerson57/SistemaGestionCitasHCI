namespace MedicalAppointments.Web.Models.Doctors;

public sealed class DoctorDto
{
    public Guid Id { get; init; }

    public required string FullName { get; init; }

    public required string ProfessionalLicense { get; init; }

    public int SpecialtyId { get; init; }

    public string? SpecialtyName { get; init; }

    public bool IsActive { get; init; }
}

public sealed class CreateDoctorRequest
{
    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string InitialPassword { get; set; } = string.Empty;

    public string ProfessionalLicense { get; set; } = string.Empty;

    public int SpecialtyId { get; set; }
}

public sealed class UpdateDoctorRequest
{
    public required string FullName { get; init; }

    public required string ProfessionalLicense { get; init; }

    public int SpecialtyId { get; init; }
}
