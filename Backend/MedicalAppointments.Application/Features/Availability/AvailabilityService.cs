using MedicalAppointments.Application.Abstractions.Persistence;
using MedicalAppointments.Application.Abstractions.Time;
using MedicalAppointments.Application.Common.Mappings;
using MedicalAppointments.Application.Common.Results;
using MedicalAppointments.Application.DTOs.Availability;
using MedicalAppointments.Domain.Entities;
using MedicalAppointments.Domain.Exceptions;

namespace MedicalAppointments.Application.Features.Availability;

public sealed class AvailabilityService(
    IDoctorRepository doctorRepository,
    IDoctorAvailabilityRepository availabilityRepository,
    IUnitOfWork unitOfWork,
    IDateTimeProvider dateTimeProvider) : IAvailabilityService
{
    public async Task<Result<IReadOnlyList<DoctorAvailabilityDto>>> GetAvailableByDoctorAsync(
        Guid doctorId,
        CancellationToken cancellationToken)
    {
        var doctor = await doctorRepository.GetByIdAsync(doctorId, cancellationToken);
        if (doctor is null)
        {
            return Result<IReadOnlyList<DoctorAvailabilityDto>>.Failure(Error.NotFound("Doctor not found."));
        }

        var slots = await availabilityRepository.GetAvailableByDoctorAsync(doctorId, cancellationToken);
        return Result<IReadOnlyList<DoctorAvailabilityDto>>.Success(
            slots.Select(s => s.ToDto()).ToList());
    }

    public async Task<Result<DoctorAvailabilityDto>> CreateAsync(
        CreateDoctorAvailabilityRequest request,
        CancellationToken cancellationToken)
    {
        if (request.DoctorId == Guid.Empty)
        {
            return Result<DoctorAvailabilityDto>.Failure(Error.Validation("Doctor id is required."));
        }

        var doctor = await doctorRepository.GetByIdAsync(request.DoctorId, cancellationToken);
        if (doctor is null)
        {
            return Result<DoctorAvailabilityDto>.Failure(Error.NotFound("Doctor not found."));
        }

        if (!doctor.IsActive)
        {
            return Result<DoctorAvailabilityDto>.Failure(Error.Validation("Doctor is not active."));
        }

        if (await availabilityRepository.HasOverlappingSlotAsync(
                request.DoctorId,
                request.Date,
                request.StartTime,
                request.EndTime,
                excludeAvailabilityId: null,
                cancellationToken))
        {
            return Result<DoctorAvailabilityDto>.Failure(
                Error.Conflict("An overlapping availability slot already exists for this doctor."));
        }

        DoctorAvailability availability;
        try
        {
            availability = DoctorAvailability.Create(
                Guid.NewGuid(),
                request.DoctorId,
                request.Date,
                request.StartTime,
                request.EndTime,
                dateTimeProvider.UtcNow);
        }
        catch (DomainException ex)
        {
            return Result<DoctorAvailabilityDto>.Failure(Error.Validation(ex.Message));
        }

        await availabilityRepository.AddAsync(availability, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<DoctorAvailabilityDto>.Success(availability.ToDto());
    }

    public async Task<Result> MarkUnavailableAsync(Guid availabilityId, CancellationToken cancellationToken)
    {
        var availability = await availabilityRepository.GetByIdAsync(availabilityId, cancellationToken);
        if (availability is null)
        {
            return Result.Failure(Error.NotFound("Availability slot not found."));
        }

        try
        {
            availability.MarkUnavailable(dateTimeProvider.UtcNow);
        }
        catch (DomainException ex)
        {
            return Result.Failure(Error.Validation(ex.Message));
        }

        availabilityRepository.Update(availability);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
