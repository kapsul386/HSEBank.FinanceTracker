namespace FinanceTracker.Application.Abstractions;

public interface IReportSortStrategy
{
    string Name { get; } 
    IEnumerable<(string CategoryName, decimal Amount)> Sort(
        IEnumerable<(string CategoryName, decimal Amount)> items);
}