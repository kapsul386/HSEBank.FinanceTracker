using FinanceTracker.Application.Commands;
using FinanceTracker.Application.Services;
using FinanceTracker.Domain.Entities;
using FinanceTracker.Domain.Factories;

namespace FinanceTracker.ConsoleApp.Commands;

/// <summary>
/// Command that adds a new financial operation (Income or Expense).
/// Implements the <b>Command</b> pattern — encapsulates a single user action.
/// </summary>
public sealed class AddOperation : ICommand
{
    private readonly OperationsService _ops;
    private readonly AccountsService _accounts;
    private readonly CategoriesService _categories;
    private readonly IDomainFactory _factory;

    /// <summary>
    /// Console command name.
    /// </summary>
    public string Name => "add-operation";

    /// <summary>
    /// Short description displayed in the help list.
    /// </summary>
    public string Description => "Add a financial operation (Income/Expense)";

    /// <summary>
    /// Initializes a new instance of the <see cref="AddOperation"/> command.
    /// </summary>
    public AddOperation(
        OperationsService ops,
        AccountsService accounts,
        CategoriesService categories,
        IDomainFactory factory)
    {
        _ops = ops;
        _accounts = accounts;
        _categories = categories;
        _factory = factory;
    }

    /// <summary>
    /// Executes the command: collects user input, validates references, and creates an operation.
    /// </summary>
    public void Run()
    {
        Console.Write("Type (Income/Expense): ");
        var typeStr = (Console.ReadLine() ?? "").Trim();
        if (!Enum.TryParse<MoneyFlowType>(typeStr, true, out var type))
        {
            Console.WriteLine("Error: invalid operation type.");
            return;
        }

        Console.Write("Account ID: ");
        var accIdText = Console.ReadLine();

        Console.Write("Category ID: ");
        var catIdText = Console.ReadLine();

        Console.Write("Amount: ");
        var amountText = Console.ReadLine();

        Console.Write("Date (YYYY-MM-DD): ");
        var dateText = Console.ReadLine();

        Console.Write("Description (optional): ");
        var desc = Console.ReadLine();

        // Basic input validation
        if (!Guid.TryParse(accIdText, out var accId) ||
            !Guid.TryParse(catIdText, out var catId) ||
            !decimal.TryParse(amountText, out var amount) ||
            !DateOnly.TryParse(dateText, out var date))
        {
            Console.WriteLine("Error: invalid input.");
            return;
        }

        // Referential integrity checks
        if (_accounts.Get(accId) is null)
        {
            Console.WriteLine("Error: account not found.");
            return;
        }

        if (_categories.Get(catId) is null)
        {
            Console.WriteLine("Error: category not found.");
            return;
        }

        // Create and persist the operation via the domain factory + facade
        var op = _factory.CreateOperation(type, accId, amount, date, catId, desc);
        _ops.Add(op);

        Console.WriteLine($"Operation added: [{type}] {amount} on {date:yyyy-MM-dd}");
    }
}
