using System.Collections.Concurrent;
using FinanceTracker.Application.Abstractions;

namespace FinanceTracker.Infrastructure.Repositories;

/// <summary>
/// Прокси над IRepository<T> с простым in-memory кэшем.
/// </summary>
public sealed class CachedRepositoryProxy<T> : IRepository<T> where T : class
{
    private readonly IRepository<T> _inner;
    private readonly Func<T, Guid> _id;
    private readonly ConcurrentDictionary<Guid, T> _byId = new();
    private volatile IReadOnlyList<T>? _allCache;

    public CachedRepositoryProxy(IRepository<T> inner, Func<T, Guid> idAccessor)
    {
        _inner = inner;
        _id = idAccessor;
    }

    public void Add(T item)
    {
        _inner.Add(item);
        var id = _id(item);
        _byId[id] = item;
        _allCache = null;
    }

    public void Update(T item)
    {
        _inner.Update(item);
        var id = _id(item);
        _byId[id] = item;
        _allCache = null;
    }

    public void Delete(Guid id)
    {
        _inner.Delete(id);
        _byId.TryRemove(id, out _);
        _allCache = null;
    }

    public T? Get(Guid id)
    {
        if (_byId.TryGetValue(id, out var cached)) return cached;
        var item = _inner.Get(id);
        if (item is not null) _byId[id] = item;
        return item;
    }

    public IReadOnlyList<T> GetAll()
    {
        var snapshot = _allCache;
        if (snapshot is not null) return snapshot;
        snapshot = _inner.GetAll();
        _allCache = snapshot;
        return snapshot;
    }
}