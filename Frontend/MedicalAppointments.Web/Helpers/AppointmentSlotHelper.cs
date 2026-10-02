using MedicalAppointments.Web.Models.Availability;

namespace MedicalAppointments.Web.Helpers;

public static class AppointmentSlotHelper
{
    public static IReadOnlyList<DateOnly> GetAvailableDates(IEnumerable<DoctorAvailabilityDto> slots) =>
        slots.Select(s => s.Date).Distinct().OrderBy(d => d).ToList();

    public static IReadOnlyList<DateTimeOffset> GetTimesForDate(
        IEnumerable<DoctorAvailabilityDto> slots,
        DateOnly date,
        TimeSpan step)
    {
        var times = new List<DateTimeOffset>();
        foreach (var slot in slots.Where(s => s.Date == date))
        {
            var cursor = slot.StartTime;
            while (cursor < slot.EndTime)
            {
                var next = cursor.Add(step);
                if (next > slot.EndTime)
                {
                    break;
                }

                var utc = new DateTime(date.Year, date.Month, date.Day, cursor.Hour, cursor.Minute, 0, DateTimeKind.Utc);
                times.Add(new DateTimeOffset(utc, TimeSpan.Zero));
                cursor = next;
            }
        }

        return times.OrderBy(t => t).ToList();
    }
}
