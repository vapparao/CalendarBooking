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
    /// <summary>
    /// CalendarBookingServices - Manages calendar booking operations
    /// </summary>
    public class CalendarBookingService : ICalendarBookingService
    {
        private readonly IBooking _bookingRepository;

        /// <summary>
        /// Constructor - Initializes CalendarBookingService
        /// </summary>
        /// <param name="bookingRepository"></param>
        public CalendarBookingService(IBooking bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        /// <summary>
        /// Performs Find operation
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<IEnumerable<BookingModel>> PerformFind(BookingModel model)
        {
            return await _bookingRepository.Find(model.PeriodStart, model.PeriodEnd);
        }

        /// <summary>
        /// Performs Keep operation
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<BookingModel> PeformReserve(BookingModel model)
        {
            return await _bookingRepository.Add(model);
        }

        /// <summary>
        /// Performs Delete operation
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<BookingModel> PeformDelete(BookingModel model)
        {
            return await _bookingRepository.Remove(model);
        }

        /// <summary>
        /// Performs Get operation
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<int> GetBooking(BookingModel model)
        {
            var result = await _bookingRepository.Get(model);
            return result;
        }

        /// <summary>
        /// Performs Add operation
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<BookingModel> PeformAdd(BookingModel model)
        {
            return await _bookingRepository.Add(model);
        }
    }
}