using MedicalAppointments.Domain.Enums;

namespace MedicalAppointments.Application.DTOs.Patients;

public sealed class UpdatePatientRequest
{
    public required string FullName { get; init; }

    public required DateOnly BirthDate { get; init; }

    public required string Address { get; init; }

    public required string PhoneNumber { get; init; }

    public Sex Sex { get; init; }

    public string? Disability { get; init; }

    public MaritalStatus MaritalStatus { get; init; }
}
