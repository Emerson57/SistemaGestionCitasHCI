namespace MedicalAppointments.Application.DTOs.Doctors;

public sealed class UpdateDoctorRequest
{
    public required string FullName { get; init; }

    public required string ProfessionalLicense { get; init; }

    public int SpecialtyId { get; init; }
}
