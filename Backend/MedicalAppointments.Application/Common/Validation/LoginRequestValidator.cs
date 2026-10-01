using MedicalAppointments.Application.Common.Results;
using MedicalAppointments.Application.DTOs.Authentication;

namespace MedicalAppointments.Application.Common.Validation;

public static class LoginRequestValidator
{
    public static Result Validate(LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
        {
            return Result.Failure(Error.Validation("Email is required."));
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            return Result.Failure(Error.Validation("Password is required."));
        }

        return Result.Success();
    }
}
