using VacationRental.Model.ViewModels;

namespace VacationRental.Services.Services.Contracts;

public interface IRentalService
{
    Task<RentalViewModel> GetRentalByIdAsync(int id);
    Task<ResourceIdViewModel> CreateRentalAsync(RentalViewModel model);
}