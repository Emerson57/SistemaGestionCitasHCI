using MedicalAppointments.Api.Extensions;
using MedicalAppointments.Application.DTOs.Availability;
using MedicalAppointments.Application.Features.Availability;
using MedicalAppointments.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedicalAppointments.Api.Controllers;

[ApiController]
[Route("api/doctors")]
[Authorize]
public sealed class AvailabilityController(IAvailabilityService availabilityService) : ControllerBase
{
    [HttpGet("{doctorId:guid}/availability")]
    public async Task<ActionResult<IReadOnlyList<DoctorAvailabilityDto>>> GetByDoctor(
        Guid doctorId,
        CancellationToken cancellationToken) =>
        (await availabilityService.GetAvailableByDoctorAsync(doctorId, cancellationToken)).ToActionResult(this);

    [HttpPost("me/availability")]
    [Authorize(Roles = AppRoles.Doctor)]
    public async Task<ActionResult<DoctorAvailabilityDto>> CreateForCurrentDoctor(
        [FromBody] CreateMyDoctorAvailabilityRequest request,
        CancellationToken cancellationToken) =>
        (await availabilityService.CreateForCurrentDoctorAsync(request, cancellationToken))
            .ToCreatedActionResult(this, dto => $"/api/doctors/{dto.DoctorId}/availability");
}
