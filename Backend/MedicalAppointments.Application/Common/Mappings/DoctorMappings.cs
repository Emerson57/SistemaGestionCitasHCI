using MedicalAppointments.Application.DTOs.Doctors;
using MedicalAppointments.Domain.Entities;

namespace MedicalAppointments.Application.Common.Mappings;

public static class DoctorMappings
{
    public static DoctorDto ToDto(this Doctor doctor, string? specialtyName = null) =>
        new()
        {
            Id = doctor.Id,
            FullName = doctor.FullName,
            ProfessionalLicense = doctor.ProfessionalLicense,
            SpecialtyId = doctor.SpecialtyId,
            SpecialtyName = specialtyName ?? doctor.Specialty?.Name,
            IsActive = doctor.IsActive
        };
}
