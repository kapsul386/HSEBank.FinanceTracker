using System.Collections.Concurrent;
using FinanceTracker.Application.Abstractions;

namespace FinanceTracker.Infrastructure.Repositories;

/// <summary>
/// In-memory caching proxy for any <see cref="IRepository{T}"/> implementation.
/// Implements the <b>Proxy</b> design pattern to transparently add caching behavior
/// without modifying the original repository.
/// </summary>
/// <typeparam name="T">Type of the domain entity.</typeparam>
public sealed class CachedRepositoryProxy<T> : IRepository<T> where T : class
{
    private readonly IRepository<T> _inner;
    private readonly Func<T, Guid> _id;
    private readonly ConcurrentDictionary<Guid, T> _byId = new();
    private volatile IReadOnlyList<T>? _allCache;

    /// <summary>
    /// Initializes a new instance of the <see cref="CachedRepositoryProxy{T}"/> class.
    /// </summary>
    /// <param name="inner">The underlying repository (e.g., <see cref="MemoryRepository{T}"/>).</param>
    /// <param name="idAccessor">Function that returns the unique ID of an entity.</param>
    public CachedRepositoryProxy(IRepository<T> inner, Func<T, Guid> idAccessor)
    {
        _inner = inner;
        _id = idAccessor;
    }

    /// <summary>
    /// Adds an entity to the repository and updates the cache.
    /// </summary>
    /// <param name="item">Entity to add.</param>
    public void Add(T item)
    {
        _inner.Add(item);
        var id = _id(item);
        _byId[id] = item;
        _allCache = null; // invalidate global cache
    }

    /// <summary>
    /// Updates an entity in the repository and refreshes the cache entry.
    /// </summary>
    /// <param name="item">Entity to update.</param>
    public void Update(T item)
    {
        _inner.Update(item);
        var id = _id(item);
        _byId[id] = item;
        _allCache = null;
    }

    /// <summary>
    /// Deletes an entity by ID from both the repository and cache.
    /// </summary>
    /// <param name="id">Unique identifier of the entity to delete.</param>
    public void Delete(Guid id)
    {
        _inner.Delete(id);
        _byId.TryRemove(id, out _);
        _allCache = null;
    }

    /// <summary>
    /// Retrieves an entity by ID, using cache when available.
    /// </summary>
    /// <param name="id">Unique identifier of the entity.</param>
    /// <returns>Cached or freshly loaded entity, or <c>null</c> if not found.</returns>
    public T? Get(Guid id)
    {
        if (_byId.TryGetValue(id, out var cached))
            return cached;

        var item = _inner.Get(id);
        if (item is not null)
            _byId[id] = item;

        return item;
    }

    /// <summary>
    /// Retrieves all entities. Uses a cached snapshot if available.
    /// </summary>
    /// <returns>Read-only list of all entities.</returns>
    public IReadOnlyList<T> GetAll()
    {
        var snapshot = _allCache;
        if (snapshot is not null)
            return snapshot;

        snapshot = _inner.GetAll();
        _allCache = snapshot;
        return snapshot;
    }
}
