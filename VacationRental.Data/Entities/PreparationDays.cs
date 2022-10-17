using VacationRental.Data.Entities.Contracts;

namespace VacationRental.Data.Entities;

public class PreparationDays : InMemoryEntity
{
    public int RentalId { get; set; }
    public int Unit { get; set; }
    public DateTime Start { get; set; }
    public int Days { get; set; }
}