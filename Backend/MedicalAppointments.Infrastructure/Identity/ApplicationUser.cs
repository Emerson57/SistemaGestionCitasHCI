using Microsoft.AspNetCore.Identity;

namespace MedicalAppointments.Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    public required string FullName { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? UpdatedAt { get; set; }
}
