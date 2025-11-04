using FinanceTracker.Application.Commands;
using FinanceTracker.Application.Services;

namespace FinanceTracker.ConsoleApp.Commands;

/// <summary>
/// Command that deletes a financial operation by its ID.
/// Implements the <b>Command</b> design pattern — encapsulates a single user action.
/// </summary>
public sealed class DeleteOperation : ICommand
{
    private readonly OperationsService _operations;

    /// <summary>
    /// Console command name.
    /// </summary>
    public string Name => "delete-operation";

    /// <summary>
    /// Short description displayed in the help list.
    /// </summary>
    public string Description => "Delete an operation by ID";

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteOperation"/> command.
    /// </summary>
    /// <param name="operations">Service responsible for managing operations.</param>
    public DeleteOperation(OperationsService operations)
    {
        _operations = operations;
    }

    /// <summary>
    /// Executes the command: requests an operation ID, confirms, and deletes it.
    /// </summary>
    public void Run()
    {
        Console.Write("Enter operation ID to delete: ");
        var idText = Console.ReadLine();

        if (!Guid.TryParse(idText, out var id))
        {
            Console.WriteLine("Error: invalid ID format.");
            return;
        }

        var op = _operations.Get(id);
        if (op is null)
        {
            Console.WriteLine("Error: operation not found.");
            return;
        }

        Console.Write(
            $"Are you sure you want to delete the operation of {op.Amount} ({op.Type}) dated {op.Date:yyyy-MM-dd}? (y/n): ");
        var confirm = Console.ReadLine()?.Trim().ToLowerInvariant();

        if (confirm != "y")
        {
            Console.WriteLine("Operation cancelled by user.");
            return;
        }

        try
        {
            _operations.Delete(id);
            Console.WriteLine("Operation deleted successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error while deleting: {ex.Message}");
        }
    }
}
