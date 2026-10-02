namespace MedicalAppointments.Web.Services.Api;

public static class ApiErrorTranslator
{
    public static string ToUserMessage(ApiException exception) =>
        exception.StatusCode switch
        {
            401 => "Tu sesión ha expirado. Inicia sesión nuevamente.",
            403 => "No tienes permisos para realizar esta acción.",
            404 => "No se encontró el recurso solicitado.",
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
}
