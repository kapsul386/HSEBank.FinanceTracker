using FinanceTracker.Application.Abstractions;
using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Application.Services;

/// <summary>
/// Facade service for managing <see cref="Operation"/> entities.
/// Encapsulates CRUD operations for income and expense transactions.
/// </summary>
public sealed class OperationsService
{
    private readonly IRepository<Operation> _repo;

    /// <summary>
    /// Initializes a new instance of the <see cref="OperationsService"/> class.
    /// </summary>
    /// <param name="repo">Repository for storing and retrieving operations.</param>
    public OperationsService(IRepository<Operation> repo) => _repo = repo;

    /// <summary>
    /// Adds a new operation (income or expense) to the repository.
    /// </summary>
    public void Add(Operation op) => _repo.Add(op);

    /// <summary>
    /// Updates an existing operation in the repository.
    /// </summary>
    public void Update(Operation op) => _repo.Update(op);

    /// <summary>
    /// Deletes an operation by its unique identifier.
    /// </summary>
    /// <param name="id">Unique identifier of the operation to delete.</param>
    public void Delete(Guid id) => _repo.Delete(id);

    /// <summary>
    /// Retrieves an operation by its unique identifier.
    /// </summary>
    /// <param name="id">Unique identifier of the operation.</param>
    /// <returns>The <see cref="Operation"/> instance or <c>null</c> if not found.</returns>
    public Operation? Get(Guid id) => _repo.Get(id);

    /// <summary>
    /// Returns all operations from the repository.
    /// </summary>
    public IReadOnlyList<Operation> List() => _repo.GetAll();
}