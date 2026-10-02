using MedicalAppointments.Application.DTOs.Specialties;
using MedicalAppointments.Application.Features.Specialties;
using MedicalAppointments.Domain.Entities;
using MedicalAppointments.UnitTests.Application.Fakes;

namespace MedicalAppointments.UnitTests.Application;

public class SpecialtyServiceTests
{
    private static readonly DateTimeOffset Now = new(2026, 10, 1, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public async Task CreateAsync_RejectsDuplicateSpecialtyName()
    {
        var repository = new InMemorySpecialtyRepository();
        var existing = Specialty.Create("Cardiology", null, Now);
        existing.AssignIdentity(1);
        repository.Seed(existing);

        var service = new SpecialtyService(repository, new FakeUnitOfWork(), new FakeDateTimeProvider(Now));

        var result = await service.CreateAsync(
            new CreateSpecialtyRequest { Name = "Cardiology", Description = "Heart" },
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("Conflict", result.Error!.Code);
    }

    [Fact]
    public async Task CreateAsync_CreatesValidSpecialty()
    {
        var repository = new InMemorySpecialtyRepository();
        var service = new SpecialtyService(repository, new FakeUnitOfWork(), new FakeDateTimeProvider(Now));

        var result = await service.CreateAsync(
            new CreateSpecialtyRequest { Name = "Dermatology", Description = "Skin" },
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("Dermatology", result.Value!.Name);
        Assert.True(result.Value.IsActive);
    }
}
