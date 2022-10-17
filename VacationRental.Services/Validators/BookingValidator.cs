using VacationRental.Data.Entities;

namespace VacationRental.Services.Validators;

public class BookingValidator
{
    public Task<bool> IsRentalAvailable(Rental rental, int usedUnits) 
        => Task.FromResult(usedUnits < rental.Units);
}