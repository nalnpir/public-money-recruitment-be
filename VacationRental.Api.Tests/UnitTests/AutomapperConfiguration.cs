using System.Collections.Generic;
using AutoMapper;
using VacationRental.Services.Profiles;

namespace VacationRental.Api.Tests.UnitTests
{
    public static class AutomapperConfiguration
    {
        public static IMapper CreateAutomapper()
        {
            var bookingProfile = new BookingProfile();
            var rentalProfile = new RentalProfile();
            var preparationDaysProfile = new PreparationDaysProfile();

            var profiles = new List<Profile>()
            {
                bookingProfile,
                rentalProfile,
                preparationDaysProfile
            };

            var config = new MapperConfiguration(cfg => cfg.AddProfiles(profiles));

            return config.CreateMapper();
        }
    }
}
