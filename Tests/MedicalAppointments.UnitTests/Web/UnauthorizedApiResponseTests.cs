using MedicalAppointments.Web.Services.Authentication;

namespace MedicalAppointments.UnitTests.Web;

public sealed class UnauthorizedApiResponseTests
{
    [Fact]
    public void Session_expired_handling_requires_bearer_token_was_attached()
    {
        var withoutBearer = new OutgoingApiAuthContext { BearerTokenAttached = false };
        var withBearer = new OutgoingApiAuthContext { BearerTokenAttached = true };

        Assert.False(ShouldTreatUnauthorizedAsSessionExpired(withoutBearer));
        Assert.True(ShouldTreatUnauthorizedAsSessionExpired(withBearer));
    }

    private static bool ShouldTreatUnauthorizedAsSessionExpired(IOutgoingApiAuthContext context) =>
        context.BearerTokenAttached;
}
