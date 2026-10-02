using MedicalAppointments.Web.Services.Api;
using MedicalAppointments.Web.Services.Authentication;
using MedicalAppointments.Web.Services.State;
using Microsoft.AspNetCore.Components.Authorization;

namespace MedicalAppointments.Web.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWebApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        var apiBaseUri = configuration.GetApiBaseUri();

        services.AddScoped<JwtAuthenticationStateProvider>();
        services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<JwtAuthenticationStateProvider>());
        services.AddAuthorizationCore();
        services.AddCascadingAuthenticationState();

        services.AddScoped<ITokenStorageService, ProtectedSessionTokenStorage>();
        services.AddScoped<ISessionExpiredHandler, SessionExpiredHandler>();
        services.AddScoped<NotificationService>();
        services.AddTransient<BearerTokenHandler>();

        services.AddHttpClient("ApiAnonymous", client => client.BaseAddress = apiBaseUri);
        services.AddHttpClient("ApiAuthorized", client => client.BaseAddress = apiBaseUri)
            .AddHttpMessageHandler<BearerTokenHandler>();

        services.AddScoped<IAuthApiService>(sp =>
        {
            var factory = sp.GetRequiredService<IHttpClientFactory>();
            return new AuthApiService(factory.CreateClient("ApiAnonymous"));
        });

        RegisterAuthorizedApiService<IPatientApiService, PatientApiService>(services);
        RegisterAuthorizedApiService<ISpecialtyApiService, SpecialtyApiService>(services);
        RegisterAuthorizedApiService<IDoctorApiService, DoctorApiService>(services);
        RegisterAuthorizedApiService<IAppointmentApiService, AppointmentApiService>(services);
        RegisterAuthorizedApiService<IAvailabilityApiService, AvailabilityApiService>(services);

        return services;
    }

    private static void RegisterAuthorizedApiService<TInterface, TImplementation>(IServiceCollection services)
        where TInterface : class
        where TImplementation : class, TInterface
    {
        services.AddScoped<TInterface>(sp =>
        {
            var factory = sp.GetRequiredService<IHttpClientFactory>();
            var httpClient = factory.CreateClient("ApiAuthorized");
            var sessionHandler = sp.GetRequiredService<ISessionExpiredHandler>();
            return ActivatorUtilities.CreateInstance<TImplementation>(sp, httpClient, sessionHandler);
        });
    }
}
