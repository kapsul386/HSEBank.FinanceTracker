namespace FinanceTracker.Domain.Entities;

public sealed class Category
{
    public Guid Id { get; }
    public MoneyFlowType Type { get; }
    public string Name { get; private set; }

    public Category(Guid id, MoneyFlowType type, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required", nameof(name));

        Id = id;
        Type = type;
        Name = name.Trim();
    }

    public void Rename(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new ArgumentException("Name is required", nameof(newName));
        Name = newName.Trim();
    }
}