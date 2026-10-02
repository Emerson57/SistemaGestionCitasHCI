using MedicalAppointments.Web.Models.Enums;

namespace MedicalAppointments.Web.Helpers;

public static class DisplayLabels
{
    public static string FormatAppointmentStatus(AppointmentStatus status) => status switch
    {
        AppointmentStatus.Scheduled => "Programada",
        AppointmentStatus.Confirmed => "Confirmada",
        AppointmentStatus.Completed => "Completada",
        AppointmentStatus.Cancelled => "Cancelada",
        AppointmentStatus.Rescheduled => "Reprogramada (historial)",
        _ => status.ToString()
    };

    public static string FormatSex(Sex sex) => sex switch
    {
        Sex.Male => "Masculino",
        Sex.Female => "Femenino",
        Sex.NonBinary => "No binario",
        Sex.PreferNotToSay => "Prefiero no decir",
        _ => sex.ToString()
    };

    public static string FormatMaritalStatus(MaritalStatus status) => status switch
    {
        MaritalStatus.Single => "Soltero/a",
        MaritalStatus.Married => "Casado/a",
        MaritalStatus.Divorced => "Divorciado/a",
        MaritalStatus.Widowed => "Viudo/a",
        MaritalStatus.Separated => "Separado/a",
        MaritalStatus.CivilUnion => "Unión civil",
        _ => status.ToString()
    };

    public static string FormatRole(string role) => role switch
    {
        Auth.AppRoles.Patient => "Paciente",
        Auth.AppRoles.Doctor => "Médico",
        Auth.AppRoles.Administrator => "Administrador",
        _ => role
    };
}
