using FinanceTracker.Application.Commands;
using FinanceTracker.Application.Services;
using FinanceTracker.Domain.Entities;

namespace FinanceTracker.ConsoleApp.Commands;

public sealed class RecalculateBalance : ICommand
{
    private readonly AccountsService _accounts;
    private readonly OperationsService _operations;
    private readonly CategoriesService _categories;

    public string Name => "recalc-balance";
    public string Description => "Пересчитать баланс счёта на основе операций";

    public RecalculateBalance(AccountsService accounts, OperationsService operations, CategoriesService categories)
    { _accounts = accounts; _operations = operations; _categories = categories; }

    public void Run()
    {
        Console.Write("Id счёта: ");
        if (!Guid.TryParse(Console.ReadLine(), out var accountId))
        { Console.WriteLine("Неверный Id."); return; }

        var acc = _accounts.Get(accountId);
        if (acc is null) { Console.WriteLine("Счёт не найден."); return; }

        var ops = _operations.List().Where(o => o.BankAccountId == accountId).ToList();
        if (ops.Count == 0) { Console.WriteLine("Операций по счёту нет. Баланс не изменён."); return; }

        decimal income = 0, expense = 0;
        foreach (var o in ops)
        {
            if (o.Type == MoneyFlowType.Income) income += o.Amount;
            else expense += o.Amount;
        }

        var newBalance = income - expense;
        var delta = newBalance - acc.Balance;
        if (delta == 0) { Console.WriteLine("Баланс уже соответствует операциям."); return; }

        if (delta > 0) acc.Credit(delta); else acc.Debit(-delta);
        _accounts.Update(acc);

        Console.WriteLine($"OK: баланс пересчитан → {newBalance}");
    }
}