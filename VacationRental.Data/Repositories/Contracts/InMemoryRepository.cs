using AutoMapper;
using VacationRental.Data.Entities.Contracts;

namespace VacationRental.Data.Repositories.Contracts;

public abstract class InMemoryRepository<TEntity> : IRepository<TEntity> where TEntity : InMemoryEntity
{
    protected readonly IDictionary<int, TEntity> Entities;
    private readonly IMapper _mapper;

    protected InMemoryRepository(IDictionary<int, TEntity> entities, IMapper mapper)
    {
        Entities = entities;
        _mapper = mapper;
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

    public Task<TEntity> UpdateAsync(TEntity current, TEntity updated)
    {
        if (!Entities.ContainsKey(current.Id))
            throw new KeyNotFoundException("Entity not found");

        var result = _mapper.Map(updated, current);

        Entities[current.Id] = result;

        return Task.FromResult(Entities[result.Id]);
    }
}