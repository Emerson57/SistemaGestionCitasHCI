using MedicalAppointments.Web.Models.Authentication;
using Microsoft.AspNetCore.Components;

namespace MedicalAppointments.Web.Services.Authentication;

public static class RegistrationSignInHelper
{
    public static async Task CompleteSignInAsync(
        AuthenticationResponse response,
        JwtAuthenticationStateProvider authProvider,
        ISignInTicketStore signInTickets,
        NavigationManager navigation)
    {
        if (string.IsNullOrWhiteSpace(response.AccessToken))
        {
            throw new RegistrationSignInException("Authentication response did not include an access token.");
        }

        await authProvider.SignInAsync(response);
        var ticket = signInTickets.CreateTicket(response);
        navigation.NavigateTo($"/auth/complete/{ticket}", forceLoad: true);
    }
}

public sealed class RegistrationSignInException(string message) : Exception(message);
