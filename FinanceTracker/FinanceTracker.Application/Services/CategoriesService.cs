using FinanceTracker.Application.Abstractions;
using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Application.Services;

/// <summary>
/// Facade service for managing <see cref="Category"/> entities.
/// Encapsulates all CRUD operations for income and expense categories.
/// </summary>
public sealed class CategoriesService
{
    private readonly IRepository<Category> _repo;

    /// <summary>
    /// Initializes a new instance of the <see cref="CategoriesService"/> class.
    /// </summary>
    /// <param name="repo">Repository used for storing and retrieving category entities.</param>
    public CategoriesService(IRepository<Category> repo) => _repo = repo;

    /// <summary>
    /// Adds a new category to the repository.
    /// </summary>
    public void Add(Category c) => _repo.Add(c);

    /// <summary>
    /// Updates an existing category in the repository.
    /// </summary>
    public void Update(Category c) => _repo.Update(c);

    /// <summary>
    /// Deletes a category by its identifier.
    /// </summary>
    /// <param name="id">Unique identifier of the category to delete.</param>
    public void Delete(Guid id) => _repo.Delete(id);

    /// <summary>
    /// Retrieves a category by its identifier.
    /// </summary>
    /// <param name="id">Unique identifier of the category to retrieve.</param>
    /// <returns>The matching <see cref="Category"/> or <c>null</c> if not found.</returns>
    public Category? Get(Guid id) => _repo.Get(id);

    /// <summary>
    /// Returns all categories currently stored in the repository.
    /// </summary>
    public IReadOnlyList<Category> List() => _repo.GetAll();
}