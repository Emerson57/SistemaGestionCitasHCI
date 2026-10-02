using MedicalAppointments.Application.Abstractions.Identity;
using MedicalAppointments.Application.Common.Results;
using MedicalAppointments.Application.DTOs.Authentication;

namespace MedicalAppointments.UnitTests.Application.Fakes;

public sealed class FakeIdentityService : IIdentityService
{
    public Task<Result<AuthenticationResponse>> RegisterPatientAsync(
        RegisterPatientRequest request,
        CancellationToken cancellationToken) =>
        throw new NotImplementedException();

    public Task<Result<AuthenticationResponse>> AuthenticateAsync(
        LoginRequest request,
        CancellationToken cancellationToken) =>
        throw new NotImplementedException();

    public Task<Result<string>> CreateDoctorUserAsync(
        string email,
        string password,
        string fullName,
        CancellationToken cancellationToken) =>
        Task.FromResult(Result<string>.Success("doctor-user-id"));

    public Task<string?> GetUserRoleAsync(string userId, CancellationToken cancellationToken) =>
        Task.FromResult<string?>("Doctor");

    public Task<bool> IsInRoleAsync(string userId, string role, CancellationToken cancellationToken) =>
        Task.FromResult(true);
}
