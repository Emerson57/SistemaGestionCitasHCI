using MedicalAppointments.Application.DTOs.Appointments;
using MedicalAppointments.Domain.Entities;

namespace MedicalAppointments.Application.Common.Mappings;

public static class AppointmentMappings
{
    public static AppointmentDto ToDto(
        this Appointment appointment,
        string patientName,
        string doctorName,
        int specialtyId,
        string? specialtyName) =>
        new()
        {
            Id = appointment.Id,
            PatientId = appointment.PatientId,
            PatientName = patientName,
            DoctorId = appointment.DoctorId,
            DoctorName = doctorName,
            SpecialtyId = specialtyId,
            SpecialtyName = specialtyName,
            AppointmentDateTime = appointment.AppointmentDateTime,
            Status = appointment.Status,
            Reason = appointment.Reason,
            CreatedAt = appointment.CreatedAt
        };
}
