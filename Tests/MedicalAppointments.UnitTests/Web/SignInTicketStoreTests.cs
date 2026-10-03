using MedicalAppointments.Web.Models.Authentication;
using MedicalAppointments.Web.Services.Authentication;
using Microsoft.Extensions.Caching.Memory;

namespace MedicalAppointments.UnitTests.Web;

public sealed class SignInTicketStoreTests
{
    [Fact]
    public void ConsumeTicket_is_one_time_use()
    {
        var store = new SignInTicketStore(new MemoryCache(new MemoryCacheOptions()));
        var response = new AuthenticationResponse
        {
            UserId = "u1",
            FullName = "Test",
            Email = "t@test.local",
            Role = "Patient",
            AccessToken = "token-value"
        };

        var ticket = store.CreateTicket(response);
        var first = store.ConsumeTicket(ticket);
        var second = store.ConsumeTicket(ticket);

        Assert.NotNull(first);
        Assert.Null(second);
    }
}
