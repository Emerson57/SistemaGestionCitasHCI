using MedicalAppointments.Api.Extensions;
using MedicalAppointments.Application.DTOs.Specialties;
using MedicalAppointments.Application.Features.Specialties;
using MedicalAppointments.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedicalAppointments.Api.Controllers;

[ApiController]
[Route("api/specialties")]
[Authorize]
public sealed class SpecialtiesController(ISpecialtyService specialtyService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<SpecialtyDto>>> GetActive(CancellationToken cancellationToken) =>
        (await specialtyService.GetActiveAsync(cancellationToken)).ToActionResult(this);

    [HttpGet("{id:int}")]
    public async Task<ActionResult<SpecialtyDto>> GetById(int id, CancellationToken cancellationToken) =>
        (await specialtyService.GetByIdAsync(id, cancellationToken)).ToActionResult(this);

    [HttpPost]
    [Authorize(Roles = AppRoles.Administrator)]
    public async Task<ActionResult<SpecialtyDto>> Create(
        [FromBody] CreateSpecialtyRequest request,
        CancellationToken cancellationToken) =>
        (await specialtyService.CreateAsync(request, cancellationToken))
            .ToCreatedActionResult(this, dto => $"/api/specialties/{dto.Id}");

    [HttpPut("{id:int}")]
    [Authorize(Roles = AppRoles.Administrator)]
    public async Task<ActionResult<SpecialtyDto>> Update(
        int id,
        [FromBody] UpdateSpecialtyRequest request,
        CancellationToken cancellationToken) =>
        (await specialtyService.UpdateAsync(id, request, cancellationToken)).ToActionResult(this);

    [HttpDelete("{id:int}")]
    [Authorize(Roles = AppRoles.Administrator)]
    public async Task<IActionResult> Deactivate(int id, CancellationToken cancellationToken) =>
        (await specialtyService.DeactivateAsync(id, cancellationToken)).ToActionResult(this);
}
