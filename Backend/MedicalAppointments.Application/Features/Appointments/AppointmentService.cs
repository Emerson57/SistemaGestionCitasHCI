using MedicalAppointments.Application.Abstractions.Identity;
using MedicalAppointments.Application.Abstractions.Persistence;
using MedicalAppointments.Application.Abstractions.Time;
using MedicalAppointments.Application.Common.Mappings;
using MedicalAppointments.Application.Common.Results;
using MedicalAppointments.Application.Common.Validation;
using MedicalAppointments.Application.DTOs.Appointments;
using MedicalAppointments.Domain.Entities;
using MedicalAppointments.Domain.Enums;
using MedicalAppointments.Domain.Exceptions;

namespace MedicalAppointments.Application.Features.Appointments;

public sealed class AppointmentService(
    IAppointmentRepository appointmentRepository,
    IPatientRepository patientRepository,
    IDoctorRepository doctorRepository,
    ISpecialtyRepository specialtyRepository,
    IDoctorAvailabilityRepository availabilityRepository,
    ICurrentUserService currentUserService,
    IUnitOfWork unitOfWork,
    IDateTimeProvider dateTimeProvider) : IAppointmentService
{
    private const string PatientRole = "Patient";
    private const string DoctorRole = "Doctor";

    public async Task<Result<AppointmentDto>> ScheduleAsync(
        ScheduleAppointmentRequest request,
        CancellationToken cancellationToken)
    {
        var validation = AppointmentRequestValidator.ValidateSchedule(request, dateTimeProvider.UtcNow);
        if (validation.IsFailure)
        {
            return Result<AppointmentDto>.Failure(validation.Error!);
        }

        var patientResult = await ResolveCurrentPatientAsync(cancellationToken);
        if (patientResult.IsFailure)
        {
            return Result<AppointmentDto>.Failure(patientResult.Error!);
        }

        var patient = patientResult.Value!;

        var doctor = await doctorRepository.GetByIdAsync(request.DoctorId, cancellationToken);
        if (doctor is null)
        {
            return Result<AppointmentDto>.Failure(Error.NotFound("Doctor not found."));
        }

        if (!doctor.IsActive)
        {
            return Result<AppointmentDto>.Failure(Error.Validation("Doctor is not active."));
        }

        if (await appointmentRepository.ExistsActiveAppointmentForSlotAsync(
                request.DoctorId,
                request.AppointmentDateTime,
                excludeAppointmentId: null,
                cancellationToken))
        {
            return Result<AppointmentDto>.Failure(
                Error.Conflict("The selected doctor and time slot is already occupied."));
        }

        var slot = await FindAvailableSlotAsync(
            request.DoctorId,
            request.AppointmentDateTime,
            cancellationToken);

        if (slot is null)
        {
            return Result<AppointmentDto>.Failure(
                Error.Validation("No available slot matches the selected date and time."));
        }

        Appointment appointment;
        try
        {
            appointment = Appointment.Create(
                Guid.NewGuid(),
                patient.Id,
                doctor.Id,
                request.AppointmentDateTime,
                request.Reason,
                dateTimeProvider.UtcNow);

            slot.Reserve(dateTimeProvider.UtcNow);
        }
        catch (DomainException ex)
        {
            return Result<AppointmentDto>.Failure(Error.Validation(ex.Message));
        }

        await appointmentRepository.AddAsync(appointment, cancellationToken);
        availabilityRepository.Update(slot);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = await MapToDtoAsync(appointment, patient, doctor, cancellationToken);
        return Result<AppointmentDto>.Success(dto);
    }

    public async Task<Result<IReadOnlyList<AppointmentDto>>> GetMyAppointmentsAsync(
        CancellationToken cancellationToken)
    {
        var patientResult = await ResolveCurrentPatientAsync(cancellationToken);
        if (patientResult.IsFailure)
        {
            return Result<IReadOnlyList<AppointmentDto>>.Failure(patientResult.Error!);
        }

        var patient = patientResult.Value!;
        var appointments = await appointmentRepository.GetByPatientAsync(patient.Id, cancellationToken);

        var active = appointments
            .Where(a => a.Status is AppointmentStatus.Scheduled
                or AppointmentStatus.Confirmed)
            .ToList();

        var dtos = new List<AppointmentDto>();
        foreach (var appointment in active)
        {
            dtos.Add(await MapToDtoAsync(appointment, patient, cancellationToken));
        }

        return Result<IReadOnlyList<AppointmentDto>>.Success(dtos);
    }

    public async Task<Result<IReadOnlyList<AppointmentDto>>> GetMyHistoryAsync(
        CancellationToken cancellationToken)
    {
        var patientResult = await ResolveCurrentPatientAsync(cancellationToken);
        if (patientResult.IsFailure)
        {
            return Result<IReadOnlyList<AppointmentDto>>.Failure(patientResult.Error!);
        }

        var patient = patientResult.Value!;
        var history = await appointmentRepository.GetHistoryByPatientAsync(patient.Id, cancellationToken);

        var dtos = new List<AppointmentDto>();
        foreach (var appointment in history)
        {
            dtos.Add(await MapToDtoAsync(appointment, patient, cancellationToken));
        }

        return Result<IReadOnlyList<AppointmentDto>>.Success(dtos);
    }

    public async Task<Result<IReadOnlyList<AppointmentDto>>> GetDoctorAgendaAsync(
        CancellationToken cancellationToken)
    {
        var doctorResult = await ResolveCurrentDoctorAsync(cancellationToken);
        if (doctorResult.IsFailure)
        {
            return Result<IReadOnlyList<AppointmentDto>>.Failure(doctorResult.Error!);
        }

        var doctor = doctorResult.Value!;
        var appointments = await appointmentRepository.GetByDoctorAsync(doctor.Id, cancellationToken);

        var dtos = new List<AppointmentDto>();
        foreach (var appointment in appointments)
        {
            dtos.Add(await MapToDtoAsync(appointment, doctor, cancellationToken));
        }

        return Result<IReadOnlyList<AppointmentDto>>.Success(dtos);
    }

    public async Task<Result<AppointmentDto>> RescheduleAsync(
        Guid appointmentId,
        RescheduleAppointmentRequest request,
        CancellationToken cancellationToken)
    {
        var validation = AppointmentRequestValidator.ValidateReschedule(request, dateTimeProvider.UtcNow);
        if (validation.IsFailure)
        {
            return Result<AppointmentDto>.Failure(validation.Error!);
        }

        var patientResult = await ResolveCurrentPatientAsync(cancellationToken);
        if (patientResult.IsFailure)
        {
            return Result<AppointmentDto>.Failure(patientResult.Error!);
        }

        var patient = patientResult.Value!;
        var appointment = await appointmentRepository.GetByIdAsync(appointmentId, cancellationToken);
        if (appointment is null)
        {
            return Result<AppointmentDto>.Failure(Error.NotFound("Appointment not found."));
        }

        if (appointment.PatientId != patient.Id)
        {
            return Result<AppointmentDto>.Failure(
                Error.Forbidden("Patients can reschedule only their own appointments."));
        }

        if (await appointmentRepository.ExistsActiveAppointmentForSlotAsync(
                appointment.DoctorId,
                request.AppointmentDateTime,
                excludeAppointmentId: appointment.Id,
                cancellationToken))
        {
            return Result<AppointmentDto>.Failure(
                Error.Conflict("The selected doctor and time slot is already occupied."));
        }

        var previousDateTime = appointment.AppointmentDateTime;
        var releasedSlot = await ReleaseSlotForDateTimeAsync(
            appointment.DoctorId,
            previousDateTime,
            cancellationToken);

        DoctorAvailability? newSlot = releasedSlot is not null
            && SlotContainsDateTime(releasedSlot, request.AppointmentDateTime)
            ? releasedSlot
            : await FindAvailableSlotAsync(
                appointment.DoctorId,
                request.AppointmentDateTime,
                cancellationToken);

        if (newSlot is null)
        {
            return Result<AppointmentDto>.Failure(
                Error.Validation("No available slot matches the selected date and time."));
        }

        try
        {
            appointment.Reschedule(
                request.AppointmentDateTime,
                currentUserService.UserId!,
                dateTimeProvider.UtcNow);

            newSlot.Reserve(dateTimeProvider.UtcNow);
        }
        catch (DomainException ex)
        {
            if (releasedSlot is not null)
            {
                try
                {
                    releasedSlot.Reserve(dateTimeProvider.UtcNow);
                    availabilityRepository.Update(releasedSlot);
                }
                catch (DomainException)
                {
                    // Best-effort rollback of released slot.
                }
            }

            return Result<AppointmentDto>.Failure(Error.Validation(ex.Message));
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = await MapToDtoAsync(appointment, patient, cancellationToken);
        return Result<AppointmentDto>.Success(dto);
    }

    public async Task<Result> CancelAsync(Guid appointmentId, CancellationToken cancellationToken)
    {
        var patientResult = await ResolveCurrentPatientAsync(cancellationToken);
        if (patientResult.IsFailure)
        {
            return Result.Failure(patientResult.Error!);
        }

        var patient = patientResult.Value!;
        var appointment = await appointmentRepository.GetByIdAsync(appointmentId, cancellationToken);
        if (appointment is null)
        {
            return Result.Failure(Error.NotFound("Appointment not found."));
        }

        if (appointment.PatientId != patient.Id)
        {
            return Result.Failure(
                Error.Forbidden("Patients can cancel only their own appointments."));
        }

        var releasedSlot = await ReleaseSlotForDateTimeAsync(
            appointment.DoctorId,
            appointment.AppointmentDateTime,
            cancellationToken);

        try
        {
            appointment.Cancel(currentUserService.UserId!, dateTimeProvider.UtcNow);
        }
        catch (DomainException ex)
        {
            if (releasedSlot is not null)
            {
                try
                {
                    releasedSlot.Reserve(dateTimeProvider.UtcNow);
                    availabilityRepository.Update(releasedSlot);
                }
                catch (DomainException)
                {
                    // Best-effort rollback of released slot.
                }
            }

            return Result.Failure(Error.Validation(ex.Message));
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result<AppointmentDto>> ConfirmAsync(
        Guid appointmentId,
        CancellationToken cancellationToken)
    {
        var doctorResult = await ResolveCurrentDoctorAsync(cancellationToken);
        if (doctorResult.IsFailure)
        {
            return Result<AppointmentDto>.Failure(doctorResult.Error!);
        }

        var doctor = doctorResult.Value!;
        var appointment = await appointmentRepository.GetByIdAsync(appointmentId, cancellationToken);
        if (appointment is null)
        {
            return Result<AppointmentDto>.Failure(Error.NotFound("Appointment not found."));
        }

        if (appointment.DoctorId != doctor.Id)
        {
            return Result<AppointmentDto>.Failure(
                Error.Forbidden("Doctors can confirm only their own appointments."));
        }

        try
        {
            appointment.Confirm(currentUserService.UserId!, dateTimeProvider.UtcNow);
        }
        catch (DomainException ex)
        {
            return Result<AppointmentDto>.Failure(Error.Validation(ex.Message));
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = await MapToDtoAsync(appointment, doctor, cancellationToken);
        return Result<AppointmentDto>.Success(dto);
    }

    public async Task<Result<AppointmentDto>> CompleteAsync(
        Guid appointmentId,
        CancellationToken cancellationToken)
    {
        var doctorResult = await ResolveCurrentDoctorAsync(cancellationToken);
        if (doctorResult.IsFailure)
        {
            return Result<AppointmentDto>.Failure(doctorResult.Error!);
        }

        var doctor = doctorResult.Value!;
        var appointment = await appointmentRepository.GetByIdAsync(appointmentId, cancellationToken);
        if (appointment is null)
        {
            return Result<AppointmentDto>.Failure(Error.NotFound("Appointment not found."));
        }

        if (appointment.DoctorId != doctor.Id)
        {
            return Result<AppointmentDto>.Failure(
                Error.Forbidden("Doctors can complete only their own appointments."));
        }

        try
        {
            appointment.Complete(currentUserService.UserId!, dateTimeProvider.UtcNow);
        }
        catch (DomainException ex)
        {
            return Result<AppointmentDto>.Failure(Error.Validation(ex.Message));
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = await MapToDtoAsync(appointment, doctor, cancellationToken);
        return Result<AppointmentDto>.Success(dto);
    }

    private async Task<DoctorAvailability?> FindAvailableSlotAsync(
        Guid doctorId,
        DateTimeOffset appointmentDateTime,
        CancellationToken cancellationToken)
    {
        var slots = await availabilityRepository.GetAvailableByDoctorAsync(doctorId, cancellationToken);
        return slots.FirstOrDefault(s => SlotContainsDateTime(s, appointmentDateTime));
    }

    private static bool SlotContainsDateTime(DoctorAvailability slot, DateTimeOffset appointmentDateTime)
    {
        var date = DateOnly.FromDateTime(appointmentDateTime.UtcDateTime);
        var time = TimeOnly.FromDateTime(appointmentDateTime.UtcDateTime);

        return slot.Date == date
            && slot.StartTime <= time
            && time < slot.EndTime;
    }

    private async Task<DoctorAvailability?> ReleaseSlotForDateTimeAsync(
        Guid doctorId,
        DateTimeOffset appointmentDateTime,
        CancellationToken cancellationToken)
    {
        var date = DateOnly.FromDateTime(appointmentDateTime.UtcDateTime);
        var time = TimeOnly.FromDateTime(appointmentDateTime.UtcDateTime);

        var slots = await availabilityRepository.GetAllByDoctorAsync(doctorId, cancellationToken);
        var slot = slots.FirstOrDefault(s =>
            s.Date == date
            && s.StartTime <= time
            && time < s.EndTime
            && s.Status == AvailabilityStatus.Reserved);

        if (slot is null)
        {
            return null;
        }

        slot.Release(dateTimeProvider.UtcNow);
        return slot;
    }

    private async Task<Result<Patient>> ResolveCurrentPatientAsync(CancellationToken cancellationToken)
    {
        if (!currentUserService.IsAuthenticated || string.IsNullOrWhiteSpace(currentUserService.UserId))
        {
            return Result<Patient>.Failure(Error.Unauthorized("User is not authenticated."));
        }

        if (!string.Equals(currentUserService.Role, PatientRole, StringComparison.OrdinalIgnoreCase))
        {
            return Result<Patient>.Failure(Error.Forbidden("Current user is not a patient."));
        }

        var patient = await patientRepository.GetByUserIdAsync(currentUserService.UserId, cancellationToken);
        if (patient is null)
        {
            return Result<Patient>.Failure(Error.NotFound("Patient profile not found."));
        }

        return Result<Patient>.Success(patient);
    }

    private async Task<Result<Doctor>> ResolveCurrentDoctorAsync(CancellationToken cancellationToken)
    {
        if (!currentUserService.IsAuthenticated || string.IsNullOrWhiteSpace(currentUserService.UserId))
        {
            return Result<Doctor>.Failure(Error.Unauthorized("User is not authenticated."));
        }

        if (!string.Equals(currentUserService.Role, DoctorRole, StringComparison.OrdinalIgnoreCase))
        {
            return Result<Doctor>.Failure(Error.Forbidden("Current user is not a doctor."));
        }

        var doctor = await doctorRepository.GetByUserIdAsync(currentUserService.UserId, cancellationToken);
        if (doctor is null)
        {
            return Result<Doctor>.Failure(Error.NotFound("Doctor profile not found."));
        }

        return Result<Doctor>.Success(doctor);
    }

    private async Task<AppointmentDto> MapToDtoAsync(
        Appointment appointment,
        Patient patient,
        CancellationToken cancellationToken)
    {
        var doctor = await doctorRepository.GetByIdAsync(appointment.DoctorId, cancellationToken)
            ?? throw new InvalidOperationException("Doctor not found for appointment mapping.");

        return await MapToDtoAsync(appointment, patient, doctor, cancellationToken);
    }

    private async Task<AppointmentDto> MapToDtoAsync(
        Appointment appointment,
        Doctor doctor,
        CancellationToken cancellationToken)
    {
        var patient = await patientRepository.GetByIdAsync(appointment.PatientId, cancellationToken)
            ?? throw new InvalidOperationException("Patient not found for appointment mapping.");

        return await MapToDtoAsync(appointment, patient, doctor, cancellationToken);
    }

    private async Task<AppointmentDto> MapToDtoAsync(
        Appointment appointment,
        Patient patient,
        Doctor doctor,
        CancellationToken cancellationToken)
    {
        var specialty = await specialtyRepository.GetByIdAsync(doctor.SpecialtyId, cancellationToken);
        return appointment.ToDto(
            patient.FullName,
            doctor.FullName,
            doctor.SpecialtyId,
            specialty?.Name);
    }
}
