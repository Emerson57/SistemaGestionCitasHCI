using MedicalAppointments.Api.Extensions;
using MedicalAppointments.Application.Abstractions.Identity;
using MedicalAppointments.Application.DTOs.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace MedicalAppointments.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(IIdentityService identityService) : ControllerBase
{
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthenticationResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult<AuthenticationResponse>> Register(
        [FromBody] RegisterPatientRequest request,
        CancellationToken cancellationToken)
    {
        var result = await identityService.RegisterPatientAsync(request, cancellationToken);
        if (result.IsFailure)
        {
            return (ActionResult<AuthenticationResponse>)result.ToActionResult<AuthenticationResponse>(this);
        }

        return Created("/api/patients/me", result.Value);
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthenticationResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<AuthenticationResponse>> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        var result = await identityService.AuthenticateAsync(request, cancellationToken);
        return result.ToActionResult(this);
    }
}
