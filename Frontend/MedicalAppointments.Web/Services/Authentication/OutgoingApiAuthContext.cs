namespace MedicalAppointments.Web.Services.Authentication;

public interface IOutgoingApiAuthContext
{
    bool BearerTokenAttached { get; set; }
}

public sealed class OutgoingApiAuthContext : IOutgoingApiAuthContext
{
    public bool BearerTokenAttached { get; set; }
}
