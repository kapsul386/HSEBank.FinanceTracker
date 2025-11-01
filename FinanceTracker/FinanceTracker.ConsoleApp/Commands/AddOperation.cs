using FinanceTracker.Application.Commands;
using FinanceTracker.Application.Services;
using FinanceTracker.Domain.Entities;
using FinanceTracker.Domain.Factories;

namespace FinanceTracker.ConsoleApp.Commands;

public sealed class AddOperation : ICommand
{
    private readonly OperationsService _ops;
    private readonly AccountsService _accounts;
    private readonly CategoriesService _categories;
    private readonly IDomainFactory _factory;

    public string Name => "add-operation";
    public string Description => "Добавить операцию (Income/Expense)";

    public AddOperation(
        OperationsService ops,
        AccountsService accounts,
        CategoriesService categories,
        IDomainFactory factory)
    {
        _ops = ops; _accounts = accounts; _categories = categories; _factory = factory;
    }

    public void Run()
    {
        Console.Write("Тип (Income/Expense): ");
        var typeStr = (Console.ReadLine() ?? "").Trim();
        if (!Enum.TryParse<MoneyFlowType>(typeStr, true, out var type))
        {
            Console.WriteLine("Неверный тип");
            return;
        }

        Console.Write("ID счёта: ");      var accIdText = Console.ReadLine();
        Console.Write("ID категории: ");  var catIdText = Console.ReadLine();
        Console.Write("Сумма: ");         var amountText = Console.ReadLine();
        Console.Write("Дата (YYYY-MM-DD): "); var dateText = Console.ReadLine();
        Console.Write("Описание (optional): "); var desc = Console.ReadLine();

        if (!Guid.TryParse(accIdText, out var accId) ||
            !Guid.TryParse(catIdText, out var catId) ||
            !decimal.TryParse(amountText, out var amount) ||
            !DateOnly.TryParse(dateText, out var date))
        {
            Console.WriteLine("Неверный ввод");
            return;
        }

        // примитивная валидация наличия счёта/категории
        if (_accounts.Get(accId) is null) { Console.WriteLine("Нет такого счёта"); return; }
        if (_categories.Get(catId) is null){ Console.WriteLine("Нет такой категории"); return; }

        var op = _factory.CreateOperation(type, accId, amount, date, catId, desc);
        _ops.Add(op);
        Console.WriteLine($"OK: добавлена операция [{type}] {amount} от {date}");
    }
}
