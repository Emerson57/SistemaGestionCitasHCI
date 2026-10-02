namespace MedicalAppointments.Web.Services.State;

public sealed class NotificationService
{
    public event Action? OnChange;

    public string? Message { get; private set; }

    public string Type { get; private set; } = "info";

    public void ShowSuccess(string message) => Show(message, "success");

    public void ShowInfo(string message) => Show(message, "info");

    public void ShowError(string message) => Show(message, "error");

    public void Clear()
    {
        Message = null;
        OnChange?.Invoke();
    }

    private void Show(string message, string type)
    {
        Message = message;
        Type = type;
        OnChange?.Invoke();
    }
}
