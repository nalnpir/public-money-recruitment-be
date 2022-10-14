using AutoMapper;
using VacationRental.Data.Entities;
using VacationRental.Data.Repositories.Contracts;

namespace VacationRental.Data.Repositories;

public class BookingRepository : InMemoryRepository<Booking>, IBookingRepository
{
    public BookingRepository(IDictionary<int, Booking> entities, IMapper mapper) : base(entities, mapper)
    {
    }
}