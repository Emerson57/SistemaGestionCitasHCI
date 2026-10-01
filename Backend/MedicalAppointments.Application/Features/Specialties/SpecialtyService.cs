using MedicalAppointments.Application.Abstractions.Persistence;
using MedicalAppointments.Application.Abstractions.Time;
using MedicalAppointments.Application.Common.Mappings;
using MedicalAppointments.Application.Common.Results;
using MedicalAppointments.Application.Common.Validation;
using MedicalAppointments.Application.DTOs.Specialties;
using MedicalAppointments.Domain.Entities;
using MedicalAppointments.Domain.Exceptions;

namespace MedicalAppointments.Application.Features.Specialties;

public sealed class SpecialtyService(
    ISpecialtyRepository specialtyRepository,
    IUnitOfWork unitOfWork,
    IDateTimeProvider dateTimeProvider) : ISpecialtyService
{
    public async Task<Result<IReadOnlyList<SpecialtyDto>>> GetActiveAsync(CancellationToken cancellationToken)
    {
        var specialties = await specialtyRepository.GetActiveAsync(cancellationToken);
        return Result<IReadOnlyList<SpecialtyDto>>.Success(
            specialties.Select(s => s.ToDto()).ToList());
    }

    public async Task<Result<IReadOnlyList<SpecialtyDto>>> GetAllAsync(CancellationToken cancellationToken)
    {
        var specialties = await specialtyRepository.GetAllAsync(cancellationToken);
        return Result<IReadOnlyList<SpecialtyDto>>.Success(
            specialties.Select(s => s.ToDto()).ToList());
    }

    public async Task<Result<SpecialtyDto>> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var specialty = await specialtyRepository.GetByIdAsync(id, cancellationToken);
        if (specialty is null)
        {
            return Result<SpecialtyDto>.Failure(Error.NotFound("Specialty not found."));
        }

        return Result<SpecialtyDto>.Success(specialty.ToDto());
    }

    public async Task<Result<SpecialtyDto>> CreateAsync(
        CreateSpecialtyRequest request,
        CancellationToken cancellationToken)
    {
        var validation = SpecialtyRequestValidator.ValidateCreate(request);
        if (validation.IsFailure)
        {
            return Result<SpecialtyDto>.Failure(validation.Error!);
        }

        var normalizedName = request.Name.Trim();
        if (await specialtyRepository.ExistsByNameAsync(normalizedName, cancellationToken))
        {
            return Result<SpecialtyDto>.Failure(Error.Conflict("A specialty with the same name already exists."));
        }

        var nextId = await GetNextSpecialtyIdAsync(cancellationToken);

        Specialty specialty;
        try
        {
            specialty = Specialty.Create(
                nextId,
                normalizedName,
                request.Description,
                dateTimeProvider.UtcNow);
        }
        catch (DomainException ex)
        {
            return Result<SpecialtyDto>.Failure(Error.Validation(ex.Message));
        }

        await specialtyRepository.AddAsync(specialty, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<SpecialtyDto>.Success(specialty.ToDto());
    }

    public async Task<Result<SpecialtyDto>> UpdateAsync(
        int id,
        UpdateSpecialtyRequest request,
        CancellationToken cancellationToken)
    {
        var validation = SpecialtyRequestValidator.ValidateUpdate(request);
        if (validation.IsFailure)
        {
            return Result<SpecialtyDto>.Failure(validation.Error!);
        }

        var specialty = await specialtyRepository.GetByIdAsync(id, cancellationToken);
        if (specialty is null)
        {
            return Result<SpecialtyDto>.Failure(Error.NotFound("Specialty not found."));
        }

        var normalizedName = request.Name.Trim();
        if (!string.Equals(specialty.Name, normalizedName, StringComparison.OrdinalIgnoreCase)
            && await specialtyRepository.ExistsByNameAsync(normalizedName, cancellationToken))
        {
            return Result<SpecialtyDto>.Failure(Error.Conflict("A specialty with the same name already exists."));
        }

        try
        {
            specialty.Update(normalizedName, request.Description, dateTimeProvider.UtcNow);
        }
        catch (DomainException ex)
        {
            return Result<SpecialtyDto>.Failure(Error.Validation(ex.Message));
        }

        specialtyRepository.Update(specialty);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<SpecialtyDto>.Success(specialty.ToDto());
    }

    public async Task<Result> DeactivateAsync(int id, CancellationToken cancellationToken)
    {
        var specialty = await specialtyRepository.GetByIdAsync(id, cancellationToken);
        if (specialty is null)
        {
            return Result.Failure(Error.NotFound("Specialty not found."));
        }

        specialty.Deactivate(dateTimeProvider.UtcNow);
        specialtyRepository.Update(specialty);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    private async Task<int> GetNextSpecialtyIdAsync(CancellationToken cancellationToken)
    {
        var all = await specialtyRepository.GetAllAsync(cancellationToken);
        return all.Count == 0 ? 1 : all.Max(s => s.Id) + 1;
    }
}
