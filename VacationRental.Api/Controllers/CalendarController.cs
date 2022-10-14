using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VacationRental.Model.ViewModels;
using VacationRental.Services.Services.Contracts;

namespace VacationRental.Api.Controllers;

[Route("api/v1/calendar")]
[ApiController]
public class CalendarController : ControllerBase
{
    private readonly ICalendarService _calendarService;

    public CalendarController(ICalendarService calendarService)
    {
        _calendarService = calendarService;
    }

    [HttpGet]
    public async Task<CalendarViewModel> Get(int rentalId, DateTime start, int nights)
    {
        return await _calendarService.GetCalendarAvailabilityAsync(rentalId, start, nights);
    }
}