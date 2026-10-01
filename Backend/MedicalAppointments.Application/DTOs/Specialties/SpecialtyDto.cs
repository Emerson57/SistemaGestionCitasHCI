namespace MedicalAppointments.Application.DTOs.Specialties;

public sealed class SpecialtyDto
{
    public int Id { get; init; }

    public required string Name { get; init; }

    public string? Description { get; init; }

    public bool IsActive { get; init; }
}
