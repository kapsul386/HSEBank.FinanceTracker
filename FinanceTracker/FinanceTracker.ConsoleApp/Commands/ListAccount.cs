using FinanceTracker.Application.Commands;
using FinanceTracker.Application.Services;

namespace FinanceTracker.ConsoleApp.Commands;

/// <summary>
/// Command that displays all existing accounts.
/// Command: <c>list-accounts</c>.
/// </summary>
public sealed class ListAccounts : ICommand
{
    private readonly AccountsService _accounts;

    /// <summary>Console command name.</summary>
    public string Name => "list-accounts";

    /// <summary>Short description shown in the help list.</summary>
    public string Description => "Show all accounts";

    public ListAccounts(AccountsService accounts) => _accounts = accounts;

    /// <summary>
    /// Executes the command:
    /// <list type="number">
    /// <item>Retrieves all accounts from the service.</item>
    /// <item>Prints their Id, name, and balance to the console.</item>
    /// <item>If no accounts exist, displays <c>(empty)</c>.</item>
    /// </list>
    /// </summary>
    public void Run()
    {
        var all = _accounts.List();
        if (all.Count == 0)
        {
            Console.WriteLine("(empty)");
            return;
        }

        foreach (var a in all)
            Console.WriteLine($"{a.Id} | {a.Name} | {a.Balance}");
    }
}