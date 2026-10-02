using MedicalAppointments.Api.Extensions;
using MedicalAppointments.Application.DTOs.Patients;
using MedicalAppointments.Application.Features.Patients;
using MedicalAppointments.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedicalAppointments.Api.Controllers;

[ApiController]
[Route("api/patients")]
[Authorize]
public sealed class PatientsController(IPatientService patientService) : ControllerBase
{
    [HttpGet("me")]
    public async Task<ActionResult<PatientDto>> GetCurrent(CancellationToken cancellationToken) =>
        (await patientService.GetCurrentPatientAsync(cancellationToken)).ToActionResult(this);

    [HttpPut("me")]
    public async Task<ActionResult<PatientDto>> UpdateCurrent(
        [FromBody] UpdatePatientRequest request,
        CancellationToken cancellationToken) =>
        (await patientService.UpdateCurrentPatientAsync(request, cancellationToken)).ToActionResult(this);

    [HttpGet]
    [Authorize(Roles = AppRoles.Administrator)]
    public async Task<ActionResult<IReadOnlyList<PatientDto>>> GetAll(CancellationToken cancellationToken) =>
        (await patientService.GetAllPatientsAsync(cancellationToken)).ToActionResult(this);

    [HttpGet("{id:guid}")]
    [Authorize(Roles = AppRoles.Administrator)]
    public async Task<ActionResult<PatientDto>> GetById(Guid id, CancellationToken cancellationToken) =>
        (await patientService.GetPatientByIdAsync(id, cancellationToken)).ToActionResult(this);

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = AppRoles.Administrator)]
    public async Task<IActionResult> Deactivate(Guid id, CancellationToken cancellationToken) =>
        (await patientService.DeactivatePatientAsync(id, cancellationToken)).ToActionResult(this);
}
