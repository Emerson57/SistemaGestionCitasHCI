using MedicalAppointments.Application.Common.Results;
using MedicalAppointments.Application.DTOs.Specialties;

namespace MedicalAppointments.Application.Common.Validation;

public static class SpecialtyRequestValidator
{
    public static Result ValidateCreate(CreateSpecialtyRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return Result.Failure(Error.Validation("Specialty name is required."));
        }

        return Result.Success();
    }

    public static Result ValidateUpdate(UpdateSpecialtyRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return Result.Failure(Error.Validation("Specialty name is required."));
        }

        return Result.Success();
    }
}
