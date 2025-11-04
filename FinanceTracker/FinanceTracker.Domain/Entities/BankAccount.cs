namespace FinanceTracker.Domain.Entities;

/// <summary>
/// Represents a bank account in the finance tracking domain.
/// Encapsulates balance management and enforces domain validation rules.
/// </summary>
public sealed class BankAccount
{
    /// <summary>
    /// Unique identifier of the bank account.
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// Name of the bank account (e.g., "Main", "Savings").
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Current account balance.
    /// </summary>
    public decimal Balance { get; private set; }

    /// <summary>
    /// Creates a new instance of <see cref="BankAccount"/> with validation.
    /// </summary>
    /// <param name="id">Unique identifier of the account.</param>
    /// <param name="name">Display name of the account.</param>
    /// <param name="balance">Initial account balance (must be non-negative).</param>
    /// <exception cref="ArgumentException">Thrown if name is null or empty.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if balance is negative.</exception>
    public BankAccount(Guid id, string name, decimal balance)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required", nameof(name));
        if (balance < 0)
            throw new ArgumentOutOfRangeException(nameof(balance), "Initial balance cannot be negative");

        Id = id;
        Name = name.Trim();
        Balance = balance;
    }

    /// <summary>
    /// Renames the account while enforcing non-empty name rule.
    /// </summary>
    /// <param name="newName">New name of the account.</param>
    public void Rename(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new ArgumentException("Name is required", nameof(newName));
        Name = newName.Trim();
    }

    /// <summary>
    /// Adds funds to the account balance (credit operation).
    /// </summary>
    /// <param name="amount">Amount to credit (must be positive).</param>
    public void Credit(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "Credit amount must be positive.");
        Balance += amount;
    }

    /// <summary>
    /// Deducts funds from the account balance (debit operation).
    /// </summary>
    /// <param name="amount">Amount to debit (must be positive).</param>
    public void Debit(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "Debit amount must be positive.");
        Balance -= amount;
    }
}
