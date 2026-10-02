using MedicalAppointments.Application.Common.Results;
using MedicalAppointments.Application.DTOs.Availability;

namespace MedicalAppointments.Application.Features.Availability;

public interface IAvailabilityService
{
    Task<Result<IReadOnlyList<DoctorAvailabilityDto>>> GetAvailableByDoctorAsync(
        Guid doctorId,
        CancellationToken cancellationToken);

    Task<Result<DoctorAvailabilityDto>> CreateAsync(
        CreateDoctorAvailabilityRequest request,
        CancellationToken cancellationToken);

    Task<Result> MarkUnavailableAsync(Guid availabilityId, CancellationToken cancellationToken);

    Task<Result<DoctorAvailabilityDto>> CreateForCurrentDoctorAsync(
        CreateMyDoctorAvailabilityRequest request,
        CancellationToken cancellationToken);
}
