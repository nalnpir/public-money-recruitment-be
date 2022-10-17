using VacationRental.Data.Entities.Contracts;

namespace VacationRental.Data.Entities;

public class Booking : InMemoryEntity
{
    public int RentalId { get; set; }
    public DateTime Start { get; set; }
    public int Nights { get; set; }
    public int Unit { get; set; }

}