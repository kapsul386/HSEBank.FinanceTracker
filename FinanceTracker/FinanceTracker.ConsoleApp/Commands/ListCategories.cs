using FinanceTracker.Application.Commands;
using FinanceTracker.Application.Services;

namespace FinanceTracker.ConsoleApp.Commands;

/// <summary>
/// Command that displays all categories.
/// Command: <c>list-categories</c>.
/// </summary>
public sealed class ListCategories : ICommand
{
    private readonly CategoriesService _categories;

    /// <summary>Console command name.</summary>
    public string Name => "list-categories";

    /// <summary>Short description shown in the help list.</summary>
    public string Description => "Show all categories";

    public ListCategories(CategoriesService categories) => _categories = categories;

    /// <summary>
    /// Executes the command:
    /// <list type="number">
    /// <item>Retrieves all categories from the service.</item>
    /// <item>Prints their Id, type, and name to the console.</item>
    /// <item>If no categories exist, displays <c>(empty)</c>.</item>
    /// </list>
    /// </summary>
    public void Run()
    {
        var all = _categories.List();
        if (all.Count == 0)
        {
            Console.WriteLine("(empty)");
            return;
        }

        foreach (var c in all)
            Console.WriteLine($"{c.Id} | {c.Type} | {c.Name}");
    }
}