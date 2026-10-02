namespace MedicalAppointments.Web.Models.Specialties;

public sealed class SpecialtyDto
{
    public int Id { get; init; }

    public required string Name { get; init; }

    public string? Description { get; init; }

    public bool IsActive { get; init; }
}

public sealed class CreateSpecialtyRequest
{
    public required string Name { get; init; }

    public string? Description { get; init; }
}

public sealed class UpdateSpecialtyRequest
{
    public required string Name { get; init; }

    public string? Description { get; init; }
}
