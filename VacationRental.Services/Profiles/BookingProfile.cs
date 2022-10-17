using AutoMapper;
using VacationRental.Data.Entities;
using VacationRental.Model.ViewModels;

namespace VacationRental.Services.Profiles;

public class BookingProfile : Profile
{
    public BookingProfile()
    {
        CreateMap<BookingViewModel, Booking>()
            .ForMember(dest => dest.Start, opt => opt.MapFrom(src => src.Start.Date));
        CreateMap<Booking, BookingViewModel>()
            .ForMember(dest => dest.Start, opt => opt.MapFrom(src => src.Start.Date));
        CreateMap<Booking, Booking>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());

    }
}