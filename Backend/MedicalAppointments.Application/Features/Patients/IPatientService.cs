using MedicalAppointments.Application.Common.Results;
using MedicalAppointments.Application.DTOs.Patients;

namespace MedicalAppointments.Application.Features.Patients;

public interface IPatientService
{
    Task<Result<PatientDto>> GetCurrentPatientAsync(CancellationToken cancellationToken);

    Task<Result<PatientDto>> GetPatientByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<Result<IReadOnlyList<PatientDto>>> GetAllPatientsAsync(CancellationToken cancellationToken);

    Task<Result<PatientDto>> UpdateCurrentPatientAsync(
        UpdatePatientRequest request,
        CancellationToken cancellationToken);

    Task<Result> DeactivatePatientAsync(Guid id, CancellationToken cancellationToken);
}
