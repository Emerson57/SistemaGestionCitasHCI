using MedicalAppointments.Api.Extensions;
using MedicalAppointments.Application.DTOs.Doctors;
using MedicalAppointments.Application.Features.Appointments;
using MedicalAppointments.Application.Features.Doctors;
using MedicalAppointments.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedicalAppointments.Api.Controllers;

[ApiController]
[Route("api/doctors")]
[Authorize]
public sealed class DoctorsController(
    IDoctorService doctorService,
    IAppointmentService appointmentService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<DoctorDto>>> GetAll(
        [FromQuery] int? specialtyId,
        CancellationToken cancellationToken)
    {
        if (specialtyId.HasValue)
        {
            return (await doctorService.GetBySpecialtyAsync(specialtyId.Value, cancellationToken))
                .ToActionResult(this);
        }

        return (await doctorService.GetAllAsync(cancellationToken)).ToActionResult(this);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<DoctorDto>> GetById(Guid id, CancellationToken cancellationToken) =>
        (await doctorService.GetByIdAsync(id, cancellationToken)).ToActionResult(this);

    [HttpGet("me/agenda")]
    [Authorize(Roles = AppRoles.Doctor)]
    public async Task<ActionResult<IReadOnlyList<Application.DTOs.Appointments.AppointmentDto>>> GetMyAgenda(
        CancellationToken cancellationToken) =>
        (await appointmentService.GetDoctorAgendaAsync(cancellationToken)).ToActionResult(this);

    [HttpPost]
    [Authorize(Roles = AppRoles.Administrator)]
    public async Task<ActionResult<DoctorDto>> Create(
        [FromBody] CreateDoctorRequest request,
        CancellationToken cancellationToken) =>
        (await doctorService.CreateAsync(request, cancellationToken))
            .ToCreatedActionResult(this, dto => $"/api/doctors/{dto.Id}");

    [HttpPut("{id:guid}")]
    [Authorize(Roles = AppRoles.Administrator)]
    public async Task<ActionResult<DoctorDto>> Update(
        Guid id,
        [FromBody] UpdateDoctorRequest request,
        CancellationToken cancellationToken) =>
        (await doctorService.UpdateAsync(id, request, cancellationToken)).ToActionResult(this);

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = AppRoles.Administrator)]
    public async Task<IActionResult> Deactivate(Guid id, CancellationToken cancellationToken) =>
        (await doctorService.DeactivateAsync(id, cancellationToken)).ToActionResult(this);
}
