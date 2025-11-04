namespace FinanceTracker.Domain.Entities;

/// <summary>
/// Represents a financial operation — either income or expense.
/// Links to a specific bank account and category and records
/// the amount, date, and optional description.
/// </summary>
public sealed class Operation
{
    /// <summary>
    /// Unique identifier of the operation.
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// Type of the operation — <see cref="MoneyFlowType.Income"/> or <see cref="MoneyFlowType.Expense"/>.
    /// </summary>
    public MoneyFlowType Type { get; }

    /// <summary>
    /// Identifier of the associated <see cref="BankAccount"/>.
    /// </summary>
    public Guid BankAccountId { get; }

    /// <summary>
    /// Monetary amount of the operation. Must be positive.
    /// </summary>
    public decimal Amount { get; }

    /// <summary>
    /// Date of the operation.
    /// </summary>
    public DateOnly Date { get; }

    /// <summary>
    /// Optional text description (e.g., "Dinner", "Salary for October").
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// Identifier of the associated <see cref="Category"/>.
    /// </summary>
    public Guid CategoryId { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Operation"/> class with validation.
    /// </summary>
    /// <param name="id">Unique identifier of the operation.</param>
    /// <param name="type">Type of operation (income or expense).</param>
    /// <param name="bankAccountId">Identifier of the related bank account.</param>
    /// <param name="amount">Amount of the operation (must be positive).</param>
    /// <param name="date">Date of the operation.</param>
    /// <param name="categoryId">Identifier of the related category.</param>
    /// <param name="description">Optional textual description.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when amount is zero or negative.</exception>
    public Operation(
        Guid id,
        MoneyFlowType type,
        Guid bankAccountId,
        decimal amount,
        DateOnly date,
        Guid categoryId,
        string? description = null)
    {
        if (amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be positive");

        Id = id;
        Type = type;
        BankAccountId = bankAccountId;
        Amount = amount;
        Date = date;
        CategoryId = categoryId;
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
    }

    /// <summary>
    /// Updates the textual description of the operation.
    /// Empty or whitespace input is stored as <c>null</c>.
    /// </summary>
    /// <param name="description">New description text (optional).</param>
    public void UpdateDescription(string? description)
    {
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
    }
}
