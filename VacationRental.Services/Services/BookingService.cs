using AutoMapper;
using VacationRental.Data.Entities;
using VacationRental.Data.Repositories.Contracts;
using VacationRental.Model.ViewModels;
using VacationRental.Services.Services.Contracts;
using VacationRental.Services.Validators;

namespace VacationRental.Services.Services;

public class BookingService : IBookingService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IMapper _mapper;
    private readonly CommonValidator _commonValidator;
    private readonly BookingValidator _bookingValidator;

    public BookingService(IBookingRepository bookingRepository,
        IMapper mapper,
        CommonValidator commonValidator,
        BookingValidator bookingValidator)
    {
        _bookingRepository = bookingRepository;
        _mapper = mapper;
        _commonValidator = commonValidator;
        _bookingValidator = bookingValidator;
    }

    public async Task<BookingViewModel> GetBookingByIdAsync(int id)
    {
        try
        {
            return _mapper.Map<BookingViewModel>(await _bookingRepository.GetByIdAsync(id));
        }
        catch (KeyNotFoundException)
        {
            throw new ApplicationException("Rental not found");
        }
    }

    public async Task<ResourceIdViewModel> CreateBookingAsync(BookingViewModel model)
    {
        if (!await _commonValidator.AreNightsAPositiveNumber(model.Nights))
            throw new ApplicationException("Nights must be positive");
        if (!await _commonValidator.DoesRentalExists(model.RentalId))
            throw new ApplicationException("Rental not found");

        for (var i = 0; i < model.Nights; i++)
        {
            var count = 0;
            foreach (var booking in await _bookingRepository.GetAllAsync())
            {
                if (booking.RentalId == model.RentalId
                    && (booking.Start <= model.Start.Date && booking.Start.AddDays(booking.Nights) > model.Start.Date)
                    || (booking.Start < model.Start.AddDays(model.Nights) && booking.Start.AddDays(booking.Nights) >= model.Start.AddDays(model.Nights))
                    || (booking.Start > model.Start && booking.Start.AddDays(booking.Nights) < model.Start.AddDays(model.Nights)))
                {
                    count++;
                }
            }
            if (!await _bookingValidator.IsRentalAvailable(model.RentalId, count))
                throw new ApplicationException("Not available");
        }

        var newBooking = await _bookingRepository.InsertAsync(_mapper.Map<Booking>(model));

        return new ResourceIdViewModel { Id = newBooking.Id };
    }
}