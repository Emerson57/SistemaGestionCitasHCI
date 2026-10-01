using MedicalAppointments.Application.Abstractions.Persistence;
using MedicalAppointments.Application.Abstractions.Time;
using MedicalAppointments.Application.Common.Mappings;
using MedicalAppointments.Application.Common.Results;
using MedicalAppointments.Application.Common.Validation;
using MedicalAppointments.Application.DTOs.Doctors;
using MedicalAppointments.Domain.Entities;
using MedicalAppointments.Domain.Exceptions;

namespace MedicalAppointments.Application.Features.Doctors;

public sealed class DoctorService(
    IDoctorRepository doctorRepository,
    ISpecialtyRepository specialtyRepository,
    IUnitOfWork unitOfWork,
    IDateTimeProvider dateTimeProvider) : IDoctorService
{
    public async Task<Result<IReadOnlyList<DoctorDto>>> GetAllAsync(CancellationToken cancellationToken)
    {
        var doctors = await doctorRepository.GetAllAsync(cancellationToken);
        var dtos = new List<DoctorDto>();

        foreach (var doctor in doctors)
        {
            var specialtyName = await GetSpecialtyNameAsync(doctor.SpecialtyId, cancellationToken);
            dtos.Add(doctor.ToDto(specialtyName));
        }

        return Result<IReadOnlyList<DoctorDto>>.Success(dtos);
    }

    public async Task<Result<DoctorDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var doctor = await doctorRepository.GetByIdAsync(id, cancellationToken);
        if (doctor is null)
        {
            return Result<DoctorDto>.Failure(Error.NotFound("Doctor not found."));
        }

        var specialtyName = await GetSpecialtyNameAsync(doctor.SpecialtyId, cancellationToken);
        return Result<DoctorDto>.Success(doctor.ToDto(specialtyName));
    }

    public async Task<Result<IReadOnlyList<DoctorDto>>> GetBySpecialtyAsync(
        int specialtyId,
        CancellationToken cancellationToken)
    {
        var specialty = await specialtyRepository.GetByIdAsync(specialtyId, cancellationToken);
        if (specialty is null)
        {
            return Result<IReadOnlyList<DoctorDto>>.Failure(Error.NotFound("Specialty not found."));
        }

        var doctors = await doctorRepository.GetBySpecialtyAsync(specialtyId, cancellationToken);
        var dtos = doctors.Select(d => d.ToDto(specialty.Name)).ToList();
        return Result<IReadOnlyList<DoctorDto>>.Success(dtos);
    }

    public async Task<Result<DoctorDto>> CreateAsync(
        CreateDoctorRequest request,
        CancellationToken cancellationToken)
    {
        var validation = DoctorRequestValidator.ValidateCreate(request);
        if (validation.IsFailure)
        {
            return Result<DoctorDto>.Failure(validation.Error!);
        }

        var specialty = await specialtyRepository.GetByIdAsync(request.SpecialtyId, cancellationToken);
        if (specialty is null || !specialty.IsActive)
        {
            return Result<DoctorDto>.Failure(Error.NotFound("Specialty not found."));
        }

        Doctor doctor;
        try
        {
            doctor = Doctor.Create(
                Guid.NewGuid(),
                request.UserId,
                request.SpecialtyId,
                request.FullName,
                request.ProfessionalLicense,
                dateTimeProvider.UtcNow);
        }
        catch (DomainException ex)
        {
            return Result<DoctorDto>.Failure(Error.Validation(ex.Message));
        }

        await doctorRepository.AddAsync(doctor, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<DoctorDto>.Success(doctor.ToDto(specialty.Name));
    }

    public async Task<Result<DoctorDto>> UpdateAsync(
        Guid id,
        UpdateDoctorRequest request,
        CancellationToken cancellationToken)
    {
        var validation = DoctorRequestValidator.ValidateUpdate(request);
        if (validation.IsFailure)
        {
            return Result<DoctorDto>.Failure(validation.Error!);
        }

        var doctor = await doctorRepository.GetByIdAsync(id, cancellationToken);
        if (doctor is null)
        {
            return Result<DoctorDto>.Failure(Error.NotFound("Doctor not found."));
        }

        var specialty = await specialtyRepository.GetByIdAsync(request.SpecialtyId, cancellationToken);
        if (specialty is null || !specialty.IsActive)
        {
            return Result<DoctorDto>.Failure(Error.NotFound("Specialty not found."));
        }

        try
        {
            doctor.ChangeSpecialty(request.SpecialtyId, dateTimeProvider.UtcNow);
            doctorRepository.Update(doctor);
        }
        catch (DomainException ex)
        {
            return Result<DoctorDto>.Failure(Error.Validation(ex.Message));
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<DoctorDto>.Success(doctor.ToDto(specialty.Name));
    }

    public async Task<Result> DeactivateAsync(Guid id, CancellationToken cancellationToken)
    {
        var doctor = await doctorRepository.GetByIdAsync(id, cancellationToken);
        if (doctor is null)
        {
            return Result.Failure(Error.NotFound("Doctor not found."));
        }

        doctor.Deactivate(dateTimeProvider.UtcNow);
        doctorRepository.Update(doctor);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    private async Task<string?> GetSpecialtyNameAsync(int specialtyId, CancellationToken cancellationToken)
    {
        var specialty = await specialtyRepository.GetByIdAsync(specialtyId, cancellationToken);
        return specialty?.Name;
    }
}
