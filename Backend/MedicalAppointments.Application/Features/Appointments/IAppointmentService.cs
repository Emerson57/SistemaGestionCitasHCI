using MedicalAppointments.Application.Common.Results;
using MedicalAppointments.Application.DTOs.Appointments;

namespace MedicalAppointments.Application.Features.Appointments;

public interface IAppointmentService
{
    Task<Result<AppointmentDto>> ScheduleAsync(
        ScheduleAppointmentRequest request,
        CancellationToken cancellationToken);

    Task<Result<IReadOnlyList<AppointmentDto>>> GetMyAppointmentsAsync(CancellationToken cancellationToken);

    Task<Result<IReadOnlyList<AppointmentDto>>> GetMyHistoryAsync(CancellationToken cancellationToken);

    Task<Result<IReadOnlyList<AppointmentDto>>> GetDoctorAgendaAsync(CancellationToken cancellationToken);

    Task<Result<AppointmentDto>> RescheduleAsync(
        Guid appointmentId,
        RescheduleAppointmentRequest request,
        CancellationToken cancellationToken);

    Task<Result> CancelAsync(Guid appointmentId, CancellationToken cancellationToken);

    Task<Result<AppointmentDto>> ConfirmAsync(Guid appointmentId, CancellationToken cancellationToken);

    Task<Result<AppointmentDto>> CompleteAsync(Guid appointmentId, CancellationToken cancellationToken);
}
