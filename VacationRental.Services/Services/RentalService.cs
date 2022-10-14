using AutoMapper;
using VacationRental.Data.Entities;
using VacationRental.Data.Repositories.Contracts;
using VacationRental.Model.ViewModels;
using VacationRental.Services.Services.Contracts;

namespace VacationRental.Services.Services;

public class RentalService : IRentalService
{
    private readonly IRentalRepository _rentalRepository;
    private readonly IMapper _mapper;

    public RentalService(IRentalRepository rentalRepository, IMapper mapper)
    {
        _rentalRepository = rentalRepository;
        _mapper = mapper;
    }
    public async Task<RentalViewModel> GetRentalByIdAsync(int id)
    {
        try
        {
            return _mapper.Map<RentalViewModel>(await _rentalRepository.GetByIdAsync(id));
        }
        catch (KeyNotFoundException)
        {
            throw new ApplicationException("Rental not found");
        }
    }

    public async Task<ResourceIdViewModel> CreateRentalAsync(RentalViewModel model)
    {
        var rental = await _rentalRepository.InsertAsync(_mapper.Map<Rental>(model));

        var key = new ResourceIdViewModel { Id = rental.Id };

        return key;
    }
}