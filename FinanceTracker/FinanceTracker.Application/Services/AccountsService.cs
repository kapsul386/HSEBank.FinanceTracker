using FinanceTracker.Application.Abstractions;
using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Application.Services;

/// <summary>
/// Facade service for managing <see cref="BankAccount"/> entities.
/// Encapsulates all operations related to bank accounts and provides
/// a simple interface for use in application layer or commands.
/// </summary>
public sealed class AccountsService
{
    private readonly IRepository<BankAccount> _repo;

    /// <summary>
    /// Initializes a new instance of the <see cref="AccountsService"/> class.
    /// </summary>
    public AccountsService(IRepository<BankAccount> repo) => _repo = repo;

    /// <summary>
    /// Adds a new bank account to the repository.
    /// </summary>
    public void Add(BankAccount account) => _repo.Add(account);

    /// <summary>
    /// Updates an existing bank account.
    /// </summary>
    public void Update(BankAccount account) => _repo.Update(account);

    /// <summary>
    /// Deletes a bank account by its identifier.
    /// </summary>
    public void Delete(Guid id) => _repo.Delete(id);

    /// <summary>
    /// Retrieves a bank account by its identifier.
    /// Returns <c>null</c> if not found.
    /// </summary>
    public BankAccount? Get(Guid id) => _repo.Get(id);

    /// <summary>
    /// Returns all bank accounts from the repository.
    /// </summary>
    public IReadOnlyList<BankAccount> List() => _repo.GetAll();
}