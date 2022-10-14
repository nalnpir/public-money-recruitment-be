using VacationRental.Model.ViewModels;

namespace VacationRental.Services.Services.Contracts;

public interface ICalendarService
{
    public Task<CalendarViewModel> GetCalendarAvailabilityAsync(int rentalId, DateTime start, int nights);
}