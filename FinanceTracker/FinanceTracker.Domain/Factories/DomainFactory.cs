using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Domain.Factories;

public sealed class DomainFactory : IDomainFactory
{
    public BankAccount CreateBankAccount(string name, decimal initialBalance = 0)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required", nameof(name));
        if (initialBalance < 0)
            throw new ArgumentOutOfRangeException(nameof(initialBalance), "Initial balance cannot be negative");

        return new BankAccount(Guid.NewGuid(), name.Trim(), initialBalance);
    }

    public Category CreateCategory(MoneyFlowType type, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required", nameof(name));

        return new Category(Guid.NewGuid(), type, name.Trim());
    }

    public Operation CreateOperation(
        MoneyFlowType type,
        Guid accountId,
        decimal amount,
        DateOnly date,
        Guid categoryId,
        string? description = null)
    {
        if (amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be positive");

        return new Operation(
            Guid.NewGuid(),
            type,
            accountId,
            amount,
            date,
            categoryId,
            description);
    }
}