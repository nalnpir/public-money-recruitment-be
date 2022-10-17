using VacationRental.Data.Entities.Contracts;

namespace VacationRental.Data.Repositories.Contracts;

public abstract class InMemoryRepository<TEntity> : IRepository<TEntity> where TEntity : InMemoryEntity
{
    protected readonly IDictionary<int, TEntity> Entities;

    protected InMemoryRepository(IDictionary<int, TEntity> entities)
    {
        Entities = entities;
    }
    public Task<TEntity> GetByIdAsync(int id)
    {
        if (!Entities.ContainsKey(id))
            throw new KeyNotFoundException("Entity not found");

        return Task.FromResult(Entities[id]);
    }

    public Task<bool> ExistsAsync(int id)
    {
        return Task.FromResult(Entities.ContainsKey(id));
    }

    public Task<ICollection<TEntity>> GetAllAsync()
    {
        return Task.FromResult(Entities.Values);
    }

    public Task<TEntity> InsertAsync(TEntity entity)
    {
        var key = Entities.Keys.Count + 1;
        entity.Id = key;

        Entities.Add(key, entity);

        return Task.FromResult(Entities[entity.Id]);
    }

    public Task<TEntity> UpdateAsync(TEntity updated)
    {
        if (!Entities.ContainsKey(updated.Id))
            throw new KeyNotFoundException("Entity not found");

        Entities[updated.Id] = updated;
        
        return Task.FromResult(Entities[updated.Id]);
    }

    public async Task UpdateBulkAsync(IEnumerable<TEntity> updatedEntities)
    {
        foreach (var entity in updatedEntities)
        {
            await UpdateAsync(entity);
        }
    }
}