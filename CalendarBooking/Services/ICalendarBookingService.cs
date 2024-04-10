using CalendarBooking.Models;

namespace CalendarBooking.Services
{
    public interface ICalendarBookingService
    {
        Task<BookingModel> PeformAdd(BookingModel model);

        Task<BookingModel> PeformReserve(BookingModel model);

        Task<BookingModel> PeformDelete(BookingModel model);

        Task<IEnumerable<BookingModel>> PerformFind(BookingModel model);

        Task<int> GetBooking(BookingModel model);
    }
}