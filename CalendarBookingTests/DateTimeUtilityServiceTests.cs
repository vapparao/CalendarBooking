using CalendarBooking.Services.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalendarBookingTests
{
    public class DateTimeUtilityServiceTests
    {
        private readonly IDateTimeUtilityService _dateTimeUtilityService;

        public DateTimeUtilityServiceTests()
        {
            _dateTimeUtilityService = new DateTimeUtilityService();
        }

        [Theory]
        [InlineData("16", "04", "2024", "16", "00")]
        [InlineData("16", "04", "2024", "16", "14")]
        [InlineData("16", "04", "2024", "16", "30")]
        [InlineData("16", "04", "2024", "16", "46")]
        [InlineData("16", "04", "2024", "17", "00")]
        [InlineData("14", "05", "2024", "17", "00")]
        [InlineData("12", "03", "2024", "16", "45")]
        [InlineData("13", "02", "2024", "16", "30")]
        [InlineData("11", "02", "2025", "16", "30")]
        [InlineData("12", "12", "2023", "16", "30")]
        public void IsSecondDayOfThirdWeek_ReturnsTrueForMatchingData(string date, string month, string year, string hours, string minutes)
        {
            // Arrange

            // Act
            var result = _dateTimeUtilityService.IsSecondDayOfThirdWeek(date, month, year, hours, minutes);

            // Assert
            Assert.True(result);
        }

        [Theory]
        [InlineData("16", "04", "2024", "15", "59")]
        [InlineData("16", "04", "2024", "17", "14")]
        [InlineData("16", "04", "2024", "15", "30")]
        [InlineData("16", "04", "2024", "15", "46")]
        [InlineData("16", "04", "2024", "17", "01")]
        [InlineData("13", "02", "2024", "14", "30")]
        [InlineData("11", "02", "2025", "12", "30")]
        [InlineData("12", "12", "2023", "17", "30")]
        public void IsSecondDayOfThirdWeek_ReturnsFalseForUnMatchedData(string date, string month, string year, string hours, string minutes)
        {
            // Arrange

            // Act
            var result = _dateTimeUtilityService.IsSecondDayOfThirdWeek(date, month, year, hours, minutes);

            // Assert
            Assert.False(result);
        }
    }
}