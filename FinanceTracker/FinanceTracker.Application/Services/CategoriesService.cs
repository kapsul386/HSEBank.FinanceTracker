using FinanceTracker.Application.Abstractions;
using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Application.Services;

public sealed class CategoriesService
{
    private readonly IRepository<Category> _repo;

    public CategoriesService(IRepository<Category> repo) => _repo = repo;

    public void Add(Category c) => _repo.Add(c);
    public void Update(Category c) => _repo.Update(c);
    public void Delete(Guid id) => _repo.Delete(id);

    public Category? Get(Guid id) => _repo.Get(id);
    public IReadOnlyList<Category> List() => _repo.GetAll();
}