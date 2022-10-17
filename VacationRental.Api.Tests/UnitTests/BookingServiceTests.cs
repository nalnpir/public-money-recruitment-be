using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using NSubstitute;
using VacationRental.Data.Entities;
using VacationRental.Data.Repositories;
using VacationRental.Data.Repositories.Contracts;
using VacationRental.Model.ViewModels;
using VacationRental.Services.Services;
using VacationRental.Services.Validators;
using Xunit;

namespace VacationRental.Api.Tests.UnitTests
{
    public class BookingServiceTests
    {
        private readonly BookingService _sut;
        private readonly IBookingRepository _bookingRepository = new BookingRepository(new Dictionary<int, Booking>());

        private readonly IPreparationDaysRepository _preparationDaysRepository = Substitute.For<IPreparationDaysRepository>();
        private readonly IRentalRepository _rentalRepository = Substitute.For<IRentalRepository>();
        private readonly IMapper _mapper;
        private readonly CommonValidator _commonValidator;
        private readonly BookingValidator _bookingValidator = new();
        private readonly RentalValidator _rentalValidator = new();
        public BookingServiceTests()
        {
            _mapper = AutomapperConfiguration.CreateAutomapper();
            _commonValidator = new CommonValidator(_rentalRepository);
            _sut = new BookingService(_bookingRepository, _mapper, _commonValidator, _bookingValidator, _rentalValidator, _preparationDaysRepository, _rentalRepository);
        }

