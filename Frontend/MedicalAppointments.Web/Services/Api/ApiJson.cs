using System.Text.Json;

namespace MedicalAppointments.Web.Services.Api;

internal static class ApiJson
{
    public static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web);
}
