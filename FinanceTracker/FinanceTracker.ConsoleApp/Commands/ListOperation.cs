using FinanceTracker.Application.Commands;
using FinanceTracker.Application.Services;

namespace FinanceTracker.ConsoleApp.Commands;

/// <summary>
/// Command that displays all operations.
/// Command: <c>list-operations</c>.
/// </summary>
public sealed class ListOperations : ICommand
{
    private readonly OperationsService _ops;

    /// <summary>Console command name.</summary>
    public string Name => "list-operations";

    /// <summary>Short description shown in the help list.</summary>
    public string Description => "Show all operations";

    public ListOperations(OperationsService ops) => _ops = ops;

    /// <summary>
    /// Executes the command:
    /// <list type="number">
    /// <item>Retrieves all operations from the service.</item>
    /// <item>Prints their Id, type, account, category, amount, date, and description to the console.</item>
    /// <item>If no operations exist, displays <c>(empty)</c>.</item>
    /// </list>
    /// </summary>
    public void Run()
    {
        var all = _ops.List();
        if (all.Count == 0)
        {
            Console.WriteLine("(empty)");
            return;
        }

        foreach (var o in all)
            Console.WriteLine($"{o.Id} | {o.Type} | acc:{o.BankAccountId} | cat:{o.CategoryId} | {o.Amount} | {o.Date} | {o.Description}");
    }
}