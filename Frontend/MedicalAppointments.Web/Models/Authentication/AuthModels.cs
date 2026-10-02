namespace MedicalAppointments.Web.Models.Authentication;

public sealed class LoginRequest
{
    public required string Email { get; init; }

    public required string Password { get; init; }
}

public sealed class RegisterPatientRequest
{
    public required string FullName { get; init; }

    public required string Email { get; init; }

    public required string Password { get; init; }

    public required DateOnly BirthDate { get; init; }

    public required string Address { get; init; }

    public required string PhoneNumber { get; init; }

    public Models.Enums.Sex Sex { get; init; }

    public string? Disability { get; init; }

    public Models.Enums.MaritalStatus MaritalStatus { get; init; }
}

public sealed class AuthenticationResponse
{
    public required string UserId { get; init; }

    public required string FullName { get; init; }

    public required string Email { get; init; }

    public required string Role { get; init; }

    public string? AccessToken { get; init; }

    public DateTimeOffset? ExpiresAt { get; init; }
}

public sealed class StoredAuthSession
{
    public required AuthenticationResponse User { get; init; }

    public required string AccessToken { get; init; }
}
