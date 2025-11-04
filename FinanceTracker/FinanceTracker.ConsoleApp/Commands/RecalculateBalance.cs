using FinanceTracker.Application.Commands;
using FinanceTracker.Application.Services;
using FinanceTracker.Domain.Entities;

namespace FinanceTracker.ConsoleApp.Commands;

/// <summary>
/// Command that recalculates an account balance based on its operations.
/// Command: <c>recalc-balance</c>.
/// </summary>
public sealed class RecalculateBalance : ICommand
{
    private readonly AccountsService _accounts;
    private readonly OperationsService _operations;
    private readonly CategoriesService _categories;

    /// <summary>Console command name.</summary>
    public string Name => "recalc-balance";

    /// <summary>Short description shown in the help list.</summary>
    public string Description => "Recalculate account balance based on operations";

    public RecalculateBalance(
        AccountsService accounts,
        OperationsService operations,
        CategoriesService categories)
    {
        _accounts = accounts;
        _operations = operations;
        _categories = categories;
    }

    /// <summary>
    /// Executes the command:
    /// <list type="number">
    /// <item>Prompts for the account ID.</item>
    /// <item>Retrieves all operations for that account.</item>
    /// <item>Calculates new balance as total income minus total expenses.</item>
    /// <item>Updates the account if its balance differs.</item>
    /// </list>
    /// </summary>
    public void Run()
    {
        Console.Write("Account ID: ");
        if (!Guid.TryParse(Console.ReadLine(), out var accountId))
        {
            Console.WriteLine("Error: invalid ID.");
            return;
        }

        var acc = _accounts.Get(accountId);
        if (acc is null)
        {
            Console.WriteLine("Error: account not found.");
            return;
        }

        var ops = _operations.List().Where(o => o.BankAccountId == accountId).ToList();
        if (ops.Count == 0)
        {
            Console.WriteLine("No operations found for this account. Balance not changed.");
            return;
        }

        decimal income = 0, expense = 0;
        foreach (var o in ops)
        {
            if (o.Type == MoneyFlowType.Income)
                income += o.Amount;
            else
                expense += o.Amount;
        }

        var newBalance = income - expense;
        var delta = newBalance - acc.Balance;

        if (delta == 0)
        {
            Console.WriteLine("Balance already matches operations.");
            return;
        }

        if (delta > 0)
            acc.Credit(delta);
        else
            acc.Debit(-delta);

        _accounts.Update(acc);

        Console.WriteLine($"OK: balance recalculated => {newBalance}");
    }
}
