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
    private readonly IPreparationDaysRepository _preparationDaysRepository;
    private readonly IRentalRepository _rentalRepository;
    private readonly IMapper _mapper;
    private readonly CommonValidator _commonValidator;
    private readonly BookingValidator _bookingValidator;
    private readonly RentalValidator _rentalValidator;

    public BookingService(IBookingRepository bookingRepository,
        IMapper mapper,
        CommonValidator commonValidator,
        BookingValidator bookingValidator,
        RentalValidator rentalValidator,
        IPreparationDaysRepository preparationDaysRepository, 
        IRentalRepository rentalRepository)
    {
        _bookingRepository = bookingRepository;
        _mapper = mapper;
        _commonValidator = commonValidator;
        _bookingValidator = bookingValidator;
        _rentalValidator = rentalValidator;
        _preparationDaysRepository = preparationDaysRepository;
        _rentalRepository = rentalRepository;
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

        var filteredBookings = (await _bookingRepository.GetAllAsync()).Where(x => x.RentalId == model.RentalId).ToList();
        var filteredPreparationDays = (await _preparationDaysRepository.GetAllAsync())
            .Where(x => x.RentalId == model.RentalId).ToList();
        var rental = await _rentalRepository.GetByIdAsync(model.RentalId);

        var busyUnits = await RentalUnitsOccupied(model, filteredBookings, filteredPreparationDays, rental);

        if (!await _bookingValidator.IsRentalAvailable(rental, busyUnits.Count(x => x.Value)))
            throw new ApplicationException("Not available");

        var mappedBooking = _mapper.Map<Booking>(model);
        var occupiedUnit = busyUnits.FirstOrDefault(x => !x.Value).Key;
        mappedBooking.Unit = occupiedUnit;

        var newBooking = await _bookingRepository.InsertAsync(mappedBooking);

        var mappedPreparationDays = _mapper.Map<PreparationDays>(model);
        _mapper.Map(rental, mappedPreparationDays);
        mappedPreparationDays.Unit = occupiedUnit;

        await _preparationDaysRepository.InsertAsync(mappedPreparationDays);

        return new ResourceIdViewModel { Id = newBooking.Id };
    }

    private async Task<Dictionary<int, bool>> RentalUnitsOccupied(BookingViewModel model, List<Booking> bookings, List<PreparationDays> preparationDays, Rental rental)
    {
        var busyUnits = new Dictionary<int, bool>();

        foreach (var unit in Enumerable.Range(1, rental.Units))
        {
            busyUnits[unit] = false;
        }

        foreach (var booking in bookings)
        {
            if (await _rentalValidator.IsRentalUnitOccupied(model, booking)) busyUnits[booking.Unit] = true;
        }

        foreach (var prepDays in preparationDays)
        {
            if (await _rentalValidator.IsRentalUnitInPreparation(model, prepDays)) busyUnits[prepDays.Unit] = true;
        }

        return busyUnits;
    }
}