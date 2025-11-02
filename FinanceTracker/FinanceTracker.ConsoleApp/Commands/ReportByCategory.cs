using FinanceTracker.Application.Commands;
using FinanceTracker.Application.Services;

namespace FinanceTracker.ConsoleApp.Commands;

public sealed class ReportByCategoryCommand : ICommand
{
    private readonly AnalyticsService _analytics;

    public string Name => "report-by-category";
    public string Description => "Суммы по категориям за период";

    public ReportByCategoryCommand(AnalyticsService analytics) => _analytics = analytics;

    public void Run()
    {
        Console.Write("Период от (YYYY-MM-DD): ");
        var fromText = Console.ReadLine();
        Console.Write("Период до (YYYY-MM-DD): ");
        var toText = Console.ReadLine();

        if (!DateOnly.TryParse(fromText, out var from) ||
            !DateOnly.TryParse(toText, out var to))
        {
            Console.WriteLine("Неверные даты");
            return;
        }

        var rows = _analytics.ByCategory(from, to);
        if (rows.Count == 0)
        {
            Console.WriteLine("(пусто)");
            return;
        }

        foreach (var (categoryId, total) in rows)
            Console.WriteLine($"{categoryId} | {total}");
    }
}