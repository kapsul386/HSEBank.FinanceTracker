using FinanceTracker.Application.Abstractions;

namespace FinanceTracker.Application.Services.ReportSort;

public sealed class NameAscSort : IReportSortStrategy
{
    public string Name => "name-asc";
    public IEnumerable<(string CategoryName, decimal Amount)> Sort(
        IEnumerable<(string CategoryName, decimal Amount)> items)
        => items.OrderBy(x => x.CategoryName);
}