namespace MedicalAppointments.Web.Services.Api;

public static class ApiErrorTranslator
{
    public static string ToUserMessage(ApiException exception) =>
        exception.StatusCode switch
        {
            401 => "Tu sesión ha expirado. Inicia sesión nuevamente.",
            403 => "No tienes permisos para realizar esta acción.",
            404 when exception.Message.Contains("patient", StringComparison.OrdinalIgnoreCase)
                     || exception.Problem?.Detail?.Contains("patient", StringComparison.OrdinalIgnoreCase) == true
                     || exception.Problem?.Title?.Contains("patient", StringComparison.OrdinalIgnoreCase) == true
                => PatientProfileNotFoundMessage(),
            404 => "No se encontró el recurso solicitado.",
            409 when IsDuplicateEmailConflict(exception)
                => DuplicateEmailRegistrationMessage(),
            409 when exception.Message.Contains("horario", StringComparison.OrdinalIgnoreCase)
                     || exception.Problem?.Detail?.Contains("slot", StringComparison.OrdinalIgnoreCase) == true
                => "Este horario ya no está disponible. Selecciona otro horario.",
            409 => "La operación no pudo completarse por un conflicto. Verifica los datos e inténtalo de nuevo.",
            400 => string.IsNullOrWhiteSpace(exception.Problem?.Detail)
                ? "Revisa los datos ingresados."
                : exception.Problem!.Detail!,
            >= 500 => "No fue posible completar la operación. Inténtalo nuevamente.",
            _ => string.IsNullOrWhiteSpace(exception.Message)
                ? "Ocurrió un error inesperado."
                : exception.Message
        };

    public static string LoginFailureMessage() =>
        "Correo o contraseña incorrectos. Verifica tus datos e inténtalo de nuevo.";

    public static string MissingBearerTokenMessage() =>
        "No se pudo autenticar la solicitud. Inténtalo de nuevo.";

    public static string PatientProfileNotFoundMessage() =>
        "No fue posible encontrar tu perfil de paciente.";

    public static string NetworkUnavailableMessage() =>
        "No fue posible conectar con el servicio.";

    public static string ToRegistrationUserMessage(ApiException exception) =>
        exception.StatusCode switch
        {
            409 when IsDuplicateEmailConflict(exception) => DuplicateEmailRegistrationMessage(),
            400 => string.IsNullOrWhiteSpace(exception.Problem?.Detail)
                ? "Revisa los datos ingresados e inténtalo nuevamente."
                : exception.Problem!.Detail!,
            >= 500 => RegistrationFailedMessage(),
            _ => ToUserMessage(exception)
        };

    public static string RegistrationAccountCreatedLoginRequiredMessage() =>
        "Tu cuenta fue creada correctamente. Inicia sesión para continuar.";

    public static string RegistrationFailedMessage() =>
        "No fue posible crear la cuenta. Inténtalo nuevamente.";

    public static string DuplicateEmailRegistrationMessage() =>
        "Ya existe una cuenta registrada con este correo electrónico.";

    private static bool IsDuplicateEmailConflict(ApiException exception)
    {
        var text = $"{exception.Message} {exception.Problem?.Detail} {exception.Problem?.Title}";
        return text.Contains("email already exists", StringComparison.OrdinalIgnoreCase)
               || (text.Contains("correo", StringComparison.OrdinalIgnoreCase)
                   && text.Contains("exist", StringComparison.OrdinalIgnoreCase));
    }
}
