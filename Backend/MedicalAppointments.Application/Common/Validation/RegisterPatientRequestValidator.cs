using System.Net.Mail;
using MedicalAppointments.Application.Common.Results;
using MedicalAppointments.Application.DTOs.Authentication;

namespace MedicalAppointments.Application.Common.Validation;

public static class RegisterPatientRequestValidator
{
    public static Result Validate(RegisterPatientRequest request, DateOnly today)
    {
        if (string.IsNullOrWhiteSpace(request.FullName))
        {
            return Result.Failure(Error.Validation("Full name is required."));
        }

        if (string.IsNullOrWhiteSpace(request.Email))
        {
            return Result.Failure(Error.Validation("Email is required."));
        }

        if (!IsValidEmail(request.Email))
        {
            return Result.Failure(Error.Validation("Email format is invalid."));
        }

        if (request.BirthDate > today)
        {
            return Result.Failure(Error.Validation("Birth date cannot be in the future."));
        }

        if (string.IsNullOrWhiteSpace(request.PhoneNumber))
        {
            return Result.Failure(Error.Validation("Phone number is required."));
        }

        return Result.Success();
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            _ = new MailAddress(email);
            return true;
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
