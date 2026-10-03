using MedicalAppointments.Web.Services.Api;
using MedicalAppointments.Web.Services.Authentication;
using MedicalAppointments.Web.Services.State;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;

namespace MedicalAppointments.Web.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWebApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        var apiBaseUri = configuration.GetApiBaseUri();

        services.AddMemoryCache();
        services.AddSingleton<ISignInTicketStore, SignInTicketStore>();
        services.AddHttpContextAccessor();
        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/login";
                options.AccessDeniedPath = "/forbidden";
                options.Cookie.Name = "MedicalAppointments.Auth";
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.Lax;
            });
        services.AddAuthorization();
        services.AddScoped<JwtAuthenticationStateProvider>();
        services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<JwtAuthenticationStateProvider>());
        services.AddAuthorizationCore();
        services.AddCascadingAuthenticationState();

        services.AddDataProtection();
        services.AddScoped<ProtectedSessionTokenStorage>();
        services.AddScoped<IApiSessionCookieStore, ApiSessionCookieStore>();
        services.AddScoped<ITokenStorageService>(sp => new CompositeTokenStorageService(
            sp.GetRequiredService<ProtectedSessionTokenStorage>(),
            sp.GetRequiredService<IApiSessionCookieStore>()));
        services.AddScoped<IOutgoingApiAuthContext, OutgoingApiAuthContext>();
        services.AddScoped<IAuthSessionReadiness, AuthSessionReadiness>();
        services.AddScoped<IAuthSessionSyncService, AuthSessionSyncService>();
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
