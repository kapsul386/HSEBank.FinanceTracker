using FinanceTracker.Application.Commands;
using FinanceTracker.Application.Services;
using FinanceTracker.Domain.Entities;
using FinanceTracker.Domain.Factories;

namespace FinanceTracker.ConsoleApp.Commands;

public sealed class AddCategory : ICommand
{
    private readonly CategoriesService _categories;
    private readonly IDomainFactory _factory;

    public string Name => "add-category";
    public string Description => "Добавить категорию (Income/Expense)";

    public AddCategory(CategoriesService categories, IDomainFactory factory)
    {
        _categories = categories;
        _factory = factory;
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

        Console.Write("Название категории: ");
        var name = Console.ReadLine() ?? "";

        var cat = _factory.CreateCategory(type, name);
        _categories.Add(cat);
        Console.WriteLine($"OK: создана категория [{type}] {cat.Name} ({cat.Id})");
    }
}