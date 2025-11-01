namespace FinanceTracker.Infrastructure.Repositories;

public interface IReadRepository<T>
{
    T? Get(Guid id);
    IReadOnlyList<T> GetAll();
}

public interface IWriteRepository<T>
{
    void Add(T entity);
    void Update(T entity);
    void Delete(Guid id);
}

public interface IRepository<T> : IReadRepository<T>, IWriteRepository<T> { }