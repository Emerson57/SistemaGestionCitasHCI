using MedicalAppointments.Domain.Entities;
using MedicalAppointments.Domain.Exceptions;

namespace MedicalAppointments.UnitTests.Domain;

public class SpecialtyTests
{
    private static readonly DateTimeOffset Now = new(2026, 10, 1, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Create_WithEmptyName_ThrowsDomainException()
    {
        var exception = Assert.Throws<DomainException>(() =>
            Specialty.Create(1, "   ", "Description", Now));

        Assert.Contains("name", exception.Message, StringComparison.OrdinalIgnoreCase);
    }
}
