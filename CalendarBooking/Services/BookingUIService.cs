using CalendarBooking.Models;
using CalendarBooking.Services.Utilities;
using Microsoft.Extensions.Logging;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalendarBooking.Services
{
    public class BookingUIService : IBookingUIService
    {
        private readonly ILogger<BookingUIService> _log;
        private readonly ICalendarBookingService _bookingService;
        private readonly IDateTimeUtilityService _dateTimeUtilityService;

        public BookingUIService(ILogger<BookingUIService> log, ICalendarBookingService bookingService, IDateTimeUtilityService dateTimeUtilityService)
        {
            _log = log;
            _bookingService = bookingService;
            _dateTimeUtilityService = dateTimeUtilityService;
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

            _log.LogInformation("Provided Input {input}", inputString);

            var command = inputString?.Split(" ")[0];
            string dd = $"{DateTime.Now.ToString("dd")}", mon = $"{DateTime.Now.ToString("MM")}", yyyy = $"{DateTime.Now.ToString("yyyy")}", hh = "00", mm = "00";
            if (command == "ADD")
            {
                try
                {
                    // ADD command input processing
                    if (inputString is not null)
                    {
                        dd = inputString.Split(" ")[1].Split("/")[0];
                        mon = inputString.Split(" ")[1].Split("/")[1];
                        hh = inputString.Split(" ")[2].Split(":")[0];
                        mm = inputString.Split(" ")[2].Split(":")[1];
                    }
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
                    if (_dateTimeUtilityService.IsSecondDayOfThirdWeek(dd, mon, yyyy, hh, mm))
                    {
                        _log.LogInformation("Response: No Bookings available for {periodStart} - {periodEnd}", model.PeriodStart, model.PeriodEnd);
                    }
                    else
                    {
                        // Check for existing booking
                        var getResult = _bookingService.GetBooking(model).GetAwaiter().GetResult();
                        if (getResult > 0)
                        {
                            _log.LogInformation("Response: No Bookings available for {periodStart} - {periodEnd}", model.PeriodStart, model.PeriodEnd);
                        }
                        else
                        {
                            if (_dateTimeUtilityService.IsTimeWithInDayAppointmentsWindow(model.PeriodStart, model.PeriodEnd))
                            {
                                var result = _bookingService.PeformAdd(model).GetAwaiter().GetResult();
                                _log.LogInformation("Response: Booking for {periodStart} - {periodEnd}  {status}", result.PeriodStart, result.PeriodEnd, result.Status);
                            }
                            else
                            {
                                _log.LogInformation("Response: Invalid Input - Booking time is outside {command} ", "9 AM and 5 PM");
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    _log.LogInformation("Response: Invalid Input - Please enter valid data for {command} command", command);
                }
            }
            else if (command == "DELETE")
            {
                try
                {
                    // DELETE command input processing
                    if (inputString is not null)
                    {
                        dd = inputString.Split(" ")[1].Split("/")[0];
                        mon = inputString.Split(" ")[1].Split("/")[1];
                        hh = inputString.Split(" ")[2].Split(":")[0];
                        mm = inputString.Split(" ")[2].Split(":")[1];
                    }
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
                    if (_dateTimeUtilityService.IsSecondDayOfThirdWeek(dd, mon, yyyy, hh, mm))
                    {
                        _log.LogInformation("Response: No Bookings available for {periodStart} - {periodEnd}", model.PeriodStart, model.PeriodEnd);
                    }
                    else
                    {
                        // Check for existing booking
                        var getResult = _bookingService.GetBooking(model).GetAwaiter().GetResult();
                        if (getResult > 0)
                        {
                            var result = _bookingService.PeformDelete(model).GetAwaiter().GetResult();
                            _log.LogInformation("Response: Booking for {periodStart} - {periodEnd}  successfully deleted", result.PeriodStart, result.PeriodEnd);
                        }
                        else
                        {
                            _log.LogInformation("Response: No Bookings available for {periodStart} - {periodEnd}", model.PeriodStart, model.PeriodEnd);
                        }
                    }
                }
                catch (Exception)
                {
                    _log.LogInformation("Response: Invalid Input - Please enter valid data for {command} command", command);
                }
            }
            else if (command == "KEEP")
            {
                try
                {
                    // KEEP command input processing
                    if (inputString is not null)
                    {
                        hh = inputString.Split(" ")[1].Split(":")[0];
                        mm = inputString.Split(" ")[1].Split(":")[1];
                    }
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
                    if (_dateTimeUtilityService.IsSecondDayOfThirdWeek(dd, mon, yyyy, hh, mm))
                    {
                        _log.LogInformation("Response: No Bookings available for {periodStart} - {periodEnd}", model.PeriodStart, model.PeriodEnd);
                    }
                    else
                    {
                        // Check for existing booking
                        var getResult = _bookingService.GetBooking(model).GetAwaiter().GetResult();
                        if (getResult > 0)
                        {
                            _log.LogInformation("Response: No Bookings available for {periodStart} - {periodEnd}", model.PeriodStart, model.PeriodEnd);
                        }
                        else
                        {
                            if (_dateTimeUtilityService.IsTimeWithInDayAppointmentsWindow(model.PeriodStart, model.PeriodEnd))
                            {
                                model.Status = "Reserved";
                                var result = _bookingService.PeformReserve(model).GetAwaiter().GetResult();
                                _log.LogInformation("Response: Booking for {periodStart} - {periodEnd}  {status}", result.PeriodStart, result.PeriodEnd, result.Status);
                            }
                            else
                            {
                                _log.LogInformation("Response: Invalid Input - Booking time is outside {command} ", "9 AM and 5 PM");
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    _log.LogInformation("Response: Invalid Input - Please enter valid data for {command} command", command);
                }
            }
            else if (command == "FIND")
            {
                try
                {
                    // FIND command input processing
                    if (inputString is not null)
                    {
                        dd = inputString.Split(" ")[1].Split("/")[0];
                        mon = inputString.Split(" ")[1].Split("/")[1];
                    }
                    DateTime inputDate = DateTime.ParseExact($"{dd}/{mon}/{yyyy} {hh}:{mm}:00", "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                    Instant start = Instant.FromDateTimeUtc(DateTime.SpecifyKind(inputDate, DateTimeKind.Utc));
                    Instant end = start.Plus(Duration.FromMinutes(30));
                    start.InZone(tz);
                    end.InZone(tz);
                    model.PeriodStart = start.ToDateTimeUtc();
                    model.PeriodEnd = end.ToDateTimeUtc();
                    model.Status = "Added";
                    _log.LogInformation("Find Booking Slots Requested");
                    var result = _bookingService.PerformFind(model).GetAwaiter().GetResult();
                    if (result.Count() > 0)
                    {
                        _log.LogInformation("Response: No Bookings available for {periodStart} - {periodEnd}", model.PeriodStart, model.PeriodEnd);
                    }
                    else
                    {
                        _log.LogInformation("Response: Booking slot available for {periodStart}  -  {periodEnd}", $"{dd}/{mon} 09:00", $"{dd}/{mon} 09:30");
                    }
                }
                catch (Exception)
                {
                    _log.LogInformation("Response: Invalid Input - Please enter valid data for {command} command", command);
                }
            }
            else
            {
                _log.LogInformation("Response: Invalid Input - Please enter any of the above listed commands");
            }
        }
    }
}