using FinanceTracker.Application.Services;

namespace FinanceTracker.Application.Commands;

public sealed class ListAccounts : ICommand
{
    private readonly AccountsService _accounts;

    public string Name => "list-accounts";
    public string Description => "Показать все счета";

    public ListAccounts(AccountsService accounts) => _accounts = accounts;

    public void Run()
    {
        var all = _accounts.List();
        if (all.Count == 0) { Console.WriteLine("(пусто)"); return; }
        foreach (var a in all)
            Console.WriteLine($"{a.Id} | {a.Name} | {a.Balance}");
    }
}