using FinanceTracker.Application.Abstractions;
using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Application.Services;

public sealed class AccountsService
{
    private readonly IRepository<BankAccount> _repo;

    public AccountsService(IRepository<BankAccount> repo) => _repo = repo;

    public void Add(BankAccount account) => _repo.Add(account);
    public void Update(BankAccount account) => _repo.Update(account);
    public void Delete(Guid id) => _repo.Delete(id);

    public BankAccount? Get(Guid id) => _repo.Get(id);
    public IReadOnlyList<BankAccount> List() => _repo.GetAll();
}