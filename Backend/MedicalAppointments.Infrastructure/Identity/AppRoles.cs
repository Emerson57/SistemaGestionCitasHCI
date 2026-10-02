namespace MedicalAppointments.Infrastructure.Identity;

public static class AppRoles
{
    public const string Patient = "Patient";

    public const string Doctor = "Doctor";

    public const string Administrator = "Administrator";

    public static IReadOnlyList<string> All { get; } =
    [
        Patient,
        Doctor,
        Administrator
    ];
}
