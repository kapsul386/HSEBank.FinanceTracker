using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Domain.Factories;

/// <summary>
/// Centralized factory for creating validated domain entities.
/// Implements the <b>Factory</b> design pattern to ensure that
/// all domain objects are created consistently and safely.
/// </summary>
public sealed class DomainFactory : IDomainFactory
{
    /// <summary>
    /// Creates a new <see cref="BankAccount"/> with validation.
    /// </summary>
    /// <param name="name">Account name (cannot be null or whitespace).</param>
    /// <param name="initialBalance">Initial balance (must be non-negative).</param>
    /// <returns>New valid <see cref="BankAccount"/> instance.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="name"/> is invalid.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="initialBalance"/> is negative.</exception>
    public BankAccount CreateBankAccount(string name, decimal initialBalance = 0)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required", nameof(name));
        if (initialBalance < 0)
            throw new ArgumentOutOfRangeException(nameof(initialBalance), "Initial balance cannot be negative");

        return new BankAccount(Guid.NewGuid(), name.Trim(), initialBalance);
    }

    /// <summary>
    /// Creates a new <see cref="Category"/> with validation.
    /// </summary>
    /// <param name="type">Category type — income or expense.</param>
    /// <param name="name">Category name (cannot be null or whitespace).</param>
    /// <returns>New valid <see cref="Category"/> instance.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="name"/> is invalid.</exception>
    public Category CreateCategory(MoneyFlowType type, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required", nameof(name));

        return new Category(Guid.NewGuid(), type, name.Trim());
    }

    /// <summary>
    /// Creates a new <see cref="Operation"/> with validation.
    /// </summary>
    /// <param name="type">Operation type (income or expense).</param>
    /// <param name="accountId">Identifier of the related bank account.</param>
    /// <param name="amount">Operation amount (must be positive).</param>
    /// <param name="date">Date of the operation.</param>
    /// <param name="categoryId">Identifier of the related category.</param>
    /// <param name="description">Optional description of the operation.</param>
    /// <returns>New valid <see cref="Operation"/> instance.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="amount"/> is not positive.</exception>
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
