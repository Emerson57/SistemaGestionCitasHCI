using MedicalAppointments.Api.Extensions;
using MedicalAppointments.Application.DTOs.Appointments;
using MedicalAppointments.Application.Features.Appointments;
using MedicalAppointments.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedicalAppointments.Api.Controllers;

[ApiController]
[Route("api/appointments")]
[Authorize]
public sealed class AppointmentsController(IAppointmentService appointmentService) : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = AppRoles.Patient)]
    public async Task<ActionResult<AppointmentDto>> Schedule(
        [FromBody] ScheduleAppointmentRequest request,
        CancellationToken cancellationToken) =>
        (await appointmentService.ScheduleAsync(request, cancellationToken))
            .ToCreatedActionResult(this, dto => $"/api/appointments/{dto.Id}");

    [HttpGet("my")]
    [Authorize(Roles = AppRoles.Patient)]
    public async Task<ActionResult<IReadOnlyList<AppointmentDto>>> GetMyAppointments(
        CancellationToken cancellationToken) =>
        (await appointmentService.GetMyAppointmentsAsync(cancellationToken)).ToActionResult(this);

    [HttpGet("history")]
    [Authorize(Roles = AppRoles.Patient)]
    public async Task<ActionResult<IReadOnlyList<AppointmentDto>>> GetMyHistory(
        CancellationToken cancellationToken) =>
        (await appointmentService.GetMyHistoryAsync(cancellationToken)).ToActionResult(this);

    [HttpPut("{id:guid}/reschedule")]
    [Authorize(Roles = AppRoles.Patient)]
    public async Task<ActionResult<AppointmentDto>> Reschedule(
        Guid id,
        [FromBody] RescheduleAppointmentRequest request,
        CancellationToken cancellationToken) =>
        (await appointmentService.RescheduleAsync(id, request, cancellationToken)).ToActionResult(this);

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = AppRoles.Patient)]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken cancellationToken) =>
        (await appointmentService.CancelAsync(id, cancellationToken)).ToActionResult(this);

    [HttpPut("{id:guid}/confirm")]
    [Authorize(Roles = AppRoles.Doctor)]
    public async Task<ActionResult<AppointmentDto>> Confirm(Guid id, CancellationToken cancellationToken) =>
        (await appointmentService.ConfirmAsync(id, cancellationToken)).ToActionResult(this);

    [HttpPut("{id:guid}/complete")]
    [Authorize(Roles = AppRoles.Doctor)]
    public async Task<ActionResult<AppointmentDto>> Complete(Guid id, CancellationToken cancellationToken) =>
        (await appointmentService.CompleteAsync(id, cancellationToken)).ToActionResult(this);
}
