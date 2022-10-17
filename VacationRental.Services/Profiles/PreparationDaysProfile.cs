using AutoMapper;
using VacationRental.Data.Entities;
using VacationRental.Model.ViewModels;

namespace VacationRental.Services.Profiles;

public class PreparationDaysProfile : Profile
{
    public PreparationDaysProfile()
    {
        CreateMap<BookingViewModel, PreparationDays>()
            .ForMember(dest => dest.Start, opt => opt.MapFrom(src => src.Start.Date.AddDays(src.Nights)));

        CreateMap<Rental, PreparationDays>()
            .ForMember(dest => dest.Days, opt => opt.MapFrom(src => src.PreparationTimeInDays));
        ;
    }
}