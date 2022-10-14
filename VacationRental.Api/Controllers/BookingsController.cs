using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VacationRental.Model.BiindingModels;
using VacationRental.Model.ViewModels;
using VacationRental.Services.Services.Contracts;

namespace VacationRental.Api.Controllers;

[Route("api/v1/bookings")]
[ApiController]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public BookingsController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    [HttpGet]
    [Route("{bookingId:int}")]
    public async Task<BookingViewModel> Get(int bookingId) => await _bookingService.GetBookingByIdAsync(bookingId);

    [HttpPost]
    public async Task<ResourceIdViewModel> Post(BookingBindingModel model)
    {
        var booking = await _bookingService.CreateBookingAsync(new BookingViewModel()
        {
            RentalId = model.RentalId,
            Nights = model.Nights,
            Start = model.Start.Date
        });

        return booking;
    }
}