using NodaTime;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CalendarBooking.Services.Utilities
{
    public class DateTimeUtilityService : IDateTimeUtilityService
    {
        public bool IsSecondDayOfThirdWeek(string date, string month, string year, string hours, string minutes)
        {
            var result = false;
            DateTime inputDate = DateTime.ParseExact($"{date}/{month}/{year} {hours}:{minutes}:00", "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            DateTime inputFirstDate = DateTime.ParseExact($"01/{month}/{year} {hours}:{minutes}:00", "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            var firstMondayDateOfMonth = LocalDate.FromYearMonthWeekAndDay(inputDate.Year, inputDate.Month, 1, IsoDayOfWeek.Monday);
            var previousModayDate = firstMondayDateOfMonth.PlusDays(-7);
            if (previousModayDate.Month != firstMondayDateOfMonth.Month)
            {
                int days = Period.DaysBetween(previousModayDate, LocalDate.FromDateTime(inputFirstDate));
                if (days < 7)
                {
                    firstMondayDateOfMonth = previousModayDate;
                }
            }
            Instant inputInstance = Instant.FromDateTimeUtc(DateTime.SpecifyKind(inputDate, DateTimeKind.Utc));

            Instant secondDayThirdWeekPeriodStart = Instant.FromDateTimeUtc(DateTime.SpecifyKind(firstMondayDateOfMonth.ToDateTimeUnspecified(), DateTimeKind.Utc)).Plus(Duration.FromDays(15)).Plus(Duration.FromHours(16));
            Instant secondDayThirdWeekPeriodEnd = Instant.FromDateTimeUtc(DateTime.SpecifyKind(firstMondayDateOfMonth.ToDateTimeUnspecified(), DateTimeKind.Utc)).Plus(Duration.FromDays(15)).Plus(Duration.FromHours(17));
            DateTimeZone tz = DateTimeZoneProviders.Tzdb.GetSystemDefault();
            inputInstance.InZone(tz);
            secondDayThirdWeekPeriodStart.InZone(tz);
            secondDayThirdWeekPeriodEnd.InZone(tz);
            var result1 = secondDayThirdWeekPeriodStart.CompareTo(inputInstance);
            var result2 = secondDayThirdWeekPeriodEnd.CompareTo(inputInstance);
            result = result1 <= 0 && result2 >= 0;
            return result;
        }

        public bool IsTimeWithInDayAppointmentsWindow(DateTime periodStart, DateTime periodEnd)
        {
            var result = false;
            DateTime dayBookingStart = DateTime.ParseExact($"{periodStart.ToString("dd")}/{periodStart.ToString("MM")}/{periodStart.ToString("yyyy")} 09:00:00", "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            DateTime dayBookingEnd = DateTime.ParseExact($"{periodStart.ToString("dd")}/{periodStart.ToString("MM")}/{periodStart.ToString("yyyy")} 17:00:00", "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            if ((periodStart >= dayBookingStart && periodStart <= dayBookingEnd) && (periodEnd >= dayBookingStart && periodEnd <= dayBookingEnd))
            {
                result = true;
            }
            return result;
        }
    }
}