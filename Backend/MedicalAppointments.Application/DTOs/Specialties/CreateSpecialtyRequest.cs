namespace MedicalAppointments.Application.DTOs.Specialties;

public sealed class CreateSpecialtyRequest
{
    public required string Name { get; init; }

    public string? Description { get; init; }
}
