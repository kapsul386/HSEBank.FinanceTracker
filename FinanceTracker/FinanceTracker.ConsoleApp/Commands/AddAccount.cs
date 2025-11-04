using FinanceTracker.Application.Commands;
using FinanceTracker.Application.Services;
using FinanceTracker.Domain.Factories;

namespace FinanceTracker.ConsoleApp.Commands;

/// <summary>
/// Command that adds a new bank account to the system.
/// Implements the <b>Command</b> design pattern: encapsulates a single user action.
/// </summary>
public sealed class AddAccount : ICommand
{
    private readonly AccountsService _accounts;
    private readonly IDomainFactory _factory;

    /// <summary>
    /// Command name used in the console interface.
    /// </summary>
    public string Name => "add-account";

    /// <summary>
    /// Short description displayed in the help list.
    /// </summary>
    public string Description => "Add a new bank account";

    /// <summary>
    /// Initializes a new instance of the <see cref="AddAccount"/> command.
    /// </summary>
    /// <param name="accounts">Service responsible for managing bank accounts.</param>
    /// <param name="factory">Factory for creating validated domain entities.</param>
    public AddAccount(AccountsService accounts, IDomainFactory factory)
    {
        _accounts = accounts;
        _factory = factory;
    }

    /// <summary>
    /// Executes the command: asks for user input and creates a new account.
    /// </summary>
    public void Run()
    {
        Console.Write("Account name: ");
        var name = Console.ReadLine() ?? "";

        Console.Write("Initial balance: ");
        var text = Console.ReadLine();

        if (!decimal.TryParse(text, out var balance))
        {
            Console.WriteLine("Error: invalid amount.");
            return;
        }

        var acc = _factory.CreateBankAccount(name, balance);
        _accounts.Add(acc);

        Console.WriteLine($"Account '{acc.Name}' created successfully with balance {acc.Balance}.");
    }
}