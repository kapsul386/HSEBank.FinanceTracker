using FinanceTracker.Application.Commands;
using FinanceTracker.Application.Services;

namespace FinanceTracker.ConsoleApp.Commands;

/// <summary>
/// Command that deletes a bank account by its ID.
/// Implements the <b>Command</b> pattern — encapsulates a single user action.
/// </summary>
public sealed class DeleteAccount : ICommand
{
    private readonly AccountsService _accounts;

    /// <summary>
    /// Console command name.
    /// </summary>
    public string Name => "delete-account";

    /// <summary>
    /// Short description displayed in the help list.
    /// </summary>
    public string Description => "Delete a bank account by ID";

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteAccount"/> command.
    /// </summary>
    /// <param name="accounts">Service responsible for managing bank accounts.</param>
    public DeleteAccount(AccountsService accounts)
    {
        _accounts = accounts;
    }

    /// <summary>
    /// Executes the command: asks for an account ID, confirms, and deletes it if found.
    /// </summary>
    public void Run()
    {
        Console.Write("Enter account ID to delete: ");
        var idText = Console.ReadLine();

        if (!Guid.TryParse(idText, out var id))
        {
            Console.WriteLine("Error: invalid ID format.");
            return;
        }

        var acc = _accounts.Get(id);
        if (acc is null)
        {
            Console.WriteLine("Error: account not found.");
            return;
        }

        Console.Write($"Are you sure you want to delete account '{acc.Name}'? (y/n): ");
        var confirm = Console.ReadLine()?.Trim().ToLowerInvariant();

        if (confirm != "y")
        {
            Console.WriteLine("Operation cancelled by user.");
            return;
        }

        try
        {
            _accounts.Delete(id);
            Console.WriteLine("Account deleted successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error while deleting: {ex.Message}");
        }
    }
}
