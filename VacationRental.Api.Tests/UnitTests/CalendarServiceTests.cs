using System;
using System.Threading.Tasks;
using NSubstitute;
using VacationRental.Data.Repositories.Contracts;
using VacationRental.Services.Services;
using VacationRental.Services.Validators;
using Xunit;

namespace VacationRental.Api.Tests.UnitTests
{
    public class CalendarServiceTests
    {
        public readonly CalendarService _sut;

        private readonly IBookingRepository _bookingRepository = Substitute.For<IBookingRepository>();
        private readonly CommonValidator _commonValidator;
        private readonly IRentalRepository _rentalRepository = Substitute.For<IRentalRepository>();
        private readonly IPreparationDaysRepository _preparationDaysRepository = Substitute.For<IPreparationDaysRepository>();

        public CalendarServiceTests()
        {
            _commonValidator = new CommonValidator(_rentalRepository);
            _sut = new CalendarService(_bookingRepository, _commonValidator, _preparationDaysRepository);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public async Task GetCalendarAvailabilityAsync_ShouldFail_WhenNightsAreNotValid(int nights)
        {
            //arrange
            const int rentalId = 1;
            DateTime start = new DateTime(2000, 01, 01);
            
            //act
            var calendar = _sut.GetCalendarAvailabilityAsync(rentalId, start, nights);

            //assert
            var ex = await Assert.ThrowsAsync<ApplicationException>(() => calendar);
            Assert.Equal("Nights must be positive", ex.Message);
        }

        [Fact]
        public async Task GetCalendarAvailabilityAsync_ShouldFail_WhenRentalDoesNotExist()
        {
            //arrange
            const int rentalId = 1;
            DateTime start = new DateTime(2000, 01, 01);
            const int nights = 1;
            //act
            var calendar = _sut.GetCalendarAvailabilityAsync(rentalId, start, nights);

            //assert
            var ex = await Assert.ThrowsAsync<ApplicationException>(() => calendar);
            Assert.Equal("Rental not found", ex.Message);
        }
    }
}
