using AutoMapper;
using VacationRental.Data.Repositories.Contracts;
using VacationRental.Model.ViewModels;
using VacationRental.Services.Services.Contracts;
using VacationRental.Services.Validators;

namespace VacationRental.Services.Services;

public class CalendarService : ICalendarService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IMapper _mapper;
    private readonly CommonValidator _commonValidator;

    public CalendarService(IBookingRepository bookingRepository, IMapper mapper, CommonValidator commonValidator)
    {
        _bookingRepository = bookingRepository;
        _mapper = mapper;
        _commonValidator = commonValidator;
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

        for (var i = 0; i < nights; i++)
        {
            var date = new CalendarDateViewModel
            {
                Date = start.Date.AddDays(i),
                Bookings = new List<CalendarBookingViewModel>()
            };

            foreach (var booking in await _bookingRepository.GetAllAsync())
            {
                if (booking.RentalId == rentalId
                    && booking.Start <= date.Date && booking.Start.AddDays(booking.Nights) > date.Date)
                {
                    date.Bookings.Add(new CalendarBookingViewModel { Id = booking.Id });
                }
            }

            result.Dates.Add(date);
        }

        return result;
    }
}