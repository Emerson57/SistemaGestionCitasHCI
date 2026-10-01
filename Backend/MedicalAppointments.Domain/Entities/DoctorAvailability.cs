using MedicalAppointments.Domain.Enums;
using MedicalAppointments.Domain.Exceptions;

namespace MedicalAppointments.Domain.Entities;

public class DoctorAvailability
{
    private DoctorAvailability()
    {
    }

    private DoctorAvailability(
        Guid id,
        Guid doctorId,
        DateOnly date,
        TimeOnly startTime,
        TimeOnly endTime,
        DateTimeOffset createdAt)
    {
        Id = id;
        DoctorId = doctorId;
        Date = date;
        StartTime = startTime;
        EndTime = endTime;
        Status = AvailabilityStatus.Available;
        CreatedAt = createdAt;
    }

    public Guid Id { get; private set; }

    public Guid DoctorId { get; private set; }

    public DateOnly Date { get; private set; }

    public TimeOnly StartTime { get; private set; }

    public TimeOnly EndTime { get; private set; }

    public AvailabilityStatus Status { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset? UpdatedAt { get; private set; }

    public Doctor? Doctor { get; private set; }

    public static DoctorAvailability Create(
        Guid id,
        Guid doctorId,
        DateOnly date,
        TimeOnly startTime,
        TimeOnly endTime,
        DateTimeOffset createdAt)
    {
        if (id == Guid.Empty)
        {
            throw new DomainException("Availability id cannot be empty.");
        }

        if (doctorId == Guid.Empty)
        {
            throw new DomainException("Doctor id cannot be empty.");
        }

        ValidateTimeRange(startTime, endTime);

        return new DoctorAvailability(id, doctorId, date, startTime, endTime, createdAt);
    }

    public void Reserve(DateTimeOffset updatedAt)
    {
        if (Status != AvailabilityStatus.Available)
        {
            throw new DomainException("Only available slots can be reserved.");
        }

        Status = AvailabilityStatus.Reserved;
        UpdatedAt = updatedAt;
    }

    public void Release(DateTimeOffset updatedAt)
    {
        if (Status != AvailabilityStatus.Reserved)
        {
            throw new DomainException("Only reserved slots can be released.");
        }

        Status = AvailabilityStatus.Available;
        UpdatedAt = updatedAt;
    }

    public void MarkUnavailable(DateTimeOffset updatedAt)
    {
        if (Status == AvailabilityStatus.Reserved)
        {
            throw new DomainException("Reserved slots cannot be marked unavailable. Release the slot first.");
        }

        Status = AvailabilityStatus.Unavailable;
        UpdatedAt = updatedAt;
    }

    internal void AssignDoctor(Doctor doctor)
    {
        Doctor = doctor;
        DoctorId = doctor.Id;
    }

    private static void ValidateTimeRange(TimeOnly startTime, TimeOnly endTime)
    {
        if (endTime <= startTime)
        {
            throw new DomainException("End time must be greater than start time.");
        }
    }
}
