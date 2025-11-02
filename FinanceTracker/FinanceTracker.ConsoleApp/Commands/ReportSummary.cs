using FinanceTracker.Application.Commands;
using FinanceTracker.Application.Services;

namespace FinanceTracker.ConsoleApp.Commands;

public sealed class ReportSummary : ICommand
{
    private readonly AnalyticsService _analytics;

    public string Name => "report-summary";
    public string Description => "Сводка: доход/расход/итог за период";

    public ReportSummary(AnalyticsService analytics) => _analytics = analytics;

    public void Run()
    {
        Console.Write("Период от (YYYY-MM-DD): ");
        var fromText = Console.ReadLine();
        Console.Write("Период до (YYYY-MM-DD): ");
        var toText = Console.ReadLine();
        if (!DateOnly.TryParse(fromText, out var from) || !DateOnly.TryParse(toText, out var to))
        {
            Console.WriteLine("Неверные даты");
            return;
        }

        var (income, expense, net) = _analytics.Summary(from, to);
        Console.WriteLine($"Доход:  {income}");
        Console.WriteLine($"Расход: {expense}");
        Console.WriteLine($"Итог:   {net}");
    }
}