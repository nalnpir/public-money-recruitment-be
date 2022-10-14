using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VacationRental.Model.BiindingModels;
using VacationRental.Model.ViewModels;
using VacationRental.Services.Services.Contracts;

namespace VacationRental.Api.Controllers;

[Route("api/v1/rentals")]
[ApiController]
public class RentalsController : ControllerBase
{
    private readonly IRentalService _rentalService;

    public RentalsController(IRentalService rentalService)
    {
        _rentalService = rentalService;
    }

    [HttpGet]
    [Route("{rentalId:int}")]
    public async Task<RentalViewModel> Get(int rentalId) => await _rentalService.GetRentalByIdAsync(rentalId);

    [HttpPost]
    public async Task<ResourceIdViewModel> Post(RentalBindingModel model)
    {
        var rental = await _rentalService.CreateRentalAsync(new RentalViewModel
        {
            Units = model.Units
        });

        return rental;
    }
}