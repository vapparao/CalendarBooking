﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.Runtime.InteropServices;

namespace CalendarBooking
{
    internal class Program
    {
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
                    services.AddTransient<ICalendarBookingService, CalendarBookingService>();
                })
                .UseSerilog()
                .Build();
            var repository = ActivatorUtilities.CreateInstance<BookingRepository>(host.Services);
            var svc = ActivatorUtilities.CreateInstance<CalendarBookingService>(host.Services);
            svc.Run();
        }

        private static void BuildConfig(IConfigurationBuilder builder)
        {
            builder.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "production"}.json", optional: true)
                .AddEnvironmentVariables();
        }
    }
}