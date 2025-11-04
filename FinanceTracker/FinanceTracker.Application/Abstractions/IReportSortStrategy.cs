namespace FinanceTracker.Application.Abstractions;

public interface IReportSortStrategy
{
    string Name { get; } // для подсказки в UI
    IEnumerable<(string CategoryName, decimal Amount)> Sort(
        IEnumerable<(string CategoryName, decimal Amount)> items);
}