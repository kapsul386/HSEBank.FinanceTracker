namespace FinanceTracker.Domain.Entities;

public sealed class Operation
{
    public Guid Id { get; }
    public MoneyFlowType Type { get; }
    public Guid BankAccountId { get; }
    public decimal Amount { get; }
    public DateOnly Date { get; }
    public string? Description { get; private set; }
    public Guid CategoryId { get; }

    public Operation(
        Guid id,
        MoneyFlowType type,
        Guid bankAccountId,
        decimal amount,
        DateOnly date,
        Guid categoryId,
        string? description = null)
    {
        if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be positive");

        Id = id;
        Type = type;
        BankAccountId = bankAccountId;
        Amount = amount;
        Date = date;
        CategoryId = categoryId;
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
    }

    public void UpdateDescription(string? description)
    {
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
    }
}