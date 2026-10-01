using MedicalAppointments.Application.Common.Results;
using MedicalAppointments.Application.DTOs.Appointments;

namespace MedicalAppointments.Application.Common.Validation;

public static class AppointmentRequestValidator
{
    private const int MaxReasonLength = 500;

    public static Result ValidateSchedule(ScheduleAppointmentRequest request, DateTimeOffset utcNow)
    {
        if (request.DoctorId == Guid.Empty)
        {
            return Result.Failure(Error.Validation("Doctor id is required."));
        }

        if (request.AppointmentDateTime <= utcNow)
        {
            return Result.Failure(Error.Validation("Appointment date and time must be in the future."));
        }

        if (request.Reason is { Length: > MaxReasonLength })
        {
            return Result.Failure(Error.Validation($"Reason cannot exceed {MaxReasonLength} characters."));
        }

        return Result.Success();
    }

    public static Result ValidateReschedule(RescheduleAppointmentRequest request, DateTimeOffset utcNow)
    {
        if (request.AppointmentDateTime <= utcNow)
        {
            return Result.Failure(Error.Validation("Appointment date and time must be in the future."));
        }

        return Result.Success();
    }
}
