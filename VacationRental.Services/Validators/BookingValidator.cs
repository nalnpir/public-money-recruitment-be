using VacationRental.Data.Repositories.Contracts;

namespace VacationRental.Services.Validators;

public class BookingValidator
{
    private readonly IRentalRepository _rentalRepository;

    public BookingValidator(IRentalRepository rentalRepository)
    {
        _rentalRepository = rentalRepository;
    }

    public async Task<bool> IsRentalAvailable(int rentalId, int availability)
    {
        return availability < (await _rentalRepository.GetByIdAsync(rentalId)).Units;
    }
}