namespace MedicalAppointments.Web.Auth;

public static class AppClaimTypes
{
    public const string UserId = "sub";

    public const string Email = "email";

    public const string Name = "name";

    public const string Role = "role";
}

public static class AppRoles
{
    public const string Patient = "Patient";

    public const string Doctor = "Doctor";

    public const string Administrator = "Administrator";
}
