using MedicalAppointments.Application.DTOs.Doctors;
using MedicalAppointments.Application.Features.Doctors;
using MedicalAppointments.UnitTests.Application.Fakes;

namespace MedicalAppointments.UnitTests.Application;

public class DoctorServiceTests
{
    private static readonly DateTimeOffset Now = new(2026, 10, 1, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public async Task CreateAsync_RejectsNonexistentSpecialty()
    {
        var doctorRepository = new InMemoryDoctorRepository();
        var specialtyRepository = new InMemorySpecialtyRepository();

        var service = new DoctorService(
            doctorRepository,
            specialtyRepository,
            new FakeUnitOfWork(),
            new FakeDateTimeProvider(Now));

        var result = await service.CreateAsync(
            new CreateDoctorRequest
            {
                UserId = "doctor-user",
                FullName = "Dr. Smith",
                ProfessionalLicense = "LIC-001",
                SpecialtyId = 99
            },
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("NotFound", result.Error!.Code);
    }
}
