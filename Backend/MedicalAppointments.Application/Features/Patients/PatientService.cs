using MedicalAppointments.Application.Abstractions.Identity;
using MedicalAppointments.Application.Abstractions.Persistence;
using MedicalAppointments.Application.Abstractions.Time;
using MedicalAppointments.Application.Common.Mappings;
using MedicalAppointments.Application.Common.Results;
using MedicalAppointments.Application.DTOs.Patients;
using MedicalAppointments.Domain.Exceptions;

namespace MedicalAppointments.Application.Features.Patients;

public sealed class PatientService(
    IPatientRepository patientRepository,
    ICurrentUserService currentUserService,
    IUnitOfWork unitOfWork,
    IDateTimeProvider dateTimeProvider) : IPatientService
{
    private const string PatientRole = "Patient";
    private const string AdministratorRole = "Administrator";

    public async Task<Result<PatientDto>> GetCurrentPatientAsync(CancellationToken cancellationToken)
    {
        var patientResult = await ResolveCurrentPatientAsync(cancellationToken);
        if (patientResult.IsFailure)
        {
            return Result<PatientDto>.Failure(patientResult.Error!);
        }

        return Result<PatientDto>.Success(patientResult.Value!.ToDto());
    }

    public async Task<Result<PatientDto>> GetPatientByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var access = await EnsureCanAccessPatientAsync(id, cancellationToken);
        if (access.IsFailure)
        {
            return Result<PatientDto>.Failure(access.Error!);
        }

        var patient = await patientRepository.GetByIdAsync(id, cancellationToken);
        if (patient is null)
        {
            return Result<PatientDto>.Failure(Error.NotFound("Patient not found."));
        }

        return Result<PatientDto>.Success(patient.ToDto());
    }

    public async Task<Result<IReadOnlyList<PatientDto>>> GetAllPatientsAsync(CancellationToken cancellationToken)
    {
        if (!IsAdministrator())
        {
            return Result<IReadOnlyList<PatientDto>>.Failure(
                Error.Forbidden("Only administrators can list all patients."));
        }

        var patients = await patientRepository.GetAllAsync(cancellationToken);
        var dtos = patients.Select(p => p.ToDto()).ToList();
        return Result<IReadOnlyList<PatientDto>>.Success(dtos);
    }

    public async Task<Result<PatientDto>> UpdateCurrentPatientAsync(
        UpdatePatientRequest request,
        CancellationToken cancellationToken)
    {
        var patientResult = await ResolveCurrentPatientAsync(cancellationToken);
        if (patientResult.IsFailure)
        {
            return Result<PatientDto>.Failure(patientResult.Error!);
        }

        var patient = patientResult.Value!;

        try
        {
            patient.UpdateProfile(
                request.FullName,
                request.BirthDate,
                request.Address,
                request.PhoneNumber,
                request.Sex,
                request.Disability,
                request.MaritalStatus,
                dateTimeProvider.UtcNow);
        }
        catch (DomainException ex)
        {
            return Result<PatientDto>.Failure(Error.Validation(ex.Message));
        }

        patientRepository.Update(patient);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<PatientDto>.Success(patient.ToDto());
    }

    public async Task<Result> DeactivatePatientAsync(Guid id, CancellationToken cancellationToken)
    {
        var access = await EnsureCanAccessPatientAsync(id, cancellationToken);
        if (access.IsFailure)
        {
            return Result.Failure(access.Error!);
        }

        var patient = await patientRepository.GetByIdAsync(id, cancellationToken);
        if (patient is null)
        {
            return Result.Failure(Error.NotFound("Patient not found."));
        }

        patient.Deactivate(dateTimeProvider.UtcNow);
        patientRepository.Update(patient);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    private async Task<Result<Domain.Entities.Patient>> ResolveCurrentPatientAsync(
        CancellationToken cancellationToken)
    {
        if (!currentUserService.IsAuthenticated || string.IsNullOrWhiteSpace(currentUserService.UserId))
        {
            return Result<Domain.Entities.Patient>.Failure(Error.Unauthorized("User is not authenticated."));
        }

        var patient = await patientRepository.GetByUserIdAsync(currentUserService.UserId, cancellationToken);
        if (patient is null)
        {
            return Result<Domain.Entities.Patient>.Failure(Error.NotFound("Patient profile not found."));
        }

        return Result<Domain.Entities.Patient>.Success(patient);
    }

    private async Task<Result> EnsureCanAccessPatientAsync(Guid patientId, CancellationToken cancellationToken)
    {
        if (!currentUserService.IsAuthenticated || string.IsNullOrWhiteSpace(currentUserService.UserId))
        {
            return Result.Failure(Error.Unauthorized("User is not authenticated."));
        }

        if (IsAdministrator())
        {
            return Result.Success();
        }

        if (string.Equals(currentUserService.Role, PatientRole, StringComparison.OrdinalIgnoreCase))
        {
            var currentPatient = await patientRepository.GetByUserIdAsync(
                currentUserService.UserId,
                cancellationToken);

            if (currentPatient is null)
            {
                return Result.Failure(Error.NotFound("Patient profile not found."));
            }

            if (currentPatient.Id != patientId)
            {
                return Result.Failure(Error.Forbidden("Patients can access only their own profile."));
            }

            return Result.Success();
        }

        return Result.Failure(Error.Forbidden("Access denied."));
    }

    private bool IsAdministrator() =>
        string.Equals(currentUserService.Role, AdministratorRole, StringComparison.OrdinalIgnoreCase);
}
