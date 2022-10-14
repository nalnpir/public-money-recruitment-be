using VacationRental.Data.Repositories.Contracts;

namespace VacationRental.Services.Validators;

public class CommonValidator
{
    private readonly IRentalRepository _rentalRepository;

    public CommonValidator(IRentalRepository rentalRepository)
    {
        _rentalRepository = rentalRepository;
    }
    public Task<bool> AreNightsAPositiveNumber(int nights) => Task.FromResult(nights > 0);
    public async Task<bool> DoesRentalExists(int rentalId) => await _rentalRepository.ExistsAsync(rentalId);
}