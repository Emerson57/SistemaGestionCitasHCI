using MedicalAppointments.Api.Services;
using MedicalAppointments.Application.Abstractions.Identity;

namespace MedicalAppointments.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        return services;
    }
}
