using FinanceTracker.Application.Commands;
using FinanceTracker.Application.Services;

namespace FinanceTracker.ConsoleApp.Commands;

/// <summary>
/// Command that displays a financial summary (income, expenses, net total)
/// for a specified date range.
/// Command: <c>report-summary</c>.
/// </summary>
public sealed class ReportSummary : ICommand
{
    private readonly AnalyticsService _analytics;

    /// <summary>Console command name.</summary>
    public string Name => "report-summary";

    /// <summary>Short description shown in the help list.</summary>
    public string Description => "Summary: income/expenses/net total for a period";

    public ReportSummary(AnalyticsService analytics) => _analytics = analytics;

    /// <summary>
    /// Executes the command:
    /// <list type="number">
    /// <item>Asks the user for a date range (<c>from</c> and <c>to</c>).</item>
    /// <item>Retrieves income, expenses, and net total from <see cref="AnalyticsService"/>.</item>
    /// <item>Prints the results to the console.</item>
    /// </list>
    /// </summary>
    public void Run()
    {
        Console.Write("From date (YYYY-MM-DD): ");
        var fromText = Console.ReadLine();

        Console.Write("To date (YYYY-MM-DD): ");
        var toText = Console.ReadLine();

        if (!DateOnly.TryParse(fromText, out var from) || !DateOnly.TryParse(toText, out var to))
        {
            Console.WriteLine("Error: invalid date format.");
            return;
        }

        var (income, expense, net) = _analytics.Summary(from, to);

        Console.WriteLine($"Income:  {income}");
        Console.WriteLine($"Expense: {expense}");
        Console.WriteLine($"Net:     {net}");
    }
}