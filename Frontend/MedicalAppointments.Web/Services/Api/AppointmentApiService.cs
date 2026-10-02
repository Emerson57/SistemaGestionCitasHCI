using MedicalAppointments.Web.Models.Appointments;
using MedicalAppointments.Web.Services.Authentication;

namespace MedicalAppointments.Web.Services.Api;

public interface IAppointmentApiService
{
    Task<AppointmentDto> ScheduleAsync(ScheduleAppointmentRequest request, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<AppointmentDto>> GetMyAppointmentsAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<AppointmentDto>> GetMyHistoryAsync(CancellationToken cancellationToken = default);

    Task<AppointmentDto> RescheduleAsync(Guid id, RescheduleAppointmentRequest request, CancellationToken cancellationToken = default);

    Task CancelAsync(Guid id, CancellationToken cancellationToken = default);

    Task<AppointmentDto> ConfirmAsync(Guid id, CancellationToken cancellationToken = default);

    Task<AppointmentDto> CompleteAsync(Guid id, CancellationToken cancellationToken = default);
}

public sealed class AppointmentApiService(HttpClient httpClient, ISessionExpiredHandler sessionExpiredHandler)
    : ApiClientBase(httpClient, sessionExpiredHandler), IAppointmentApiService
{
    public Task<AppointmentDto> ScheduleAsync(ScheduleAppointmentRequest request, CancellationToken cancellationToken = default) =>
        PostAsync<AppointmentDto>("api/appointments", request, cancellationToken)!;

    public Task<IReadOnlyList<AppointmentDto>> GetMyAppointmentsAsync(CancellationToken cancellationToken = default) =>
        GetAsync<IReadOnlyList<AppointmentDto>>("api/appointments/my", cancellationToken)!;

    public Task<IReadOnlyList<AppointmentDto>> GetMyHistoryAsync(CancellationToken cancellationToken = default) =>
        GetAsync<IReadOnlyList<AppointmentDto>>("api/appointments/history", cancellationToken)!;

    public Task<AppointmentDto> RescheduleAsync(Guid id, RescheduleAppointmentRequest request, CancellationToken cancellationToken = default) =>
        PutAsync<AppointmentDto>($"api/appointments/{id}/reschedule", request, cancellationToken)!;

    public Task CancelAsync(Guid id, CancellationToken cancellationToken = default) =>
        DeleteAsync($"api/appointments/{id}", cancellationToken);

    public Task<AppointmentDto> ConfirmAsync(Guid id, CancellationToken cancellationToken = default) =>
        PutAsync<AppointmentDto>($"api/appointments/{id}/confirm", null, cancellationToken)!;

    public Task<AppointmentDto> CompleteAsync(Guid id, CancellationToken cancellationToken = default) =>
        PutAsync<AppointmentDto>($"api/appointments/{id}/complete", null, cancellationToken)!;
}
