using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NodaTime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CalendarBooking
{
    public class CalendarBookingService : ICalendarBookingService
    {
        private readonly ILogger<CalendarBookingService> _log;
        private readonly IBooking _bookingRepository;

        public CalendarBookingService(ILogger<CalendarBookingService> log, IBooking bookingRepository)
        {
            _log = log;
            _bookingRepository = bookingRepository;
        }

        public void Run(string optionalInput = "")
        {
            //User Input Helper Statements

            _log.LogInformation("Running {cbService}", "CalendarBookingService");
            _log.LogInformation("Please Enter {addCommand} to add calendar booking", "ADD DD/MM hh:mm");
            _log.LogInformation("Please Enter {reserveCommand} to reserve calendar booking", "KEEP hh:mm");
            _log.LogInformation("Please Enter {findCommand} to find free calendar booking slot", "FIND DD/MM");
            _log.LogInformation("Please Enter {deleteCommand} to delete calendar booking", "DELETE DD/MM hh:mm");

            DateTimeZone tz = DateTimeZoneProviders.Tzdb.GetSystemDefault();
            var model = new BookingModel();

            //User Input read and processing

            string inputString = optionalInput.Length > 0 ? optionalInput : Console.ReadLine() ?? "";
            var command = inputString?.Split(" ")[0];
            string dd = $"{DateTime.Now.ToString("dd")}", mon = $"{DateTime.Now.ToString("MM")}", yyyy = $"{DateTime.Now.ToString("yyyy")}", hh = "00", mm = "00";
            if (command == "ADD")
            {
                try
                {
                    dd = inputString.Split(" ")[1].Split("/")[0];
                    mon = inputString.Split(" ")[1].Split("/")[1];
                    hh = inputString.Split(" ")[2].Split(":")[0];
                    mm = inputString.Split(" ")[2].Split(":")[1];
                    DateTime inputDate = DateTime.ParseExact($"{dd}/{mon}/{yyyy} {hh}:{mm}:00", "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                    Instant start = Instant.FromDateTimeUtc(DateTime.SpecifyKind(inputDate, DateTimeKind.Utc));
                    Instant end = start.Plus(Duration.FromMinutes(30));
                    start.InZone(tz);
                    end.InZone(tz);
                    model.PeriodStart = start.ToDateTimeUtc();
                    model.PeriodEnd = end.ToDateTimeUtc();
                    model.Status = "Added";

                    _log.LogInformation("Add Booking Requested");

                    // Perform Add
                    PerformAdd(model, dd, mon, yyyy, hh, mm);
                }
                catch (Exception ex)
                {
                    _log.LogInformation("Invalid Input - Please enter valid data for {command} command", command);
                }
            }
            else if (command == "DELETE")
            {
                try
                {
                    dd = inputString.Split(" ")[1].Split("/")[0];
                    mon = inputString.Split(" ")[1].Split("/")[1];
                    hh = inputString.Split(" ")[2].Split(":")[0];
                    mm = inputString.Split(" ")[2].Split(":")[1];
                    DateTime inputDate = DateTime.ParseExact($"{dd}/{mon}/{yyyy} {hh}:{mm}:00", "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                    Instant start = Instant.FromDateTimeUtc(DateTime.SpecifyKind(inputDate, DateTimeKind.Utc));
                    Instant end = start.Plus(Duration.FromMinutes(30));
                    start.InZone(tz);
                    end.InZone(tz);
                    model.PeriodStart = start.ToDateTimeUtc();
                    model.PeriodEnd = end.ToDateTimeUtc();
                    model.Status = "Added";
                    _log.LogInformation("Delete Booking Requested");

                    // Perform Delete

                    PerformDelete(model, dd, mon, yyyy, hh, mm);
                }
                catch (Exception ex)
                {
                    _log.LogInformation("Invalid Input - Please enter valid data for {command} command", command);
                }
            }
            else if (command == "KEEP")
            {
                try
                {
                    hh = inputString.Split(" ")[1].Split(":")[0];
                    mm = inputString.Split(" ")[1].Split(":")[1];
                    DateTime inputDate = DateTime.ParseExact($"{dd}/{mon}/{yyyy} {hh}:{mm}:00", "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                    Instant start = Instant.FromDateTimeUtc(DateTime.SpecifyKind(inputDate, DateTimeKind.Utc));
                    Instant end = start.Plus(Duration.FromMinutes(30));
                    start.InZone(tz);
                    end.InZone(tz);
                    model.PeriodStart = start.ToDateTimeUtc();
                    model.PeriodEnd = end.ToDateTimeUtc();
                    model.Status = "Added";
                    _log.LogInformation("Reserve Booking Requested");

                    // Perform Reserve

                    PeformReserve(model, dd, mon, yyyy, hh, mm);
                }
                catch (Exception ex)
                {
                    _log.LogInformation("Invalid Input - Please enter valid data for {command} command", command);
                }
            }
            else if (command == "FIND")
            {
                try
                {
                    dd = inputString.Split(" ")[1].Split("/")[0];
                    mon = inputString.Split(" ")[1].Split("/")[1];
                    DateTime inputDate = DateTime.ParseExact($"{dd}/{mon}/{yyyy} {hh}:{mm}:00", "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                    Instant start = Instant.FromDateTimeUtc(DateTime.SpecifyKind(inputDate, DateTimeKind.Utc));
                    Instant end = start.Plus(Duration.FromMinutes(30));
                    start.InZone(tz);
                    end.InZone(tz);
                    model.PeriodStart = start.ToDateTimeUtc();
                    model.PeriodEnd = end.ToDateTimeUtc();
                    model.Status = "Added";
                    _log.LogInformation("Find Booking Slots Requested");
                    PerformFind(model, dd, mon);
                }
                catch (Exception ex)
                {
                    _log.LogInformation("Invalid Input - Please enter valid data for {command} command", command);
                }
            }
            else
            {
                _log.LogInformation("Invalid Input - Please enter any of the above listed commands");
            }
            _log.LogInformation("Input String {input}", inputString);
        }

        private void PerformFind(BookingModel model, string dd, string mon)
        {
            var result = this.FindBookings(model.PeriodStart, model.PeriodEnd).GetAwaiter().GetResult();
            if (result.Count() > 0)
            {
                _log.LogInformation("No Bookings available for {periodStart} - {periodEnd}", model.PeriodStart, model.PeriodEnd);
            }
            else
            {
                _log.LogInformation("Booking slot available for {periodStart}  -  {periodEnd}", $"{dd}/{mon} 09:00", $"{dd}/{mon} 09:30");
            }
        }

        private void PeformReserve(BookingModel model, string dd, string mon, string yyyy, string hh, string mm)
        {
            if (this.IsSecondDayOfThirdWeek(dd, mon, yyyy, hh, mm))
            {
                _log.LogInformation("No Bookings available for {periodStart} - {periodEnd}", model.PeriodStart, model.PeriodEnd);
            }
            else
            {
                // Check for existing booking
                var getResult = this.GetBooking(model).GetAwaiter().GetResult();
                if (getResult > 0)
                {
                    _log.LogInformation("No Bookings available for {periodStart} - {periodEnd}", model.PeriodStart, model.PeriodEnd);
                }
                else
                {
                    model.Status = "Reserved";
                    var result = this.AddBooking(model).GetAwaiter().GetResult();
                    _log.LogInformation("Booking for {periodStart} - {periodEnd}  {status}", result.PeriodStart, result.PeriodEnd, result.Status);
                }
            }
        }

        private void PerformDelete(BookingModel model, string dd, string mon, string yyyy, string hh, string mm)
        {
            if (this.IsSecondDayOfThirdWeek(dd, mon, yyyy, hh, mm))
            {
                _log.LogInformation("No Bookings available for {periodStart} - {periodEnd}", model.PeriodStart, model.PeriodEnd);
            }
            else
            {
                // Check for existing booking

                var getResult = this.GetBooking(model).GetAwaiter().GetResult();
                if (getResult > 0)
                {
                    var result = this.DeleteBooking(model).GetAwaiter().GetResult();
                    _log.LogInformation("Booking for {periodStart} - {periodEnd}  successfully deleted", result.PeriodStart, result.PeriodEnd);
                }
                else
                {
                    _log.LogInformation("No Bookings available for {periodStart} - {periodEnd}", model.PeriodStart, model.PeriodEnd);
                }
            }
        }

        private void PerformAdd(BookingModel model, string dd, string mon, string yyyy, string hh, string mm)
        {
            if (this.IsSecondDayOfThirdWeek(dd, mon, yyyy, hh, mm))
            {
                _log.LogInformation("No Bookings available for {periodStart} - {periodEnd}", model.PeriodStart, model.PeriodEnd);
            }
            else
            {
                // Check for existing booking
                var getResult = this.GetBooking(model).GetAwaiter().GetResult();
                if (getResult > 0)
                {
                    _log.LogInformation("No Bookings available for {periodStart} - {periodEnd}", model.PeriodStart, model.PeriodEnd);
                }
                else
                {
                    var result = this.AddBooking(model).GetAwaiter().GetResult();
                    _log.LogInformation("Booking for {periodStart} - {periodEnd}  {status}", result.PeriodStart, result.PeriodEnd, result.Status);
                }
            }
        }

        public async Task<BookingModel> AddBooking(BookingModel model)
        {
            var result = await _bookingRepository.Add(model);
            return result;
        }

        public async Task<BookingModel> DeleteBooking(BookingModel model)
        {
            var result = await _bookingRepository.Remove(model);
            return result;
        }

        public async Task<int> GetBooking(BookingModel model)
        {
            var result = await _bookingRepository.Get(model);
            return result;
        }

        public async Task<IEnumerable<BookingModel>> FindBookings(DateTime start, DateTime end)
        {
            var result = await _bookingRepository.Find(start, end);
            return result;
        }

        private bool IsSecondDayOfThirdWeek(string date, string month, string year, string hours, string minutes)
        {
            var result = false;
            DateTime inputDate = DateTime.ParseExact($"{date}/{month}/{year} {hours}:{minutes}:00", "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            DateTime firstDateInputMonth = DateTime.ParseExact($"01/{month}/{year} 00:00:00", "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            Instant inputInstance = Instant.FromDateTimeUtc(DateTime.SpecifyKind(inputDate, DateTimeKind.Utc));
            Instant secondDayThirdWeekPeriodStart = Instant.FromDateTimeUtc(DateTime.SpecifyKind(firstDateInputMonth, DateTimeKind.Utc)).Plus(Duration.FromDays(15)).Plus(Duration.FromHours(16));
            Instant secondDayThirdWeekPeriodEnd = Instant.FromDateTimeUtc(DateTime.SpecifyKind(firstDateInputMonth, DateTimeKind.Utc)).Plus(Duration.FromDays(15)).Plus(Duration.FromHours(17));
            DateTimeZone tz = DateTimeZoneProviders.Tzdb.GetSystemDefault();
            inputInstance.InZone(tz);
            secondDayThirdWeekPeriodStart.InZone(tz);
            secondDayThirdWeekPeriodEnd.InZone(tz);
            var result1 = secondDayThirdWeekPeriodStart.CompareTo(inputInstance);
            var result2 = secondDayThirdWeekPeriodEnd.CompareTo(inputInstance);
            result = result1 <= 0 && result2 >= 0;
            return result;
        }
    }
}