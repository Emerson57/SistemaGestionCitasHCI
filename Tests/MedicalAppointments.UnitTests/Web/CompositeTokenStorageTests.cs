using MedicalAppointments.Web.Models.Authentication;
using MedicalAppointments.Web.Services.Authentication;

namespace MedicalAppointments.UnitTests.Web;

public sealed class CompositeTokenStorageTests
{
    [Fact]
    public async Task GetSessionAsync_falls_back_to_api_session_cookie()
    {
        var browser = new InMemoryTokenStorage();
        var cookieStore = new FakeApiSessionCookieStore
        {
            Session = new StoredAuthSession
            {
                AccessToken = "token-from-cookie",
                User = new AuthenticationResponse
                {
                    UserId = "u1",
                    FullName = "Test",
                    Email = "t@test.local",
                    Role = "Patient",
                    AccessToken = "token-from-cookie"
                }
            }
        };

        var composite = new CompositeTokenStorageService(browser, cookieStore);
        var session = await composite.GetSessionAsync();

        Assert.Equal("token-from-cookie", session?.AccessToken);
    }

    private sealed class InMemoryTokenStorage : ITokenStorageService
    {
        public StoredAuthSession? Session { get; set; }

        public Task<StoredAuthSession?> GetSessionAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult(Session);

        public Task SetSessionAsync(StoredAuthSession session, CancellationToken cancellationToken = default)
        {
            Session = session;
            return Task.CompletedTask;
        }

        public Task RemoveSessionAsync(CancellationToken cancellationToken = default)
        {
            Session = null;
            return Task.CompletedTask;
        }
    }

    private sealed class FakeApiSessionCookieStore : IApiSessionCookieStore
    {
        public StoredAuthSession? Session { get; set; }

        public void SetFromAuthenticationResponse(AuthenticationResponse response) =>
            Session = new StoredAuthSession
            {
                AccessToken = response.AccessToken!,
                User = response
            };

        public StoredAuthSession? GetCurrent() => Session;

        public void Delete() => Session = null;
    }
}