        [Fact]
        public async Task CreateBookingAsync_ShouldCreateABooking_WhenParametersAreValid()
        {
            //arrange
            const int bookingId = 1;
            const int rentalId = 1;
            const int nights = 1;
            const int days = 0;
            DateTime start = new DateTime(2000, 01, 01);
            var bookingViewModel = new BookingViewModel()
            {
                Nights = nights,
                RentalId = rentalId,
                Start = start
            };

            var rental = new Rental() { Id = 1, PreparationTimeInDays = days, Units = 1 };

            var booking = _mapper.Map<Booking>(bookingViewModel);
            booking.Unit = 1;
            booking.Id = bookingId;

            _rentalRepository.GetByIdAsync(rentalId).Returns(rental);
            _rentalRepository.ExistsAsync(1).Returns(true);
            _preparationDaysRepository.GetAllAsync().Returns(new List<PreparationDays>());

            //act
            var createdBooking = await _sut.CreateBookingAsync(bookingViewModel);
            
            //assert
            Assert.IsType<ResourceIdViewModel>(createdBooking);
            Assert.Equal(bookingId, createdBooking.Id);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public async Task CreateBookingAsync_ShouldFail_WhenNightsAreNotValid(int nights)
        {
            //arrange

            const int rentalId = 1;
            DateTime start = new DateTime(2000, 01, 01);
            var bookingViewModel = new BookingViewModel()
            {
                Nights = nights,
                RentalId = rentalId,
                Start = start
            };

            //act
            var createdBooking = _sut.CreateBookingAsync(bookingViewModel);

            //assert
            var ex = await Assert.ThrowsAsync<ApplicationException>(() => createdBooking);
            Assert.Equal("Nights must be positive", ex.Message);
        }

        [Fact]
        public async Task CreateBookingAsync_ShouldFail_WhenRentalDoesNotExists()
        {
            //arrange
            const int nights = 1;
            const int rentalId = 1;
            DateTime start = new DateTime(2000, 01, 01);
            var bookingViewModel = new BookingViewModel()
            {
                Nights = nights,
                RentalId = rentalId,
                Start = start
            };

            //act
            var createdBookingFailure = _sut.CreateBookingAsync(bookingViewModel);

            //assert
            var ex = await Assert.ThrowsAsync<ApplicationException>(() => createdBookingFailure);
            Assert.Equal("Rental not found", ex.Message);
        }

        [Fact]
        public async Task CreateBookingAsync_ShouldFail_WhenOverlappingDueToOverbooking()
        {
            //arrange
            const int bookingId = 0;
            const int rentalId = 1;
            const int nights = 1;
            const int days = 0;
            DateTime start = new DateTime(2000, 01, 01);
            var bookingViewModel = new BookingViewModel()
            {
                Nights = nights,
                RentalId = rentalId,
                Start = start
            };

            var rental = new Rental() { Id = 1, PreparationTimeInDays = days, Units = 1 };

            var booking = _mapper.Map<Booking>(bookingViewModel);
            booking.Unit = 1;
            booking.Id = bookingId;

            _rentalRepository.GetByIdAsync(rentalId).Returns(rental);
            _rentalRepository.ExistsAsync(1).Returns(true);
            _preparationDaysRepository.GetAllAsync().Returns(new List<PreparationDays>());

            //act
            await _sut.CreateBookingAsync(bookingViewModel);
            var createdBookingFailure = _sut.CreateBookingAsync(bookingViewModel);
            //assert
            var ex = await Assert.ThrowsAsync<ApplicationException>(() => createdBookingFailure);
            Assert.Equal("Not available", ex.Message);
        }

        [Fact]
        public async Task CreateBookingAsync_ShouldFail_WhenOverlappingDueToPreparationDays()
        {
            //arrange
            const int bookingId = 0;
            const int rentalId = 1;
            const int nights = 1;
            const int days = 0;
            DateTime start = new DateTime(2000, 01, 01);
            var bookingViewModel = new BookingViewModel()
            {
                Nights = nights,
                RentalId = rentalId,
                Start = start
            };

            var rental = new Rental() { Id = 1, PreparationTimeInDays = days, Units = 1 };

            var booking = _mapper.Map<Booking>(bookingViewModel);
            booking.Unit = 1;
            booking.Id = bookingId;

            _rentalRepository.GetByIdAsync(rentalId).Returns(rental);
            _rentalRepository.ExistsAsync(1).Returns(true);
            _preparationDaysRepository.GetAllAsync().Returns(new List<PreparationDays>()
            {
                new PreparationDays() { Id = 1, Days = 10, RentalId = rentalId, Start = start, Unit = 1 },
            });

            //act
            var createdBookingFailure = _sut.CreateBookingAsync(bookingViewModel);

            //assert
            var ex = await Assert.ThrowsAsync<ApplicationException>(() => createdBookingFailure);
            Assert.Equal("Not available", ex.Message);
        }

        [Fact]
        public async Task GetBookingByIdAsync_ShouldFail_WhenBookingDoesNotExist()
        {
            //arrange

            //act
            var booking = _sut.GetBookingByIdAsync(1);

            //assert
            var ex = await Assert.ThrowsAsync<ApplicationException>(() => booking);
            Assert.Equal("Rental not found", ex.Message);
        }

        [Fact]
        public async Task GetBookingByIdAsync_ShouldRetrieveABooking_WhenBookingExists()
        {
            //arrange
            const int bookingId = 1;
            const int rentalId = 1;
            const int nights = 1;
            const int days = 0;
            DateTime start = new DateTime(2000, 01, 01);
            var bookingViewModel = new BookingViewModel()
            {
                Nights = nights,
                RentalId = rentalId,
                Start = start
            };

            var rental = new Rental() { Id = 1, PreparationTimeInDays = days, Units = 1 };

            var booking = _mapper.Map<Booking>(bookingViewModel);
            booking.Unit = 1;

            _rentalRepository.GetByIdAsync(rentalId).Returns(rental);
            _rentalRepository.ExistsAsync(1).Returns(true);
            _preparationDaysRepository.GetAllAsync().Returns(new List<PreparationDays>());

            //act
            await _sut.CreateBookingAsync(bookingViewModel);
            var createdBooking = await _sut.GetBookingByIdAsync(1);

            //assert
            Assert.IsType<BookingViewModel>(createdBooking);
            Assert.Equal(bookingId, createdBooking.Id);
            Assert.Equal(booking.Nights, createdBooking.Nights);
            Assert.Equal(booking.RentalId, createdBooking.RentalId);
            Assert.Equal(booking.Start, createdBooking.Start);
        }
    }
}
