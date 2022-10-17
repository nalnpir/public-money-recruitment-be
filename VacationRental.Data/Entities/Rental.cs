using VacationRental.Data.Entities.Contracts;

namespace VacationRental.Data.Entities;

public class Rental : InMemoryEntity
{
    public int Units { get; set; }
    public int PreparationTimeInDays { get; set; }
}