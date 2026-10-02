using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MedicalAppointments.Infrastructure.Identity;

public static class IdentityDevAdminSeeder
{
    public static async Task SeedDevelopmentAdminAsync(
        IServiceProvider serviceProvider,
        IConfiguration configuration)
    {
        var email = configuration["DevAdmin:Email"];
        var password = configuration["DevAdmin:Password"];
        var fullName = configuration["DevAdmin:FullName"] ?? "Development Administrator";

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            return;
        }

        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        if (await userManager.FindByEmailAsync(email) is not null)
        {
            return;
        }

        var user = new ApplicationUser
        {
            UserName = email.Trim(),
            Email = email.Trim(),
            FullName = fullName,
            IsActive = true,
            CreatedAt = DateTimeOffset.UtcNow
        };

        var createResult = await userManager.CreateAsync(user, password);
        if (!createResult.Succeeded)
        {
            return;
        }

        await userManager.AddToRoleAsync(user, AppRoles.Administrator);
    }
}
