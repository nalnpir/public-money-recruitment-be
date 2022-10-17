using System.Diagnostics;
using AutoMapper;
using VacationRental.Data.Entities;
using VacationRental.Data.Repositories.Contracts;
using VacationRental.Model.ViewModels;
using VacationRental.Services.Services.Contracts;
using VacationRental.Services.Validators;

namespace VacationRental.Services.Services;

public class RentalService : IRentalService
{
    private readonly IRentalRepository _rentalRepository;
    private readonly IBookingRepository _bookingRepository;
    private readonly IPreparationDaysRepository _preparationDaysRepository;
    private readonly IMapper _mapper;
    private readonly RentalValidator _rentalValidator;

    public RentalService(
        IRentalRepository rentalRepository, 
        IMapper mapper, 
        RentalValidator rentalValidator, 
        IPreparationDaysRepository preparationDaysRepository, 
        IBookingRepository bookingRepository)
    {
        _rentalRepository = rentalRepository;
        _mapper = mapper;
        _rentalValidator = rentalValidator;
        _preparationDaysRepository = preparationDaysRepository;
        _bookingRepository = bookingRepository;
    }
    public async Task<RentalViewModel> GetRentalByIdAsync(int id)
    {
        try
        {
            return _mapper.Map<RentalViewModel>(await _rentalRepository.GetByIdAsync(id));
        }
        catch (KeyNotFoundException)
        {
            throw new ApplicationException("Rental not found");
        }
    }

    public async Task<ResourceIdViewModel> CreateRentalAsync(RentalViewModel model)
    {
        if (!await _rentalValidator.ArePreparationDaysAPositiveNumberOrZero(model.PreparationTimeInDays))
            throw new ApplicationException("Preparations should be equal or greater than 0");

        if (!await _rentalValidator.AreUnitsAPositiveNumberOrZero(model.Units))
            throw new ApplicationException("Units should be equal or greater than 0");

        var rental = await _rentalRepository.InsertAsync(_mapper.Map<Rental>(model));

        var key = new ResourceIdViewModel { Id = rental.Id };

        return key;
    }

    public async Task<RentalViewModel> UpdateRentalAsync(int rentalId, RentalViewModel model)
    {
        if (!await _rentalValidator.ArePreparationDaysAPositiveNumberOrZero(model.PreparationTimeInDays))
            throw new ApplicationException("Preparations should be equal or greater than 0");

        if (!await _rentalValidator.AreUnitsAPositiveNumberOrZero(model.Units))
            throw new ApplicationException("Units should be equal or greater than 0");

        var originalRental = await _rentalRepository.GetByIdAsync(rentalId);
        var newRental = _mapper.Map<Rental>(model);
        newRental.Id = rentalId;

        if (originalRental.Equals(newRental))
            return model;

        var bookings = (await _bookingRepository.GetAllAsync())
            .Where(x => x.RentalId == originalRental.Id)
            .OrderBy(x => x.Start)
            .ToList();

        var preparationsDays = (await _preparationDaysRepository.GetAllAsync())
            .Where(x => x.RentalId == originalRental.Id)
            .OrderBy(x => x.Start)
            .ToList();

        var targetPreparationDays =
            preparationsDays.Select(x =>
                {
                    x.Days = newRental.PreparationTimeInDays;
                    return x;
                })
                .ToList();

        if (await ArePreparationDaysAndBookingsOverlapping(originalRental, newRental, preparationsDays, bookings))
            throw new ApplicationException("Not possible");

        var updatedRental = await _rentalRepository.UpdateAsync(newRental);
        await _preparationDaysRepository.UpdateBulkAsync(targetPreparationDays);

        return _mapper.Map<RentalViewModel>(updatedRental);
    }

    private Task<bool> ArePreparationDaysAndBookingsOverlapping(
        Rental originalRental,
        Rental targetRental,
        List<PreparationDays> originalPreparationDays,
        List<Booking> originalBookings)
    {

        if (originalRental.PreparationTimeInDays >= targetRental.PreparationTimeInDays && originalRental.Units <= targetRental.Units)
            return Task.FromResult(false);

        if (originalRental.PreparationTimeInDays < targetRental.PreparationTimeInDays)
        {
            var targetPreparationDays =
                originalPreparationDays.Select(x =>
                {
                    x.Days = targetRental.PreparationTimeInDays;
                    return x;
                })
                    .ToList();

            foreach (var preparationDays in targetPreparationDays)
            {
                if (originalBookings.Any(booking => _rentalValidator.IsUnitOccupiedDuringPreparationDays(booking, preparationDays).Result))
                    return Task.FromResult(true);
            }
        }

        return Task.FromResult(originalRental.Units > targetRental.Units && originalBookings.Any(x => x.Unit > targetRental.Units));
    }
}