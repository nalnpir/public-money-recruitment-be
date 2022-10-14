using AutoMapper;
using VacationRental.Data.Entities;
using VacationRental.Model.ViewModels;

namespace VacationRental.Services.Profiles;

internal class RentalProfile : Profile
{
    public RentalProfile()
    {
        CreateMap<RentalViewModel, Rental>();
        CreateMap<Rental, RentalViewModel>();
        CreateMap<Rental, Rental>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
    }

}