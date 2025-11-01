namespace FinanceTracker.Infrastructure.Repositories;

public sealed class MemoryRepository<T> : IRepository<T> where T : class
{
    private readonly Dictionary<Guid, T> _items = new();
    private readonly Func<T, Guid> _id;

    public MemoryRepository(Func<T, Guid> idGetter)
    {
        _id = idGetter ?? throw new ArgumentNullException(nameof(idGetter));
    }

    public void Add(T entity)    => _items[_id(entity)] = entity;
    public void Update(T entity) => _items[_id(entity)] = entity;
    public void Delete(Guid id)  => _items.Remove(id);

    public T? Get(Guid id) => _items.TryGetValue(id, out var v) ? v : null;

    public IReadOnlyList<T> GetAll() => _items.Values.ToList();
}