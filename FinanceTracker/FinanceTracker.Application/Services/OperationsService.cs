using FinanceTracker.Application.Abstractions;
using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Application.Services;

public sealed class OperationsService
{
    private readonly IRepository<Operation> _repo;

    public OperationsService(IRepository<Operation> repo) => _repo = repo;

    public void Add(Operation op) => _repo.Add(op);
    public void Update(Operation op) => _repo.Update(op);
    public void Delete(Guid id) => _repo.Delete(id);

    public Operation? Get(Guid id) => _repo.Get(id);
    public IReadOnlyList<Operation> List() => _repo.GetAll();
}