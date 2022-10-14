namespace VacationRental.Data.Repositories.Contracts;

public interface IRepository<TEntity> where TEntity : class
{
    Task<TEntity> GetByIdAsync(int id);
    Task<ICollection<TEntity>> GetAllAsync();
    Task<TEntity> InsertAsync(TEntity entity);
    Task<TEntity> UpdateAsync(TEntity current, TEntity updated);
    Task<bool> ExistsAsync(int id);
}