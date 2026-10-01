using MedicalAppointments.Application.Common.Results;
using MedicalAppointments.Application.DTOs.Doctors;

namespace MedicalAppointments.Application.Common.Validation;

public static class DoctorRequestValidator
{
    public static Result ValidateCreate(CreateDoctorRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.FullName))
        {
            return Result.Failure(Error.Validation("Doctor full name is required."));
        }

        if (string.IsNullOrWhiteSpace(request.ProfessionalLicense))
        {
            return Result.Failure(Error.Validation("Professional license is required."));
        }

        if (request.SpecialtyId <= 0)
        {
            return Result.Failure(Error.Validation("A valid specialty is required."));
        }

        if (string.IsNullOrWhiteSpace(request.UserId))
        {
            return Result.Failure(Error.Validation("User id is required."));
        }

        return Result.Success();
    }

    public static Result ValidateUpdate(UpdateDoctorRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.FullName))
        {
            return Result.Failure(Error.Validation("Doctor full name is required."));
        }

        if (string.IsNullOrWhiteSpace(request.ProfessionalLicense))
        {
            return Result.Failure(Error.Validation("Professional license is required."));
        }

        if (request.SpecialtyId <= 0)
        {
            return Result.Failure(Error.Validation("A valid specialty is required."));
        }

        return Result.Success();
    }
}
