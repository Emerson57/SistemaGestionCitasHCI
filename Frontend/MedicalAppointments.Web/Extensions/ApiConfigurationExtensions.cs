namespace MedicalAppointments.Web.Extensions;

public static class ApiConfigurationExtensions
{
    public const string ApiSectionName = "Api";

    public static Uri GetApiBaseUri(this IConfiguration configuration)
    {
        var baseUrl = configuration[$"{ApiSectionName}:BaseUrl"];
        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            throw new InvalidOperationException("Api:BaseUrl is not configured.");
        }

        return new Uri(baseUrl.TrimEnd('/') + "/");
    }
}
