using AutoMapper;
using VacationRental.Data.Entities;
using VacationRental.Data.Repositories.Contracts;

namespace VacationRental.Data.Repositories;

public class RentalRepository : InMemoryRepository<Rental>, IRentalRepository
{
    public RentalRepository(IDictionary<int, Rental> entities, IMapper mapper) : base(entities, mapper)
    {
    }
}