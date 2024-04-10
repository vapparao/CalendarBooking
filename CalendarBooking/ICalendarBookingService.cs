namespace CalendarBooking
{
    public interface ICalendarBookingService
    {
        void Run(string optionalInput = "");

        Task<BookingModel> AddBooking(BookingModel model);

        Task<BookingModel> DeleteBooking(BookingModel model);

        Task<IEnumerable<BookingModel>> FindBookings(DateTime start, DateTime emd);

        Task<int> GetBooking(BookingModel model);
    }
}