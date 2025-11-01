namespace FinanceTracker.Domain.Entities;

public sealed class BankAccount
{
    public Guid Id { get; }
    public string Name { get; private set; }
    public decimal Balance { get; private set; }

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

    public void Rename(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new ArgumentException("Name is required", nameof(newName));
        Name = newName.Trim();
    }

    public void Credit(decimal amount) // пополнение
    {
        if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount));
        Balance += amount;
    }

    public void Debit(decimal amount) // списание
    {
        if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount));
        Balance -= amount;
    }
}