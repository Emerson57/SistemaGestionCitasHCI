using MedicalAppointments.Web.Models.Availability;
using MedicalAppointments.Web.Services.Authentication;

namespace MedicalAppointments.Web.Services.Api;

public interface IAvailabilityApiService
{
    Task<IReadOnlyList<DoctorAvailabilityDto>> GetByDoctorAsync(Guid doctorId, CancellationToken cancellationToken = default);

    Task<DoctorAvailabilityDto> CreateForCurrentDoctorAsync(
        CreateMyDoctorAvailabilityRequest request,
        CancellationToken cancellationToken = default);
}

public sealed class AvailabilityApiService(
    HttpClient httpClient,
    ISessionExpiredHandler sessionExpiredHandler,
    IOutgoingApiAuthContext outgoingApiAuthContext)
    : ApiClientBase(httpClient, sessionExpiredHandler, outgoingApiAuthContext), IAvailabilityApiService
{
    public Task<IReadOnlyList<DoctorAvailabilityDto>> GetByDoctorAsync(Guid doctorId, CancellationToken cancellationToken = default) =>
        GetAsync<IReadOnlyList<DoctorAvailabilityDto>>($"api/doctors/{doctorId}/availability", cancellationToken)!;

    public Task<DoctorAvailabilityDto> CreateForCurrentDoctorAsync(
        CreateMyDoctorAvailabilityRequest request,
        CancellationToken cancellationToken = default) =>
        PostAsync<DoctorAvailabilityDto>("api/doctors/me/availability", request, cancellationToken)!;
}
