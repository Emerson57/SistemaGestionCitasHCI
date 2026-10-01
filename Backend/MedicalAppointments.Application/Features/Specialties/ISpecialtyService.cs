using MedicalAppointments.Application.Common.Results;
using MedicalAppointments.Application.DTOs.Specialties;

namespace MedicalAppointments.Application.Features.Specialties;

public interface ISpecialtyService
{
    Task<Result<IReadOnlyList<SpecialtyDto>>> GetActiveAsync(CancellationToken cancellationToken);

    Task<Result<IReadOnlyList<SpecialtyDto>>> GetAllAsync(CancellationToken cancellationToken);

    Task<Result<SpecialtyDto>> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<Result<SpecialtyDto>> CreateAsync(CreateSpecialtyRequest request, CancellationToken cancellationToken);

    Task<Result<SpecialtyDto>> UpdateAsync(
        int id,
        UpdateSpecialtyRequest request,
        CancellationToken cancellationToken);

    Task<Result> DeactivateAsync(int id, CancellationToken cancellationToken);
}
