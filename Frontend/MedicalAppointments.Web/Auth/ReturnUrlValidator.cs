using Microsoft.AspNetCore.WebUtilities;

namespace MedicalAppointments.Web.Auth;

public static class ReturnUrlValidator
{
    public static bool IsSafeLocalReturnUrl(string? returnUrl)
    {
        var normalized = NormalizeLocalPath(returnUrl);
        return normalized is not null
               && normalized.StartsWith('/')
               && !normalized.StartsWith("//", StringComparison.Ordinal)
               && !normalized.Contains('\\')
               && !normalized.Contains('@', StringComparison.Ordinal);
    }

    public static string? NormalizeLocalPath(string? returnUrl)
    {
        if (string.IsNullOrWhiteSpace(returnUrl))
        {
            return null;
        }

        var trimmed = returnUrl.Trim();
        if (trimmed.StartsWith("//", StringComparison.Ordinal)
            || trimmed.Contains('\\')
            || trimmed.Contains('@', StringComparison.Ordinal)
            || trimmed.Contains("://", StringComparison.Ordinal))
        {
            return null;
        }

        return trimmed.StartsWith('/') ? trimmed : $"/{trimmed}";
    }

    public static string? ParseFromUri(string navigationUri)
    {
        if (!Uri.TryCreate(navigationUri, UriKind.Absolute, out var uri))
        {
            return null;
        }

        return ParseFromQuery(uri.Query);
    }

    public static string? ParseFromQuery(string queryString)
    {
        var query = QueryHelpers.ParseQuery(queryString);
        if (query.TryGetValue("returnUrl", out var lower) && !string.IsNullOrWhiteSpace(lower))
        {
            return lower.ToString();
        }

        if (query.TryGetValue("ReturnUrl", out var upper) && !string.IsNullOrWhiteSpace(upper))
        {
            return upper.ToString();
        }

        return null;
    }
}
