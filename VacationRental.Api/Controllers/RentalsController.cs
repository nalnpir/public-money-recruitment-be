using System.Threading.Tasks;
using AutoMapper;
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
    private readonly IMapper _mapper;

    public RentalsController(IRentalService rentalService, IMapper mapper)
    {
        _rentalService = rentalService;
        _mapper = mapper;
    }

    [HttpGet]
    [Route("{rentalId:int}")]
    public async Task<RentalViewModel> Get(int rentalId) => await _rentalService.GetRentalByIdAsync(rentalId);

    [HttpPost]
    public async Task<ResourceIdViewModel> Post(RentalBindingModel model)
    {
        var rentalViewModel = _mapper.Map<RentalViewModel>(model);

        var rental = await _rentalService.CreateRentalAsync(rentalViewModel);

        return rental;
    }

    [HttpPut]
    [Route("{rentalId:int}")]
    public async Task<RentalViewModel> Put(int rentalId, RentalBindingModel model)
    {
        var rentalViewModel = _mapper.Map<RentalViewModel>(model);

        var rental = await _rentalService.UpdateRentalAsync(rentalId, rentalViewModel);

        return rental;
    }
}