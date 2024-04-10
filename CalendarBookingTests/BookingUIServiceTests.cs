namespace CalendarBookingTests
{
    using CalendarBooking.Models;
    using CalendarBooking.Services;
    using CalendarBooking.Services.Utilities;
    using FakeItEasy;
    using Microsoft.Extensions.Logging;
    using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.ObjectModel;
    using Serilog;

    public class BookingUIServiceTests
    {
        private readonly ICalendarBookingService _bookingService;
        private readonly ILogger<BookingUIService> _log;
        private readonly IDateTimeUtilityService _dateTimeUtilityService;

        public BookingUIServiceTests()
        {
            _bookingService = A.Fake<ICalendarBookingService>();
            _log = A.Fake<ILogger<BookingUIService>>();
            _dateTimeUtilityService = new DateTimeUtilityService();
        }

        [Theory]
        [InlineData("ADD 11/04 10:30")]
        [InlineData("ADD 22/09 11:00")]
        public void Run_ADDCommandShouldAddBookingIfNewBooking(string input)
        {
            // Arrange
            var model = new BookingModel();
            var sut = new BookingUIService(_log, _bookingService, _dateTimeUtilityService);

            A.CallTo(() => _bookingService.GetBooking(A<BookingModel>.Ignored))
             .Returns(-1);
            A.CallTo(() => _bookingService.PeformAdd(A<BookingModel>.Ignored))
             .Returns(model);
            // Act
            sut.Run(input);
            // Assert
            A.CallTo(() => _bookingService.GetBooking(A<BookingModel>.That.IsInstanceOf(typeof(BookingModel)))).MustHaveHappened();
            A.CallTo(() => _bookingService.PeformAdd(A<BookingModel>.That.IsInstanceOf(typeof(BookingModel)))).MustHaveHappened();
        }

        [Theory]
        [InlineData("ADD 11/04 10:30")]
        [InlineData("ADD 22/09 11:00")]
        public void Run_ADDCommandShouldNotAddBookingIfExistedBooking(string input)
        {
            // Arrange
            var model = new BookingModel();
            var sut = new BookingUIService(_log, _bookingService, _dateTimeUtilityService);

            A.CallTo(() => _bookingService.GetBooking(A<BookingModel>.Ignored))
             .Returns(1);
            A.CallTo(() => _bookingService.PeformAdd(A<BookingModel>.Ignored))
             .Returns(model);
            // Act
            sut.Run(input);
            // Assert
            A.CallTo(() => _bookingService.GetBooking(A<BookingModel>.That.IsInstanceOf(typeof(BookingModel)))).MustHaveHappened();
            A.CallTo(() => _bookingService.PeformAdd(A<BookingModel>.That.IsInstanceOf(typeof(BookingModel)))).MustNotHaveHappened();
        }

        [Theory]
        [InlineData("ADD 11/04 00:30")]
        [InlineData("ADD 22/09 17:50")]
        public void Run_ADDCommandShouldNotHoursOutsideBookingHours(string input)
        {
            // Arrange
            var model = new BookingModel();
            var sut = new BookingUIService(_log, _bookingService, _dateTimeUtilityService);

            A.CallTo(() => _bookingService.GetBooking(A<BookingModel>.Ignored))
             .Returns(1);
            A.CallTo(() => _bookingService.PeformAdd(A<BookingModel>.Ignored))
             .Returns(model);
            // Act
            sut.Run(input);
            // Assert
            A.CallTo(() => _bookingService.GetBooking(A<BookingModel>.That.IsInstanceOf(typeof(BookingModel)))).MustHaveHappened();
            A.CallTo(() => _bookingService.PeformAdd(A<BookingModel>.That.IsInstanceOf(typeof(BookingModel)))).MustNotHaveHappened();
        }

        [Theory]
        [InlineData("ADD 16/04 16:30")]
        [InlineData("ADD 14/05 17:00")]
        public void Run_ADDCommandShouldNotAddBookingIfSpecialDate(string input)
        {
            // Arrange
            var model = new BookingModel();
            var sut = new BookingUIService(_log, _bookingService, _dateTimeUtilityService);

            A.CallTo(() => _bookingService.GetBooking(A<BookingModel>.Ignored))
             .Returns(1);
            A.CallTo(() => _bookingService.PeformAdd(A<BookingModel>.Ignored))
             .Returns(model);
            // Act
            sut.Run(input);
            // Assert
            A.CallTo(() => _bookingService.GetBooking(A<BookingModel>.That.IsInstanceOf(typeof(BookingModel)))).MustNotHaveHappened();
            A.CallTo(() => _bookingService.PeformAdd(A<BookingModel>.That.IsInstanceOf(typeof(BookingModel)))).MustNotHaveHappened();
        }

        [Theory]
        [InlineData("DELETE 11/04 10:30")]
        [InlineData("DELETE 22/09 11:00")]
        public void Run_DELETECommandShouldNotDeleteBookingIfNewBooking(string input)
        {
            // Arrange
            var model = new BookingModel();
            var sut = new BookingUIService(_log, _bookingService, _dateTimeUtilityService);

            A.CallTo(() => _bookingService.GetBooking(A<BookingModel>.Ignored))
             .Returns(-1);
            A.CallTo(() => _bookingService.PeformDelete(A<BookingModel>.Ignored))
             .Returns(model);
            // Act
            sut.Run(input);
            // Assert
            A.CallTo(() => _bookingService.GetBooking(A<BookingModel>.That.IsInstanceOf(typeof(BookingModel)))).MustHaveHappened();
            A.CallTo(() => _bookingService.PeformDelete(A<BookingModel>.That.IsInstanceOf(typeof(BookingModel)))).MustNotHaveHappened();
        }

        [Theory]
        [InlineData("DELETE 11/04 10:30")]
        [InlineData("DELETE 22/09 11:00")]
        public void Run_DELETECommandShouldDeleteBookingIfExistedBooking(string input)
        {
            // Arrange
            var model = new BookingModel();
            var sut = new BookingUIService(_log, _bookingService, _dateTimeUtilityService);

            A.CallTo(() => _bookingService.GetBooking(A<BookingModel>.Ignored))
             .Returns(1);
            A.CallTo(() => _bookingService.PeformDelete(A<BookingModel>.Ignored))
             .Returns(model);
            // Act
            sut.Run(input);
            // Assert
            A.CallTo(() => _bookingService.GetBooking(A<BookingModel>.That.IsInstanceOf(typeof(BookingModel)))).MustHaveHappened();
            A.CallTo(() => _bookingService.PeformDelete(A<BookingModel>.That.IsInstanceOf(typeof(BookingModel)))).MustHaveHappened();
        }

        [Theory]
        [InlineData("DELETE 16/04 16:30")]
        [InlineData("DELETE 14/05 17:00")]
        public void Run_DELETECommandShouldNotDeleteBookingIfSpecialDate(string input)
        {
            // Arrange
            var model = new BookingModel();
            var sut = new BookingUIService(_log, _bookingService, _dateTimeUtilityService);

            A.CallTo(() => _bookingService.GetBooking(A<BookingModel>.Ignored))
             .Returns(1);
            A.CallTo(() => _bookingService.PeformDelete(A<BookingModel>.Ignored))
             .Returns(model);
            // Act
            sut.Run(input);
            // Assert
            A.CallTo(() => _bookingService.GetBooking(A<BookingModel>.That.IsInstanceOf(typeof(BookingModel)))).MustNotHaveHappened();
            A.CallTo(() => _bookingService.PeformDelete(A<BookingModel>.That.IsInstanceOf(typeof(BookingModel)))).MustNotHaveHappened();
        }
    }
}