using MedicalAppointments.Web.Models.Authentication;

namespace MedicalAppointments.Web.Services.Authentication;

public interface ITokenStorageService
{
    Task<StoredAuthSession?> GetSessionAsync(CancellationToken cancellationToken = default);

    Task SetSessionAsync(StoredAuthSession session, CancellationToken cancellationToken = default);

    Task RemoveSessionAsync(CancellationToken cancellationToken = default);
}
