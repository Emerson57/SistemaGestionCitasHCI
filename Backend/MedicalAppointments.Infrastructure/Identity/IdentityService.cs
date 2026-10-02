using MedicalAppointments.Application.Abstractions.Identity;
using MedicalAppointments.Application.Abstractions.Time;
using MedicalAppointments.Application.Common.Results;
using MedicalAppointments.Application.Common.Validation;
using MedicalAppointments.Application.DTOs.Authentication;
using MedicalAppointments.Domain.Entities;
using MedicalAppointments.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MedicalAppointments.Infrastructure.Identity;

public sealed class IdentityService(
    UserManager<ApplicationUser> userManager,
    ApplicationDbContext dbContext,
    IDateTimeProvider dateTimeProvider,
    ITokenService tokenService) : IIdentityService
{
    public async Task<Result<AuthenticationResponse>> RegisterPatientAsync(
        RegisterPatientRequest request,
        CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(dateTimeProvider.UtcNow.UtcDateTime);
        var validation = RegisterPatientRequestValidator.Validate(request, today);
        if (validation.IsFailure)
        {
            return Result<AuthenticationResponse>.Failure(validation.Error!);
        }

        var normalizedEmail = request.Email.Trim();

        if (await userManager.FindByEmailAsync(normalizedEmail) is not null)
        {
            return Result<AuthenticationResponse>.Failure(
                Error.Conflict("A user with this email already exists."));
        }

        var user = new ApplicationUser
        {
            UserName = normalizedEmail,
            Email = normalizedEmail,
            FullName = request.FullName.Trim(),
            IsActive = true,
            CreatedAt = dateTimeProvider.UtcNow
        };

        var strategy = dbContext.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

            var identityResult = await userManager.CreateAsync(user, request.Password);
            if (!identityResult.Succeeded)
            {
                return Result<AuthenticationResponse>.Failure(
                    MapIdentityErrors(identityResult));
            }

            var roleResult = await userManager.AddToRoleAsync(user, AppRoles.Patient);
            if (!roleResult.Succeeded)
            {
                await userManager.DeleteAsync(user);
                return Result<AuthenticationResponse>.Failure(MapIdentityErrors(roleResult));
            }

            try
            {
                var patient = Patient.Create(
                    Guid.NewGuid(),
                    user.Id,
                    request.FullName,
                    request.BirthDate,
                    request.Address,
                    request.PhoneNumber,
                    request.Sex,
                    request.Disability,
                    request.MaritalStatus,
                    dateTimeProvider.UtcNow);

                await dbContext.Patients.AddAsync(patient, cancellationToken);
                await dbContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                await userManager.DeleteAsync(user);
                throw;
            }

            return Result<AuthenticationResponse>.Success(
                BuildAuthenticationResponse(user, normalizedEmail, AppRoles.Patient));
        });
    }

    public async Task<Result<AuthenticationResponse>> AuthenticateAsync(
        LoginRequest request,
        CancellationToken cancellationToken)
    {
        var validation = LoginRequestValidator.Validate(request);
        if (validation.IsFailure)
        {
            return Result<AuthenticationResponse>.Failure(validation.Error!);
        }

        var user = await userManager.FindByEmailAsync(request.Email.Trim());
        if (user is null || !user.IsActive)
        {
            return Result<AuthenticationResponse>.Failure(
                Error.Unauthorized("Invalid email or password."));
        }

        if (!await userManager.CheckPasswordAsync(user, request.Password))
        {
            return Result<AuthenticationResponse>.Failure(
                Error.Unauthorized("Invalid email or password."));
        }

        var roles = await userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? AppRoles.Patient;

        return Result<AuthenticationResponse>.Success(
            BuildAuthenticationResponse(user, user.Email ?? request.Email.Trim(), role));
    }

    public async Task<Result<string>> CreateDoctorUserAsync(
        string email,
        string password,
        string fullName,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return Result<string>.Failure(Error.Validation("Email is required."));
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            return Result<string>.Failure(Error.Validation("Password is required."));
        }

        if (string.IsNullOrWhiteSpace(fullName))
        {
            return Result<string>.Failure(Error.Validation("Full name is required."));
        }

        var normalizedEmail = email.Trim();

        if (await userManager.FindByEmailAsync(normalizedEmail) is not null)
        {
            return Result<string>.Failure(Error.Conflict("A user with this email already exists."));
        }

        var user = new ApplicationUser
        {
            UserName = normalizedEmail,
            Email = normalizedEmail,
            FullName = fullName.Trim(),
            IsActive = true,
            CreatedAt = dateTimeProvider.UtcNow
        };

        var createResult = await userManager.CreateAsync(user, password);
        if (!createResult.Succeeded)
        {
            return Result<string>.Failure(MapIdentityErrors(createResult));
        }

        var roleResult = await userManager.AddToRoleAsync(user, AppRoles.Doctor);
        if (!roleResult.Succeeded)
        {
            await userManager.DeleteAsync(user);
            return Result<string>.Failure(MapIdentityErrors(roleResult));
        }

        return Result<string>.Success(user.Id);
    }

    public async Task<string?> GetUserRoleAsync(string userId, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return null;
        }

        var roles = await userManager.GetRolesAsync(user);
        return roles.FirstOrDefault();
    }

    public async Task<bool> IsInRoleAsync(string userId, string role, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return false;
        }

        return await userManager.IsInRoleAsync(user, role);
    }

    private AuthenticationResponse BuildAuthenticationResponse(
        ApplicationUser user,
        string email,
        string role)
    {
        var token = tokenService.CreateAccessToken(user.Id, email, user.FullName, role);
        return new AuthenticationResponse
        {
            UserId = user.Id,
            FullName = user.FullName,
            Email = email,
            Role = role,
            AccessToken = token.AccessToken,
            ExpiresAt = token.ExpiresAt
        };
    }

    private static Error MapIdentityErrors(IdentityResult identityResult)
    {
        var message = string.Join(" ", identityResult.Errors.Select(e => e.Description));
        return Error.Validation(string.IsNullOrWhiteSpace(message) ? "Identity operation failed." : message);
    }
}
