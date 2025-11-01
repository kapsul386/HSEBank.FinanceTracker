using FinanceTracker.Application.Services;
using FinanceTracker.Domain.Factories;

namespace FinanceTracker.Application.Commands;

public sealed class AddAccount : ICommand
{
    private readonly AccountsService _accounts;
    private readonly IDomainFactory _factory;

    public string Name => "add-account";
    public string Description => "Добавить счёт";

    public AddAccount(AccountsService accounts, IDomainFactory factory)
    {
        _accounts = accounts;
        _factory = factory;
    }

    public void Run()
    {
        Console.Write("Название счёта: ");
        var name = Console.ReadLine() ?? "";
        Console.Write("Начальный баланс: ");
        var text = Console.ReadLine();
        if (!decimal.TryParse(text, out var balance))
        {
            Console.WriteLine("Неверная сумма");
            return;
        }

        var acc = _factory.CreateBankAccount(name, balance);
        _accounts.Add(acc);
        Console.WriteLine($"OK: создан счёт {acc.Name} с балансом {acc.Balance}");
    }
}