using CalendarBooking.Data;
using CalendarBooking.Models;
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

namespace CalendarBooking.Services
{
    public class CalendarBookingService : ICalendarBookingService
    {
        private readonly IBooking _bookingRepository;

        public CalendarBookingService(IBooking bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        public async Task<IEnumerable<BookingModel>> PerformFind(BookingModel model)
        {
            return await _bookingRepository.Find(model.PeriodStart, model.PeriodEnd);
        }

        public async Task<BookingModel> PeformReserve(BookingModel model)
        {
            return await _bookingRepository.Add(model);
        }

        public async Task<BookingModel> PeformDelete(BookingModel model)
        {
            return await _bookingRepository.Remove(model);
        }

        public async Task<int> GetBooking(BookingModel model)
        {
            var result = await _bookingRepository.Get(model);
            return result;
        }

        public async Task<BookingModel> PeformAdd(BookingModel model)
        {
            return await _bookingRepository.Add(model);
        }
    }
}