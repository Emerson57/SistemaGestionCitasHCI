using MedicalAppointments.Application.DTOs.Specialties;
using MedicalAppointments.Domain.Entities;

namespace MedicalAppointments.Application.Common.Mappings;

public static class SpecialtyMappings
{
    public static SpecialtyDto ToDto(this Specialty specialty) =>
        new()
        {
            Id = specialty.Id,
            Name = specialty.Name,
            Description = specialty.Description,
            IsActive = specialty.IsActive
        };
}
