using MedicalAppointments.Web.Models.Enums;

namespace MedicalAppointments.Web.Models.Patients;

public sealed class PatientDto
{
    public Guid Id { get; init; }

    public required string FullName { get; init; }

    public DateOnly BirthDate { get; init; }

    public required string Address { get; init; }

    public required string PhoneNumber { get; init; }

    public Sex Sex { get; init; }

    public string? Disability { get; init; }

    public MaritalStatus MaritalStatus { get; init; }

    public bool IsActive { get; init; }
}

public sealed class UpdatePatientRequest
{
    public string FullName { get; set; } = string.Empty;

    public DateOnly BirthDate { get; set; }

    public string Address { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public Sex Sex { get; set; }

    public string? Disability { get; set; }

    public MaritalStatus MaritalStatus { get; set; }
}
