using MedicalAppointments.Application.DTOs.Patients;
using MedicalAppointments.Domain.Entities;

namespace MedicalAppointments.Application.Common.Mappings;

public static class PatientMappings
{
    public static PatientDto ToDto(this Patient patient) =>
        new()
        {
            Id = patient.Id,
            FullName = patient.FullName,
            BirthDate = patient.BirthDate,
            Address = patient.Address,
            PhoneNumber = patient.PhoneNumber,
            Sex = patient.Sex,
            Disability = patient.Disability,
            MaritalStatus = patient.MaritalStatus,
            IsActive = patient.IsActive
        };
}
