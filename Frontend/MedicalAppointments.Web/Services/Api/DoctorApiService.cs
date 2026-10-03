using MedicalAppointments.Web.Models.Appointments;
using MedicalAppointments.Web.Models.Doctors;
using MedicalAppointments.Web.Services.Authentication;

namespace MedicalAppointments.Web.Services.Api;

public interface IDoctorApiService
{
    Task<IReadOnlyList<DoctorDto>> GetAllAsync(int? specialtyId = null, CancellationToken cancellationToken = default);

    Task<DoctorDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<AppointmentDto>> GetMyAgendaAsync(CancellationToken cancellationToken = default);

    Task<DoctorDto> CreateAsync(CreateDoctorRequest request, CancellationToken cancellationToken = default);

    Task<DoctorDto> UpdateAsync(Guid id, UpdateDoctorRequest request, CancellationToken cancellationToken = default);

    Task DeactivateAsync(Guid id, CancellationToken cancellationToken = default);
}

public sealed class DoctorApiService(
    HttpClient httpClient,
    ISessionExpiredHandler sessionExpiredHandler,
    IOutgoingApiAuthContext outgoingApiAuthContext)
    : ApiClientBase(httpClient, sessionExpiredHandler, outgoingApiAuthContext), IDoctorApiService
{
    public Task<IReadOnlyList<DoctorDto>> GetAllAsync(int? specialtyId = null, CancellationToken cancellationToken = default)
    {
        var url = specialtyId.HasValue ? $"api/doctors?specialtyId={specialtyId.Value}" : "api/doctors";
        return GetAsync<IReadOnlyList<DoctorDto>>(url, cancellationToken)!;
    }

    public Task<DoctorDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        GetAsync<DoctorDto>($"api/doctors/{id}", cancellationToken)!;

    public Task<IReadOnlyList<AppointmentDto>> GetMyAgendaAsync(CancellationToken cancellationToken = default) =>
        GetAsync<IReadOnlyList<AppointmentDto>>("api/doctors/me/agenda", cancellationToken)!;

    public Task<DoctorDto> CreateAsync(CreateDoctorRequest request, CancellationToken cancellationToken = default) =>
        PostAsync<DoctorDto>("api/doctors", request, cancellationToken)!;

    public Task<DoctorDto> UpdateAsync(Guid id, UpdateDoctorRequest request, CancellationToken cancellationToken = default) =>
        PutAsync<DoctorDto>($"api/doctors/{id}", request, cancellationToken)!;

    public Task DeactivateAsync(Guid id, CancellationToken cancellationToken = default) =>
        DeleteAsync($"api/doctors/{id}", cancellationToken);
}
