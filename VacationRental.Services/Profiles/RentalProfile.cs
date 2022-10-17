using AutoMapper;
using VacationRental.Data.Entities;
using VacationRental.Model.BiindingModels;
using VacationRental.Model.ViewModels;

namespace VacationRental.Services.Profiles;

public class RentalProfile : Profile
{
    public RentalProfile()
    {
        CreateMap<RentalBindingModel, RentalViewModel>();
        CreateMap<RentalViewModel, Rental>();
        CreateMap<Rental, RentalViewModel>();
        CreateMap<Rental, Rental>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
    }

}