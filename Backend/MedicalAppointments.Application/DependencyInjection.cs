using MedicalAppointments.Application.Features.Appointments;
using MedicalAppointments.Application.Features.Availability;
using MedicalAppointments.Application.Features.Doctors;
using MedicalAppointments.Application.Features.Patients;
using MedicalAppointments.Application.Features.Specialties;
using Microsoft.Extensions.DependencyInjection;

namespace MedicalAppointments.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IPatientService, PatientService>();
        services.AddScoped<ISpecialtyService, SpecialtyService>();
        services.AddScoped<IDoctorService, DoctorService>();
        services.AddScoped<IAvailabilityService, AvailabilityService>();
        services.AddScoped<IAppointmentService, AppointmentService>();

        return services;
    }
}
