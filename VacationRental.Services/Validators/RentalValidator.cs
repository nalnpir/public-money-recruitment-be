using VacationRental.Data.Entities;
using VacationRental.Model.ViewModels;
using VacationRental.Services.Utilities;

namespace VacationRental.Services.Validators;

public class RentalValidator
{
    public Task<bool> ArePreparationDaysAPositiveNumberOrZero(int days)
    {
        return Task.FromResult(days >= 0);
    }
    public Task<bool> AreUnitsAPositiveNumberOrZero(int units)
    {
        return Task.FromResult(units >= 0);
    }
    public Task<bool> IsUnitOccupiedDuringPreparationDays(Booking booking, PreparationDays preparationDays)
    {
        var finalDay = preparationDays.Start.AddDays(preparationDays.Days);
        return Task.FromResult(booking.Unit == preparationDays.Unit && DateTimeUtility.IsBetweenTwoDates(booking.Start, preparationDays.Start, finalDay));
    }

    public Task<bool> IsRentalUnitInPreparation(BookingViewModel model, PreparationDays preparationDays)
    {
        if (preparationDays.Days == 0) return Task.FromResult(false);
        return Task.FromResult(DateTimeUtility.AreDatesOverlapping(model.Start, model.Nights, preparationDays.Start, preparationDays.Days));
    }

    public Task<bool> IsRentalUnitOccupied(BookingViewModel model, Booking booking)
    {
        return Task.FromResult(DateTimeUtility.AreDatesOverlapping(model.Start, model.Nights, booking.Start, booking.Nights));
    }
}