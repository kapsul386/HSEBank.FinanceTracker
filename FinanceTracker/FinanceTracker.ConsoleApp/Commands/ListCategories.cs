using FinanceTracker.Application.Commands;
using FinanceTracker.Application.Services;

namespace FinanceTracker.ConsoleApp.Commands;

public sealed class ListCategories : ICommand
{
    private readonly CategoriesService _categories;

    public string Name => "list-categories";
    public string Description => "Показать категории";

    public ListCategories(CategoriesService categories) => _categories = categories;

    public void Run()
    {
        var all = _categories.List();
        if (all.Count == 0) { Console.WriteLine("(пусто)"); return; }
        foreach (var c in all)
            Console.WriteLine($"{c.Id} | {c.Type} | {c.Name}");
    }
}