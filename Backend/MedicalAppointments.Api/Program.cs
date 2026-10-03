using MedicalAppointments.Api.Extensions;
using MedicalAppointments.Api.Middleware;
using MedicalAppointments.Application;
using MedicalAppointments.Infrastructure;
using MedicalAppointments.Infrastructure.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApiServices();
builder.Services.AddApiAuthentication(builder.Configuration);

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        var origins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
            ?? ["https://localhost:7171"];

        policy.WithOrigins(origins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

if (!app.Environment.IsEnvironment("IntegrationTesting"))
{
    using var scope = app.Services.CreateScope();
    await IdentitySeeder.SeedRolesAsync(scope.ServiceProvider);
    await DevelopmentDataSeeder.SeedAsync(
        scope.ServiceProvider,
        app.Configuration,
        app.Environment);
}

app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseCors("Frontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.Run();

public partial class Program;
