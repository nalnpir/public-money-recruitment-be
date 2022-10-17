using VacationRental.Data.Entities;
using VacationRental.Data.Repositories.Contracts;

namespace VacationRental.Data.Repositories;

public class PreparationDaysRepository : InMemoryRepository<PreparationDays>, IPreparationDaysRepository
{
    public PreparationDaysRepository(IDictionary<int, PreparationDays> entities) : base(entities)
    {
    }
}