using CalendarBooking.Data;
using CalendarBooking.Services;
using CalendarBooking.Services.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.Runtime.InteropServices;

namespace CalendarBooking
{
    /// <summary>
    /// Entry class - Manages host configuration
    /// </summary>
    internal class Program
    {
        protected Program()
        {
        }

        private const string ENV_PRODUCTION = "production";
        private const string ASPNETCORE_ENVIRONMENT = "ASPNETCORE_ENVIRONMENT";
        private const string CONFIG_FILE_NAME = "appsettings.json";
        private const string CONFIG_FILE_PREFIX = "appsettings";
        private const string CONFIG_FILE_EXT = ".json";

        /// <summary>
        /// Entry method - Initializes host configurations
        /// </summary>
        /// <param name="args"></param>
        private static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder();
            BuildConfig(builder);

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Build())
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            Log.Logger.Information("Calendar Booking Application Starting");

            var host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    services.AddSingleton<BookingDBContext, BookingDBContext>();
                    services.AddTransient<IBooking, BookingRepository>();
                    services.AddTransient<IDateTimeUtilityService, DateTimeUtilityService>();
                    services.AddTransient<ICalendarBookingService, CalendarBookingService>();
                    services.AddTransient<IBookingUIService, BookingUIService>();
                })
                .UseSerilog()
                .Build();
            ActivatorUtilities.CreateInstance<BookingRepository>(host.Services);
            ActivatorUtilities.CreateInstance<DateTimeUtilityService>(host.Services);
            ActivatorUtilities.CreateInstance<CalendarBookingService>(host.Services);
            var svc = ActivatorUtilities.CreateInstance<BookingUIService>(host.Services);
            svc.Run();
        }

        /// <summary>
        /// Bulid Configuration
        /// </summary>
        /// <param name="builder"></param>
        private static void BuildConfig(IConfigurationBuilder builder)
        {
            builder.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(CONFIG_FILE_NAME, optional: false, reloadOnChange: true)
                .AddJsonFile($"{CONFIG_FILE_PREFIX}.{Environment.GetEnvironmentVariable(ASPNETCORE_ENVIRONMENT) ?? ENV_PRODUCTION}{CONFIG_FILE_EXT}", optional: true)
                .AddEnvironmentVariables();
        }
    }
}