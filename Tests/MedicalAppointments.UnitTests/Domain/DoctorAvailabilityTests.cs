using MedicalAppointments.Domain.Entities;
using MedicalAppointments.Domain.Enums;
using MedicalAppointments.Domain.Exceptions;

namespace MedicalAppointments.UnitTests.Domain;

public class DoctorAvailabilityTests
{
    private static readonly DateTimeOffset Now = new(2026, 10, 1, 8, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Create_WithValidTimeRange_Succeeds()
    {
        var availability = DoctorAvailability.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            new DateOnly(2026, 10, 15),
            new TimeOnly(9, 0),
            new TimeOnly(10, 0),
            Now);

        Assert.Equal(AvailabilityStatus.Available, availability.Status);
        Assert.Equal(new TimeOnly(9, 0), availability.StartTime);
        Assert.Equal(new TimeOnly(10, 0), availability.EndTime);
    }

    [Fact]
    public void Create_WithEndTimeNotAfterStartTime_ThrowsDomainException()
    {
        var exception = Assert.Throws<DomainException>(() =>
            DoctorAvailability.Create(
                Guid.NewGuid(),
                Guid.NewGuid(),
                new DateOnly(2026, 10, 15),
                new TimeOnly(10, 0),
                new TimeOnly(10, 0),
                Now));

        Assert.Contains("End time", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Reserve_WhenAlreadyReserved_ThrowsDomainException()
    {
        var availability = DoctorAvailability.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            new DateOnly(2026, 10, 15),
            new TimeOnly(9, 0),
            new TimeOnly(10, 0),
            Now);

        availability.Reserve(Now);

        var exception = Assert.Throws<DomainException>(() => availability.Reserve(Now.AddMinutes(1)));

        Assert.Contains("available", exception.Message, StringComparison.OrdinalIgnoreCase);
    }
}
