using FinanceTracker.Application.Commands;
using FinanceTracker.Application.Services;
using FinanceTracker.Application.Abstractions;

namespace FinanceTracker.ConsoleApp.Commands;

public sealed class ReportByCategory : ICommand
{
    private readonly AnalyticsService _analytics;
    private readonly CategoriesService _categories;
    private readonly IEnumerable<IReportSortStrategy> _strategies;

    public string Name => "report-by-category";
    public string Description => "Суммы по категориям за период (со стратегией сортировки)";

    public ReportByCategory(AnalyticsService analytics,
                            CategoriesService categories,
                            IEnumerable<IReportSortStrategy> strategies)
    {
        _analytics = analytics;
        _categories = categories;
        _strategies = strategies;
    }

    public void Run()
    {
        Console.Write("Дата с (yyyy-MM-dd): ");
        if (!DateOnly.TryParse(Console.ReadLine(), out var from)) { Console.WriteLine("Неверная дата"); return; }
        Console.Write("Дата по (yyyy-MM-dd): ");
        if (!DateOnly.TryParse(Console.ReadLine(), out var to)) { Console.WriteLine("Неверная дата"); return; }

        Console.WriteLine("Стратегии сортировки:");
        foreach (var s in _strategies) Console.WriteLine($" - {s.Name}");
        Console.Write("Выберите стратегию: ");
        var pick = (Console.ReadLine() ?? "").Trim();

        var strategy = _strategies.FirstOrDefault(s =>
                           s.Name.Equals(pick, StringComparison.OrdinalIgnoreCase))
                       ?? _strategies.First();

        // берём (categoryId, total) и превращаем в (CategoryName, Amount)
        var raw = _analytics.ByCategory(from, to);
        var items = raw.Select(x =>
        {
            var c = _categories.Get(x.categoryId);
            var name = c?.Name ?? x.categoryId.ToString();
            return (CategoryName: name, Amount: x.total);
        });

        var sorted = strategy.Sort(items);

        Console.WriteLine("\nКатегория;Сумма");
        foreach (var (cat, sum) in sorted)
            Console.WriteLine($"{cat};{sum}");
    }
}
