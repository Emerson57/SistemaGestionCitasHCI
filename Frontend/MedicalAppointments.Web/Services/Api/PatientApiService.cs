using MedicalAppointments.Web.Models.Patients;
using MedicalAppointments.Web.Services.Authentication;

namespace MedicalAppointments.Web.Services.Api;

public interface IPatientApiService
{
    Task<PatientDto> GetMeAsync(CancellationToken cancellationToken = default);

    Task<PatientDto> UpdateMeAsync(UpdatePatientRequest request, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PatientDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<PatientDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task DeactivateAsync(Guid id, CancellationToken cancellationToken = default);
}

public sealed class PatientApiService(HttpClient httpClient, ISessionExpiredHandler sessionExpiredHandler)
    : ApiClientBase(httpClient, sessionExpiredHandler), IPatientApiService
{
    public Task<PatientDto> GetMeAsync(CancellationToken cancellationToken = default) =>
        GetAsync<PatientDto>("api/patients/me", cancellationToken)!;

    public Task<PatientDto> UpdateMeAsync(UpdatePatientRequest request, CancellationToken cancellationToken = default) =>
        PutAsync<PatientDto>("api/patients/me", request, cancellationToken)!;

    public Task<IReadOnlyList<PatientDto>> GetAllAsync(CancellationToken cancellationToken = default) =>
        GetAsync<IReadOnlyList<PatientDto>>("api/patients", cancellationToken)!;

    public Task<PatientDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        GetAsync<PatientDto>($"api/patients/{id}", cancellationToken)!;

    public Task DeactivateAsync(Guid id, CancellationToken cancellationToken = default) =>
        DeleteAsync($"api/patients/{id}", cancellationToken);
}
