namespace MedicalAppointments.Application.DTOs.Doctors;

public sealed class CreateDoctorRequest
{
    public required string UserId { get; init; }

    public required string FullName { get; init; }

    public required string ProfessionalLicense { get; init; }

    public int SpecialtyId { get; init; }
}
