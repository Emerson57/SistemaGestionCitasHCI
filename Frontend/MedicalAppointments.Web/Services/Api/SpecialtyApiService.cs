using MedicalAppointments.Web.Models.Specialties;
using MedicalAppointments.Web.Services.Authentication;

namespace MedicalAppointments.Web.Services.Api;

public interface ISpecialtyApiService
{
    Task<IReadOnlyList<SpecialtyDto>> GetActiveAsync(CancellationToken cancellationToken = default);

    Task<SpecialtyDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<SpecialtyDto> CreateAsync(CreateSpecialtyRequest request, CancellationToken cancellationToken = default);

    Task<SpecialtyDto> UpdateAsync(int id, UpdateSpecialtyRequest request, CancellationToken cancellationToken = default);

    Task DeactivateAsync(int id, CancellationToken cancellationToken = default);
}

public sealed class SpecialtyApiService(HttpClient httpClient, ISessionExpiredHandler sessionExpiredHandler)
    : ApiClientBase(httpClient, sessionExpiredHandler), ISpecialtyApiService
{
    public Task<IReadOnlyList<SpecialtyDto>> GetActiveAsync(CancellationToken cancellationToken = default) =>
        GetAsync<IReadOnlyList<SpecialtyDto>>("api/specialties", cancellationToken)!;

    public Task<SpecialtyDto> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        GetAsync<SpecialtyDto>($"api/specialties/{id}", cancellationToken)!;

    public Task<SpecialtyDto> CreateAsync(CreateSpecialtyRequest request, CancellationToken cancellationToken = default) =>
        PostAsync<SpecialtyDto>("api/specialties", request, cancellationToken)!;

    public Task<SpecialtyDto> UpdateAsync(int id, UpdateSpecialtyRequest request, CancellationToken cancellationToken = default) =>
        PutAsync<SpecialtyDto>($"api/specialties/{id}", request, cancellationToken)!;

    public Task DeactivateAsync(int id, CancellationToken cancellationToken = default) =>
        DeleteAsync($"api/specialties/{id}", cancellationToken);
}
