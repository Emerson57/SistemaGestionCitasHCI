namespace MedicalAppointments.Application.Common.Results;

public sealed class Error
{
    private Error(string code, string message)
    {
        Code = code;
        Message = message;
    }

    public string Code { get; }

    public string Message { get; }

    public static Error Validation(string message) => new("Validation", message);

    public static Error NotFound(string message) => new("NotFound", message);

    public static Error Conflict(string message) => new("Conflict", message);

    public static Error Unauthorized(string message) => new("Unauthorized", message);

    public static Error Forbidden(string message) => new("Forbidden", message);

    public static Error Failure(string code, string message) => new(code, message);
}
