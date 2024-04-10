namespace CalendarBookingTests
{
    using CalendarBooking;
    using FakeItEasy;
    using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.ObjectModel;

    public class CalendarBookingServiceTests
    {
        private readonly ICalendarBookingService _calendarBookingService;

        public CalendarBookingServiceTests()
        {
            _calendarBookingService = A.Fake<ICalendarBookingService>();
        }

        [Fact]
        public void Test_AddBookingAddsWhenBookingAvailable()
        {
            // Arrange
            var model = new BookingModel();

            A.CallTo(() => _calendarBookingService.GetBooking(A<BookingModel>.Ignored))
             .Returns(-1);
            A.CallTo(() => _calendarBookingService.AddBooking(A<BookingModel>.Ignored))
             .Returns(model);

            // Act
            string optionalInput = "ADD 12/06 03:22";
            _calendarBookingService.Run(optionalInput);

            // Assert
            A.CallTo(() => _calendarBookingService.GetBooking(A<BookingModel>.That.IsInstanceOf(typeof(BookingModel)))).MustHaveHappened();
            A.CallTo(() => _calendarBookingService.AddBooking(A<BookingModel>.That.IsInstanceOf(typeof(BookingModel)))).MustHaveHappened();
        }

        [Fact]
        public void Test_AddBookingDoesNotAddsWhenSpecialCaseDate()
        {
            // Arrange
            var model = new BookingModel();

            A.CallTo(() => _calendarBookingService.GetBooking(A<BookingModel>.Ignored))
             .Returns(-1);
            A.CallTo(() => _calendarBookingService.AddBooking(A<BookingModel>.Ignored))
             .Returns(model);

            // Act
            string optionalInput = "ADD 16/04 16:22";
            _calendarBookingService.Run(optionalInput);

            // Assert
            A.CallTo(() => _calendarBookingService.GetBooking(A<BookingModel>.That.IsInstanceOf(typeof(BookingModel)))).MustHaveHappened();
            A.CallTo(() => _calendarBookingService.AddBooking(A<BookingModel>.That.IsInstanceOf(typeof(BookingModel)))).MustHaveHappened();
        }

        [Fact]
        public void Test_AddBookingDoesNotAddsWhenBookingNotAvailable()
        {
            // Arrange
            var model = new BookingModel();

            A.CallTo(() => _calendarBookingService.GetBooking(A<BookingModel>.Ignored))
             .Returns(1);
            A.CallTo(() => _calendarBookingService.AddBooking(A<BookingModel>.Ignored))
             .Returns(model);

            // Act
            string optionalInput = "ADD 09/22 16:22";
            _calendarBookingService.Run(optionalInput);

            // Assert
            A.CallTo(() => _calendarBookingService.GetBooking(A<BookingModel>.That.IsInstanceOf(typeof(BookingModel)))).MustHaveHappened();
            A.CallTo(() => _calendarBookingService.AddBooking(A<BookingModel>.That.IsInstanceOf(typeof(BookingModel)))).MustHaveHappened();
        }
    }
}