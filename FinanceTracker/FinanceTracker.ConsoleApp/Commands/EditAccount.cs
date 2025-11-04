using FinanceTracker.Application.Commands;
using FinanceTracker.Application.Services;

namespace FinanceTracker.ConsoleApp.Commands;

/// <summary>
/// Command that edits a bank account's name and/or balance by ID.
/// Implements the <b>Command</b> pattern — encapsulates a single user action.
/// </summary>
public sealed class EditAccount : ICommand
{
    private readonly AccountsService _accounts;

    /// <summary>
    /// Console command name.
    /// </summary>
    public string Name => "edit-account";

    /// <summary>
    /// Short description displayed in the help list.
    /// </summary>
    public string Description => "Rename an account and/or update its balance by ID";

    /// <summary>
    /// Initializes a new instance of the <see cref="EditAccount"/> command.
    /// </summary>
    public EditAccount(AccountsService accounts)
    {
        _accounts = accounts;
    }

    /// <summary>
    /// Executes the command: asks for account ID, optionally renames it and adjusts balance.
    /// </summary>
    public void Run()
    {
        Console.Write("Account ID: ");
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

        // --- Name update ---
        Console.Write($"New name [{acc.Name}] (press Enter to keep): ");
        var newName = Console.ReadLine();
        var nameChanged = false;

        if (!string.IsNullOrWhiteSpace(newName) && newName.Trim() != acc.Name)
        {
            try
            {
                acc.Rename(newName);
                nameChanged = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Rename error: {ex.Message}");
                return;
            }
        }

        // --- Balance update ---
        Console.Write($"New balance [{acc.Balance}] (press Enter to keep): ");
        var balText = Console.ReadLine();
        var balanceChanged = false;

        if (!string.IsNullOrWhiteSpace(balText))
        {
            if (!decimal.TryParse(balText, out var newBalance))
            {
                Console.WriteLine("Error: balance must be a number.");
                return;
            }

            var delta = newBalance - acc.Balance;

            try
            {
                if (delta > 0m)
                {
                    // Increase balance via domain method (credit)
                    acc.Credit(delta);
                    balanceChanged = true;
                }
                else if (delta < 0m)
                {
                    // Decrease balance via domain method (debit)
                    acc.Debit(-delta);
                    balanceChanged = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Balance update error: {ex.Message}");
                return;
            }
        }

        if (!nameChanged && !balanceChanged)
        {
            Console.WriteLine("No changes made.");
            return;
        }

        _accounts.Update(acc);
        Console.WriteLine("Account updated successfully.");
    }
}
