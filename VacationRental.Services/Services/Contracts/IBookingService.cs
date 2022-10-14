using VacationRental.Model.ViewModels;

namespace VacationRental.Services.Services.Contracts;

public interface IBookingService
{
    Task<BookingViewModel> GetBookingByIdAsync(int id);
    Task<ResourceIdViewModel> CreateBookingAsync(BookingViewModel model);
}