using Azure;
using CalendarBooking.Models;
using CalendarBooking.Services.Utilities;
using DbUp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CalendarBooking.Services
{
    /// <summary>
    /// BookingUIService
    ///  - Accepts user input
    ///  - Parses user input
    ///  - Initiates database creation
    ///  - Processes commands
    ///  - Responds appropriately to respective commands
    /// </summary>
    public class BookingUIService : IBookingUIService
    {
        private readonly ILogger<BookingUIService> _log;
        private readonly IConfiguration _config;
        private readonly ICalendarBookingService _bookingService;
        private readonly IDateTimeUtilityService _dateTimeUtilityService;
        private const string EMPTY = "";
        private const string CONNECTION_STRING = "DefaultConnection";
        private const string SUCCESS_MSG = "Success!";
        private const string DATE_FORMAT = "dd/MM/yyyy HH:mm:ss";
        private const string DAY_FORMAT = "dd";
        private const string MONTH_FORMAT = "MM";
        private const string YEAR_FORMAT = "yyyy";
        private const string DEFAULT_SECONDS = "00";
        private const string SPACE_STRING = " ";
        private const string SLASH_STRING = "/";
        private const string COLON_STRING = ":";
        private const string COMMAND_ADD = "ADD";
        private const string COMMAND_KEEP = "KEEP";
        private const string COMMAND_DELETE = "DELETE";
        private const string COMMAND_FIND = "FIND";
        private const string ADDED = "Added";
        private const string RESERVED = "Reserved";
        private const string BOOKING_DAY_START_TIME = "09:00:00";
        private const string BOOKING_DAY_START_END = "09:30:00";

        /// <summary>
        /// Constructor - Initializes BookingUIService
        /// </summary>
        /// <param name="log"></param>
        /// <param name="config"></param>
        /// <param name="bookingService"></param>
        /// <param name="dateTimeUtilityService"></param>
        public BookingUIService(ILogger<BookingUIService> log, IConfiguration config, ICalendarBookingService bookingService, IDateTimeUtilityService dateTimeUtilityService)
        {
            _log = log;
            _config = config;
            _bookingService = bookingService;
            _dateTimeUtilityService = dateTimeUtilityService;
        }

        /// <summary>
        /// Entry method - Processes user commands
        /// </summary>
        /// <param name="optionalInput"></param>
        public void Run(string optionalInput = EMPTY)
        {
            if (optionalInput.Length == 0)
            {
                // Database initialization
                var connectionString = _config.GetConnectionString(CONNECTION_STRING);
                var upgrader =
                DeployChanges.To
                    .SqlDatabase(connectionString)
                    .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                    .LogToConsole()
                    .Build();
                var dbResult = upgrader.PerformUpgrade();

                if (!dbResult.Successful)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(dbResult.Error);
                    Console.ResetColor();
                }
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(SUCCESS_MSG);
                Console.ResetColor();
            }

            //User Input Helper Statements

            _log.LogInformation("Running {cbService}", "CalendarBookingService");
            _log.LogInformation("Please Enter {addCommand} to add calendar booking", "ADD DD/MM hh:mm");
            _log.LogInformation("Please Enter {reserveCommand} to reserve calendar booking", "KEEP hh:mm");
            _log.LogInformation("Please Enter {findCommand} to find free calendar booking slot", "FIND DD/MM");
            _log.LogInformation("Please Enter {deleteCommand} to delete calendar booking", "DELETE DD/MM hh:mm");

            DateTimeZone tz = DateTimeZoneProviders.Tzdb.GetSystemDefault();
            var model = new BookingModel();

            //User Input read and processing

            string inputString = optionalInput.Length > 0 ? optionalInput : Console.ReadLine() ?? EMPTY;

            _log.LogInformation("Provided Input {input}", inputString);

            var command = inputString?.Split(" ")[0];
            string dd = $"{DateTime.Now.ToString(DAY_FORMAT)}", mon = $"{DateTime.Now.ToString(MONTH_FORMAT)}", yyyy = $"{DateTime.Now.ToString(YEAR_FORMAT)}", hh = DEFAULT_SECONDS, mm = DEFAULT_SECONDS;
            if (command == COMMAND_ADD)
            {
                try
                {
                    // ADD command input processing
                    if (inputString is not null)
                    {
                        dd = inputString.Split(SPACE_STRING)[1].Split(SLASH_STRING)[0];
                        mon = inputString.Split(SPACE_STRING)[1].Split(SLASH_STRING)[1];
                        hh = inputString.Split(SPACE_STRING)[2].Split(COLON_STRING)[0];
                        mm = inputString.Split(SPACE_STRING)[2].Split(COLON_STRING)[1];
                    }
                    DateTime inputDate = DateTime.ParseExact($"{dd}/{mon}/{yyyy} {hh}:{mm}:{DEFAULT_SECONDS}", DATE_FORMAT, CultureInfo.InvariantCulture);
                    Instant start = Instant.FromDateTimeUtc(DateTime.SpecifyKind(inputDate, DateTimeKind.Utc));
                    Instant end = start.Plus(Duration.FromMinutes(30));
                    start.InZone(tz);
                    end.InZone(tz);
                    model.PeriodStart = start.ToDateTimeUtc();
                    model.PeriodEnd = end.ToDateTimeUtc();
                    model.Status = ADDED;

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
            else if (command == COMMAND_DELETE)
            {
                try
                {
                    // DELETE command input processing
                    if (inputString is not null)
                    {
                        dd = inputString.Split(SPACE_STRING)[1].Split(SLASH_STRING)[0];
                        mon = inputString.Split(SPACE_STRING)[1].Split(SLASH_STRING)[1];
                        hh = inputString.Split(SPACE_STRING)[2].Split(COLON_STRING)[0];
                        mm = inputString.Split(SPACE_STRING)[2].Split(COLON_STRING)[1];
                    }
                    DateTime inputDate = DateTime.ParseExact($"{dd}/{mon}/{yyyy} {hh}:{mm}:{DEFAULT_SECONDS}", DATE_FORMAT, CultureInfo.InvariantCulture);
                    Instant start = Instant.FromDateTimeUtc(DateTime.SpecifyKind(inputDate, DateTimeKind.Utc));
                    Instant end = start.Plus(Duration.FromMinutes(30));
                    start.InZone(tz);
                    end.InZone(tz);
                    model.PeriodStart = start.ToDateTimeUtc();
                    model.PeriodEnd = end.ToDateTimeUtc();
                    model.Status = ADDED;
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
                            model.Id = getResult;
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
            else if (command == COMMAND_KEEP)
            {
                try
                {
                    // KEEP command input processing
                    if (inputString is not null)
                    {
                        hh = inputString.Split(SPACE_STRING)[1].Split(COLON_STRING)[0];
                        mm = inputString.Split(SPACE_STRING)[1].Split(COLON_STRING)[1];
                    }
                    DateTime inputDate = DateTime.ParseExact($"{dd}/{mon}/{yyyy} {hh}:{mm}:{DEFAULT_SECONDS}", DATE_FORMAT, CultureInfo.InvariantCulture);
                    Instant start = Instant.FromDateTimeUtc(DateTime.SpecifyKind(inputDate, DateTimeKind.Utc));
                    Instant end = start.Plus(Duration.FromMinutes(30));
                    start.InZone(tz);
                    end.InZone(tz);
                    model.PeriodStart = start.ToDateTimeUtc();
                    model.PeriodEnd = end.ToDateTimeUtc();
                    model.Status = ADDED;
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
                                model.Status = RESERVED;
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
            else if (command == COMMAND_FIND)
            {
                try
                {
                    // FIND command input processing
                    if (inputString is not null)
                    {
                        dd = inputString.Split(SPACE_STRING)[1].Split(SLASH_STRING)[0];
                        mon = inputString.Split(SPACE_STRING)[1].Split(SLASH_STRING)[1];
                        hh = "09";
                        mm = "00";
                    }
                    DateTime inputDate = DateTime.ParseExact($"{dd}/{mon}/{yyyy} {hh}:{mm}:{DEFAULT_SECONDS}", DATE_FORMAT, CultureInfo.InvariantCulture);
                    Instant start = Instant.FromDateTimeUtc(DateTime.SpecifyKind(inputDate, DateTimeKind.Utc));
                    Instant end = start.Plus(Duration.FromMinutes(30));
                    start.InZone(tz);
                    end.InZone(tz);
                    model.PeriodStart = start.ToDateTimeUtc();
                    model.PeriodEnd = end.ToDateTimeUtc();
                    model.Status = ADDED;
                    _log.LogInformation("Find Booking Slots Requested");
                    var result = _bookingService.PerformFind(model).GetAwaiter().GetResult();
                    if (result.Count() > 0)
                    {
                        var findResult = _dateTimeUtilityService.FindFreeBooking(model.PeriodStart, model.PeriodEnd, result);
                        if (findResult.IsBooked)
                        {
                            _log.LogInformation("Response: No Bookings available for {periodStart} - {periodEnd}", model.PeriodStart, model.PeriodEnd);
                        }
                        else
                        {
                            _log.LogInformation("Response: Booking slot available for {periodStart}  -  {periodEnd}", findResult.PeriodStart, findResult.PeriodEnd);
                        }
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