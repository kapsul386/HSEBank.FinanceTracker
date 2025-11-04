using FinanceTracker.Application.Commands;
using FinanceTracker.Application.Services;
using FinanceTracker.Application.Abstractions;

namespace FinanceTracker.ConsoleApp.Commands;

/// <summary>
/// Command that generates a category-based spending/income report
/// for a selected date range using a chosen sorting strategy.
/// Command: <c>report-by-category</c>.
/// </summary>
public sealed class ReportByCategory : ICommand
{
    private readonly AnalyticsService _analytics;
    private readonly CategoriesService _categories;
    private readonly IEnumerable<IReportSortStrategy> _strategies;

    /// <summary>Console command name.</summary>
    public string Name => "report-by-category";

    /// <summary>Short description shown in the help list.</summary>
    public string Description => "Show total amounts by category for a period (with sorting strategy)";

    public ReportByCategory(
        AnalyticsService analytics,
        CategoriesService categories,
        IEnumerable<IReportSortStrategy> strategies)
    {
        _analytics = analytics;
        _categories = categories;
        _strategies = strategies;
    }

    /// <summary>
    /// Executes the command:
    /// <list type="number">
    /// <item>Asks for a date range (<c>from</c> – <c>to</c>).</item>
    /// <item>Displays available sorting strategies and allows the user to choose one.</item>
    /// <item>Builds a report grouped by category and applies the selected sort.</item>
    /// <item>Prints the resulting table (<c>Category;Total</c>).</item>
    /// </list>
    /// </summary>
    public void Run()
    {
        Console.Write("From date (yyyy-MM-dd): ");
        if (!DateOnly.TryParse(Console.ReadLine(), out var from))
        {
            Console.WriteLine("Error: invalid date format.");
            return;
        }

        Console.Write("To date (yyyy-MM-dd): ");
        if (!DateOnly.TryParse(Console.ReadLine(), out var to))
        {
            Console.WriteLine("Error: invalid date format.");
            return;
        }

        Console.WriteLine("Available sorting strategies:");
        foreach (var s in _strategies)
            Console.WriteLine($" - {s.Name}");

        Console.Write("Choose a strategy: ");
        var pick = (Console.ReadLine() ?? "").Trim();

        var strategy = _strategies.FirstOrDefault(s =>
                           s.Name.Equals(pick, StringComparison.OrdinalIgnoreCase))
                       ?? _strategies.First();

        // Retrieve (categoryId, total) → convert to (CategoryName, Amount)
        var raw = _analytics.ByCategory(from, to);
        var items = raw.Select(x =>
        {
            var c = _categories.Get(x.categoryId);
            var name = c?.Name ?? x.categoryId.ToString();
            return (CategoryName: name, Amount: x.total);
        });

        var sorted = strategy.Sort(items);

        Console.WriteLine("\nCategory;Total");
        foreach (var (cat, sum) in sorted)
            Console.WriteLine($"{cat};{sum}");
    }
}
