using System;
using System.Collections.Generic;
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
    public class RentalServiceTests
    {
        private readonly RentalService _sut;
        private readonly IRentalRepository _rentalRepository = new RentalRepository(new Dictionary<int, Rental>());

        private readonly IBookingRepository _bookingRepository = Substitute.For<IBookingRepository>();
        private readonly IPreparationDaysRepository _preparationDaysRepository = Substitute.For<IPreparationDaysRepository>();
        private readonly IMapper _mapper;
        private readonly RentalValidator _rentalValidator = new();

        public RentalServiceTests()
        {
            _mapper = AutomapperConfiguration.CreateAutomapper();
            _sut = new RentalService(_rentalRepository, _mapper, _rentalValidator, _preparationDaysRepository,
                _bookingRepository);
        }

        [Fact]
        public async Task GetRentalByIdAsync_ShouldFail_WhenRentalDoesNotExist()
        {
            //arrange
            const int rentalId = 1;

            //act
            var booking = _sut.GetRentalByIdAsync(rentalId);

            //assert
            var ex = await Assert.ThrowsAsync<ApplicationException>(() => booking);
            Assert.Equal("Rental not found", ex.Message);
        }

        [Fact]
        public async Task GetRentalByIdAsync_ShouldRetrieveARental_WhenRentalExists()
        {
            //arrange
            var rentalViewModel = new RentalViewModel() { PreparationTimeInDays = 1, Units = 1 };
            var createdRental = await _sut.CreateRentalAsync(rentalViewModel);

            //act
            var getRental = await _sut.GetRentalByIdAsync(createdRental.Id);

            //assert
            Assert.IsType<RentalViewModel>(getRental);
            Assert.Equal(createdRental.Id, getRental.Id);
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(0, 0)]
        [InlineData(0, 1)]
        public async Task CreateRentalAsync_ShouldCreateARental_WhenAllParametersAreCorrect(int units, int preparationDays)
        {
            //arrange
            var rentalViewModel = new RentalViewModel() { PreparationTimeInDays = preparationDays, Units = units };

            //act
            var createdRental = await _sut.CreateRentalAsync(rentalViewModel);

            //assert
            Assert.IsType<ResourceIdViewModel>(createdRental);
        }

        [Theory]
        [InlineData(-1, 1)]
        [InlineData(0, -1)]
        [InlineData(-1, -1)]
        public async Task CreateRentalAsync_ShouldThrowAnException_WhenAnyParameterIsInvalid(int units, int preparationDays)
        {
            //arrange
            var rentalViewModel = new RentalViewModel() { PreparationTimeInDays = preparationDays, Units = units };

            //act
            var createdRental = _sut.CreateRentalAsync(rentalViewModel);

            //assert
            var ex = await Assert.ThrowsAsync<ApplicationException>(() => createdRental);
        }

        [Theory]
        [InlineData(-1, 1)]
        [InlineData(0, -1)]
        [InlineData(-1, -1)]
        public async Task UpdateRentalAsync_ShouldThrowAnException_WhenAnyParameterIsInvalid(int units, int preparationDays)
        {
            //arrange
            var goodRentalViewModel = new RentalViewModel() { PreparationTimeInDays = 1, Units = 1 };
            var rentalViewModel = new RentalViewModel() { PreparationTimeInDays = preparationDays, Units = units };
            
            var createdRental = _sut.CreateRentalAsync(goodRentalViewModel);

            //act
            var updatedRental = _sut.UpdateRentalAsync(createdRental.Id, rentalViewModel);

            //assert
            var ex = await Assert.ThrowsAsync<ApplicationException>(() => updatedRental);
        }

        [Theory]
        [InlineData(0, 1)]
        [InlineData(1, 5)]
        public async Task UpdateRentalAsync_ShouldThrowAnException_WhenOverbooking(int units, int preparationDays)
        {
            //arrange
            var goodRentalViewModel = new RentalViewModel() { PreparationTimeInDays = 1, Units = 1 };
            var rentalViewModel = new RentalViewModel() { PreparationTimeInDays = preparationDays, Units = units };

            var createdRental = await _sut.CreateRentalAsync(goodRentalViewModel);
            var firstBooking = new Booking()
            {
                Id = 1,
                Nights = 1,
                RentalId = createdRental.Id,
                Start = new DateTime(2000, 01, 01),
                Unit = 1
            };
            var secondBooking = new Booking()
            {
                Id = 2,
                Nights = 1,
                RentalId = createdRental.Id,
                Start = new DateTime(2000, 01, 03),
                Unit = 1
            };
            _bookingRepository.GetAllAsync().Returns(new List<Booking>()
            {
                firstBooking, secondBooking
            });

            _preparationDaysRepository.GetAllAsync().Returns(new List<PreparationDays>()
            {
                new PreparationDays()
                {
                    Id = firstBooking.Id,
                    Days = goodRentalViewModel.PreparationTimeInDays,
                    RentalId = createdRental.Id,
                    Start = firstBooking.Start.AddDays(firstBooking.Nights),
                    Unit = 1
                },
                new PreparationDays()
                {
                    Id = secondBooking.Id,
                    Days = goodRentalViewModel.PreparationTimeInDays,
                    RentalId = createdRental.Id,
                    Start = secondBooking.Start.AddDays(secondBooking.Nights),
                    Unit = 1
                }
            });
            //act
            var updatedRental = _sut.UpdateRentalAsync(createdRental.Id, rentalViewModel);

            //assert
            var ex = await Assert.ThrowsAsync<ApplicationException>(() => updatedRental);
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(1, 2)]
        [InlineData(2, 3)]
        public async Task UpdateRentalAsync_ShouldUpdateARental_WhenParametersAreValidAndNoOverbooking(int units, int preparationDays)
        {
            //arrange
            var goodRentalViewModel = new RentalViewModel() { PreparationTimeInDays = 1, Units = 1 };
            var rentalViewModel = new RentalViewModel() { PreparationTimeInDays = preparationDays, Units = units };

            var createdRental = await _sut.CreateRentalAsync(goodRentalViewModel);
            var firstBooking = new Booking()
            {
                Id = 1,
                Nights = 1,
                RentalId = createdRental.Id,
                Start = new DateTime(2000, 01, 01),
                Unit = 1
            };
            var secondBooking = new Booking()
            {
                Id = 2,
                Nights = 1,
                RentalId = createdRental.Id,
                Start = new DateTime(2000, 01, 10),
                Unit = 1
            };
            _bookingRepository.GetAllAsync().Returns(new List<Booking>()
            {
                firstBooking, secondBooking
            });

            _preparationDaysRepository.GetAllAsync().Returns(new List<PreparationDays>()
            {
                new PreparationDays()
                {
                    Id = firstBooking.Id,
                    Days = goodRentalViewModel.PreparationTimeInDays,
                    RentalId = createdRental.Id,
                    Start = firstBooking.Start.AddDays(firstBooking.Nights),
                    Unit = 1
                },
                new PreparationDays()
                {
                    Id = secondBooking.Id,
                    Days = goodRentalViewModel.PreparationTimeInDays,
                    RentalId = createdRental.Id,
                    Start = secondBooking.Start.AddDays(secondBooking.Nights),
                    Unit = 1
                }
            });
            //act
            var updatedRental = await _sut.UpdateRentalAsync(createdRental.Id, rentalViewModel);

            //assert
            Assert.IsType<RentalViewModel>(updatedRental);
            Assert.Equal(rentalViewModel.PreparationTimeInDays, updatedRental.PreparationTimeInDays);
            Assert.Equal(rentalViewModel.Units, updatedRental.Units);
        }
    }
}
