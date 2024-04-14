using CalendarBooking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalendarBooking.Services.Utilities
{
    public interface IDateTimeUtilityService
    {
        public bool IsSecondDayOfThirdWeek(string date, string month, string year, string hours, string minutes);

        public bool IsTimeWithInDayAppointmentsWindow(DateTime periodStart, DateTime periodEnd);
    }
}