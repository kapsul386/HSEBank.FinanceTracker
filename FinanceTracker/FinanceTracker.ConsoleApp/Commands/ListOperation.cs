using FinanceTracker.Application.Commands;
using FinanceTracker.Application.Services;

namespace FinanceTracker.ConsoleApp.Commands;

public sealed class ListOperations : ICommand
{
    private readonly OperationsService _ops;

    public string Name => "list-operations";
    public string Description => "Показать операции";

    public ListOperations(OperationsService ops) => _ops = ops;

    public void Run()
    {
        var all = _ops.List();
        if (all.Count == 0) { Console.WriteLine("(пусто)"); return; }
        foreach (var o in all)
            Console.WriteLine($"{o.Id} | {o.Type} | acc:{o.BankAccountId} | cat:{o.CategoryId} | {o.Amount} | {o.Date} | {o.Description}");
    }
}