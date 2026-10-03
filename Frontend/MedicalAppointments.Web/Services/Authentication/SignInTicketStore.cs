using MedicalAppointments.Web.Models.Authentication;
using Microsoft.Extensions.Caching.Memory;

namespace MedicalAppointments.Web.Services.Authentication;

public interface ISignInTicketStore
{
    string CreateTicket(AuthenticationResponse response);

    AuthenticationResponse? ConsumeTicket(string ticketId);
}

public sealed class SignInTicketStore(IMemoryCache cache) : ISignInTicketStore
{
    private static readonly TimeSpan TicketLifetime = TimeSpan.FromMinutes(2);

    public string CreateTicket(AuthenticationResponse response)
    {
        var ticketId = Guid.NewGuid().ToString("N");
        cache.Set(ticketId, response, TicketLifetime);
        return ticketId;
    }

    public AuthenticationResponse? ConsumeTicket(string ticketId)
    {
        if (string.IsNullOrWhiteSpace(ticketId))
        {
            return null;
        }

        if (!cache.TryGetValue(ticketId, out AuthenticationResponse? response))
        {
            return null;
        }

        cache.Remove(ticketId);
        return response;
    }
}
