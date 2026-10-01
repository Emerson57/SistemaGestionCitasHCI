namespace MedicalAppointments.Application.DTOs.Specialties;

public sealed class UpdateSpecialtyRequest
{
    public required string Name { get; init; }

    public string? Description { get; init; }
}
