using MedicalAppointments.Web.Extensions;

namespace MedicalAppointments.Web.Services.Authentication;

public sealed class BearerTokenHandler(
    ITokenStorageService tokenStorage,
    IOutgoingApiAuthContext outgoingApiAuthContext,
    IConfiguration configuration) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        outgoingApiAuthContext.BearerTokenAttached = false;

        var baseUri = configuration.GetApiBaseUri();
        if (request.RequestUri is not null && IsSameOrigin(request.RequestUri, baseUri))
        {
            var session = await tokenStorage.GetSessionAsync(cancellationToken);
            if (!string.IsNullOrWhiteSpace(session?.AccessToken))
            {
                request.Headers.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", session.AccessToken);
                outgoingApiAuthContext.BearerTokenAttached = true;
            }
        }

        return await base.SendAsync(request, cancellationToken);
    }

    private static bool IsSameOrigin(Uri requestUri, Uri baseUri) =>
        string.Equals(requestUri.Scheme, baseUri.Scheme, StringComparison.OrdinalIgnoreCase)
        && string.Equals(requestUri.Host, baseUri.Host, StringComparison.OrdinalIgnoreCase)
        && requestUri.Port == baseUri.Port;
}
