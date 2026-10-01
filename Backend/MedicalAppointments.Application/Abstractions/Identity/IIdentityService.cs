using MedicalAppointments.Application.Common.Results;
using MedicalAppointments.Application.DTOs.Authentication;

namespace MedicalAppointments.Application.Abstractions.Identity;

public interface IIdentityService
{
    Task<Result<AuthenticationResponse>> RegisterPatientAsync(
        RegisterPatientRequest request,
        CancellationToken cancellationToken);

    Task<Result<AuthenticationResponse>> AuthenticateAsync(
        LoginRequest request,
        CancellationToken cancellationToken);

    Task<Result<string>> CreateDoctorUserAsync(
        string email,
        string password,
        string fullName,
        CancellationToken cancellationToken);

    Task<string?> GetUserRoleAsync(string userId, CancellationToken cancellationToken);

    Task<bool> IsInRoleAsync(string userId, string role, CancellationToken cancellationToken);
}
