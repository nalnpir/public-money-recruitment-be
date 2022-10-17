using AutoMapper;
using VacationRental.Data.Entities;
using VacationRental.Data.Repositories.Contracts;
using VacationRental.Model.ViewModels;
using VacationRental.Services.Services.Contracts;
using VacationRental.Services.Utilities;
using VacationRental.Services.Validators;

namespace VacationRental.Services.Services;

public class CalendarService : ICalendarService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly CommonValidator _commonValidator;
    private readonly IPreparationDaysRepository _preparationDaysRepository;

    public CalendarService(
        IBookingRepository bookingRepository,
        CommonValidator commonValidator, 
        IPreparationDaysRepository preparationDaysRepository)
    {
        _bookingRepository = bookingRepository;
        _commonValidator = commonValidator;
        _preparationDaysRepository = preparationDaysRepository;
    }
    public async Task<CalendarViewModel> GetCalendarAvailabilityAsync(int rentalId, DateTime start, int nights)
    {
        if (!await _commonValidator.AreNightsAPositiveNumber(nights))
            throw new ApplicationException("Nights must be positive");
        if (!await _commonValidator.DoesRentalExists(rentalId))
            throw new ApplicationException("Rental not found");

        var result = new CalendarViewModel
        {
            RentalId = rentalId,
            Dates = new List<CalendarDateViewModel>()
        };

        var filteredBookings = (await _bookingRepository.GetAllAsync()).Where(x => x.RentalId == rentalId).ToList();
        var filteredPreparationDays = (await _preparationDaysRepository.GetAllAsync()).Where(x => x.RentalId == rentalId).ToList();

        for (var i = 0; i < nights; i++)
        {
            var date = new CalendarDateViewModel
            {
                Date = start.Date.AddDays(i),
                Bookings = new List<CalendarBookingViewModel>(),
                PreparationTimes = new List<CalendarPreparationTimeViewModel>()
            };

            foreach (var booking in filteredBookings)
            {
                if (await IsRentalUnitAlreadyBooked(booking, date))
                {
                    date.Bookings.Add(new CalendarBookingViewModel { Id = booking.Id, Unit = booking.Unit});
                }
            }

            foreach (var preparationDays in filteredPreparationDays)
            {
                if (await IsRentalUnitInPreparation(preparationDays, date))
                {
                    date.PreparationTimes.Add(new CalendarPreparationTimeViewModel()
                    {
                        Unit = preparationDays.Unit
                    });
                }
            }

            result.Dates.Add(date);
        }

        return result;
    }

    private static Task<bool> IsRentalUnitAlreadyBooked(Booking booking, CalendarDateViewModel date)
    {
        return Task.FromResult(DateTimeUtility.IsBetweenTwoDates(date.Date, booking.Start, booking.Start.AddDays(booking.Nights)));
    }
    private static Task<bool> IsRentalUnitInPreparation(PreparationDays preparationDays, CalendarDateViewModel date)
    {
        return Task.FromResult(DateTimeUtility.IsBetweenTwoDates(date.Date, preparationDays.Start, preparationDays.Start.AddDays(preparationDays.Days)));
    }
}