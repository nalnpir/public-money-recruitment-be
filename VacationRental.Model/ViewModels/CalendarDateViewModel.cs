namespace VacationRental.Model.ViewModels;

public class CalendarDateViewModel
{
    public DateTime Date { get; set; }
    public List<CalendarBookingViewModel> Bookings { get; set; } = new();
    public List<CalendarPreparationTimeViewModel> PreparationTimes { get; set; } = new();
}