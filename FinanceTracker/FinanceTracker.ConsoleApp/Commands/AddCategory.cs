using FinanceTracker.Application.Commands;
using FinanceTracker.Application.Services;
using FinanceTracker.Domain.Entities;
using FinanceTracker.Domain.Factories;

namespace FinanceTracker.ConsoleApp.Commands;

/// <summary>
/// Command that adds a new financial category (Income or Expense).
/// Implements the <b>Command</b> pattern — encapsulates a single user action.
/// </summary>
public sealed class AddCategory : ICommand
{
    private readonly CategoriesService _categories;
    private readonly IDomainFactory _factory;

    /// <summary>
    /// Console command name.
    /// </summary>
    public string Name => "add-category";

    /// <summary>
    /// Short description displayed in the help list.
    /// </summary>
    public string Description => "Add a financial category (Income/Expense)";

    /// <summary>
    /// Initializes a new instance of the <see cref="AddCategory"/> command.
    /// </summary>
    /// <param name="categories">Service responsible for managing categories.</param>
    /// <param name="factory">Factory for creating validated domain entities.</param>
    public AddCategory(CategoriesService categories, IDomainFactory factory)
    {
        _categories = categories;
        _factory = factory;
    }

    /// <summary>
    /// Executes the command: asks the user for input and creates a new category.
    /// </summary>
    public void Run()
    {
        Console.Write("Type (Income/Expense): ");
        var typeStr = (Console.ReadLine() ?? "").Trim();

        if (!Enum.TryParse<MoneyFlowType>(typeStr, true, out var type))
        {
            Console.WriteLine("Error: invalid category type.");
            return;
        }

        Console.Write("Category name: ");
        var name = Console.ReadLine() ?? "";

        var cat = _factory.CreateCategory(type, name);
        _categories.Add(cat);

        Console.WriteLine($"Category [{type}] '{cat.Name}' created successfully (ID: {cat.Id}).");
    }
}