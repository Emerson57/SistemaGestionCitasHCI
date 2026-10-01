using MedicalAppointments.Application.Common.Results;
using MedicalAppointments.Application.DTOs.Doctors;

namespace MedicalAppointments.Application.Features.Doctors;

public interface IDoctorService
{
    Task<Result<IReadOnlyList<DoctorDto>>> GetAllAsync(CancellationToken cancellationToken);

    Task<Result<DoctorDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<Result<IReadOnlyList<DoctorDto>>> GetBySpecialtyAsync(int specialtyId, CancellationToken cancellationToken);

    Task<Result<DoctorDto>> CreateAsync(CreateDoctorRequest request, CancellationToken cancellationToken);

    Task<Result<DoctorDto>> UpdateAsync(Guid id, UpdateDoctorRequest request, CancellationToken cancellationToken);

    Task<Result> DeactivateAsync(Guid id, CancellationToken cancellationToken);
}
