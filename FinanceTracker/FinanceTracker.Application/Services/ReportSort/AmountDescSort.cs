using FinanceTracker.Application.Abstractions;

namespace FinanceTracker.Application.Services.ReportSort;

public sealed class AmountDescSort : IReportSortStrategy
{
    public string Name => "amount-desc";
    public IEnumerable<(string CategoryName, decimal Amount)> Sort(
        IEnumerable<(string CategoryName, decimal Amount)> items)
        => items.OrderByDescending(x => x.Amount).ThenBy(x => x.CategoryName);
}