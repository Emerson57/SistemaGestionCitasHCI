namespace MedicalAppointments.Web.Services.Api;

public sealed class ApiProblemDetails
{
    public string? Title { get; init; }

    public string? Detail { get; init; }

    public int? Status { get; init; }
}

public sealed class ApiException : Exception
{
    public ApiException(string userMessage, int statusCode, ApiProblemDetails? problem = null)
        : base(userMessage)
    {
        StatusCode = statusCode;
        Problem = problem;
    }

    public int StatusCode { get; }

    public ApiProblemDetails? Problem { get; }
}
