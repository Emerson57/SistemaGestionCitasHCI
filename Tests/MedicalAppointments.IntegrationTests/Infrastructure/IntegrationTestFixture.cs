using MedicalAppointments.Infrastructure.Identity;
using MedicalAppointments.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MedicalAppointments.IntegrationTests.Infrastructure;

public sealed class IntegrationTestFixture : IAsyncLifetime
{
    private const string SigningKey = "IntegrationTestSigningKeyMustBeAtLeast32CharactersLong!";

    private readonly string _databaseName = $"MedicalAppointmentsIntegrationTests_{Guid.NewGuid():N}";

    private readonly Dictionary<string, string?> _previousEnvironment = new();

    public WebApplicationFactory<Program> Factory { get; private set; } = null!;

    public string AdminEmail { get; } = "admin.integration@test.local";

    public string AdminPassword { get; } = "AdminTestPassword123!";

    public async Task InitializeAsync()
    {
        SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "IntegrationTesting");
        SetEnvironmentVariable(
            "ConnectionStrings__DefaultConnection",
            $"Server=(localdb)\\MSSQLLocalDB;Database={_databaseName};Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True");
        SetEnvironmentVariable("Jwt__Issuer", "IntegrationTests");
        SetEnvironmentVariable("Jwt__Audience", "IntegrationTests");
        SetEnvironmentVariable("Jwt__SigningKey", SigningKey);
        SetEnvironmentVariable("Jwt__ExpirationMinutes", "60");

        Factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("IntegrationTesting");
        });

        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await dbContext.Database.MigrateAsync();
        await IdentitySeeder.SeedRolesAsync(scope.ServiceProvider);

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        if (await userManager.FindByEmailAsync(AdminEmail) is null)
        {
            var admin = new ApplicationUser
            {
                UserName = AdminEmail,
                Email = AdminEmail,
                FullName = "Integration Admin",
                IsActive = true,
                CreatedAt = DateTimeOffset.UtcNow
            };

            await userManager.CreateAsync(admin, AdminPassword);
            await userManager.AddToRoleAsync(admin, AppRoles.Administrator);
        }
    }

    public async Task DisposeAsync()
    {
        RestoreEnvironmentVariables();

        if (Factory is null)
        {
            return;
        }

        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await dbContext.Database.EnsureDeletedAsync();
        Factory.Dispose();
    }

    private void SetEnvironmentVariable(string name, string value)
    {
        _previousEnvironment.TryAdd(name, Environment.GetEnvironmentVariable(name));
        Environment.SetEnvironmentVariable(name, value);
    }

    private void RestoreEnvironmentVariables()
    {
        foreach (var (name, previous) in _previousEnvironment)
        {
            Environment.SetEnvironmentVariable(name, previous);
        }

        _previousEnvironment.Clear();
    }
}
