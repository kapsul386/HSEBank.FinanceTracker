namespace FinanceTracker.Domain.Entities;

/// <summary>
/// Represents a financial category, either for income or expense.
/// Used to classify operations (e.g., "Salary", "Café", "Health").
/// </summary>
public sealed class Category
{
    /// <summary>
    /// Unique identifier of the category.
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// Type of money flow this category belongs to — <see cref="MoneyFlowType.Income"/> or <see cref="MoneyFlowType.Expense"/>.
    /// </summary>
    public MoneyFlowType Type { get; }

    /// <summary>
    /// Display name of the category.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Category"/> class with validation.
    /// </summary>
    /// <param name="id">Unique identifier of the category.</param>
    /// <param name="type">Type of category (income or expense).</param>
    /// <param name="name">Name of the category (cannot be empty).</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="name"/> is null or empty.</exception>
    public Category(Guid id, MoneyFlowType type, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required", nameof(name));

        Id = id;
        Type = type;
        Name = name.Trim();
    }

    /// <summary>
    /// Renames the category while enforcing non-empty name rule.
    /// </summary>
    /// <param name="newName">New category name.</param>
    public void Rename(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new ArgumentException("Name is required", nameof(newName));
        Name = newName.Trim();
    }
}