using CalendarBooking.Models;
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
    /// <summary>
    /// DateTimeUtilityService - Helper class for date related logic implementation
    /// </summary>
    public class DateTimeUtilityService : IDateTimeUtilityService
    {
        private const string DATE_FORMAT = "dd/MM/yyyy HH:mm:ss";
        private const string DAY_FORMAT = "dd";
        private const string MONTH_FORMAT = "MM";
        private const string YEAR_FORMAT = "yyyy";
        private const string DEFAULT_SECONDS = "00";
        private const string FIRST_DAY = "01";
        private const string BOOKING_DAY_START_TIME = "09:00:00";
        private const string BOOKING_DAY_END_TIME = "17:00:00";

        /// <summary>
        /// Determinse if the povided date IsSecondDayOfThirdWeek
        /// </summary>
        /// <param name="date"></param>
        /// <param name="month"></param>
        /// <param name="year"></param>
        /// <param name="hours"></param>
        /// <param name="minutes"></param>
        /// <returns></returns>
        public bool IsSecondDayOfThirdWeek(string date, string month, string year, string hours, string minutes)
        {
            var result = false;
            DateTime inputDate = DateTime.ParseExact($"{date}/{month}/{year} {hours}:{minutes}:{DEFAULT_SECONDS}", DATE_FORMAT, CultureInfo.InvariantCulture);
            DateTime inputFirstDate = DateTime.ParseExact($"{FIRST_DAY}/{month}/{year} {hours}:{minutes}:{DEFAULT_SECONDS}", DATE_FORMAT, CultureInfo.InvariantCulture);
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

        /// <summary>
        /// Determinse if the povided time IsTimeWithInDayAppointmentsWindow
        /// </summary>
        /// <param name="periodStart"></param>
        /// <param name="periodEnd"></param>
        /// <returns></returns>
        public bool IsTimeWithInDayAppointmentsWindow(DateTime periodStart, DateTime periodEnd)
        {
            var result = false;
            DateTime dayBookingStart = DateTime.ParseExact($"{periodStart.ToString(DAY_FORMAT)}/{periodStart.ToString(MONTH_FORMAT)}/{periodStart.ToString(YEAR_FORMAT)} {BOOKING_DAY_START_TIME}", DATE_FORMAT, CultureInfo.InvariantCulture);
            DateTime dayBookingEnd = DateTime.ParseExact($"{periodStart.ToString(DAY_FORMAT)}/{periodStart.ToString(MONTH_FORMAT)}/{periodStart.ToString(YEAR_FORMAT)} {BOOKING_DAY_END_TIME}", DATE_FORMAT, CultureInfo.InvariantCulture);
            if ((periodStart >= dayBookingStart && periodStart <= dayBookingEnd) && (periodEnd >= dayBookingStart && periodEnd <= dayBookingEnd))
            {
                result = true;
            }
            return result;
        }
    }
}