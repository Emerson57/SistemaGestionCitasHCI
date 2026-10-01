using MedicalAppointments.Application.DTOs.Availability;
using MedicalAppointments.Domain.Entities;

namespace MedicalAppointments.Application.Common.Mappings;

public static class DoctorAvailabilityMappings
{
    public static DoctorAvailabilityDto ToDto(this DoctorAvailability availability) =>
        new()
        {
            Id = availability.Id,
            DoctorId = availability.DoctorId,
            Date = availability.Date,
            StartTime = availability.StartTime,
            EndTime = availability.EndTime,
            Status = availability.Status
        };
}
