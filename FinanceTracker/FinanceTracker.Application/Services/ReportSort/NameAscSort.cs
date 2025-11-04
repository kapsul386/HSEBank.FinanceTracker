using FinanceTracker.Application.Abstractions;

namespace FinanceTracker.Application.Services.ReportSort;

/// <summary>
/// Strategy for sorting analytical report results
/// alphabetically by category name (A → Z).
/// </summary>
/// <remarks>
/// Implements the <b>Strategy</b> pattern — one of several interchangeable
/// sorting algorithms for analytical reports.
/// </remarks>
public sealed class NameAscSort : IReportSortStrategy
{
    /// <summary>
    /// Unique name of this sorting strategy.
    /// Used for selecting the strategy from the console.
    /// </summary>
    public string Name => "name-asc";

    /// <summary>
    /// Sorts the given report items alphabetically by <c>CategoryName</c>.
    /// </summary>
    /// <param name="items">Collection of category–amount pairs.</param>
    /// <returns>Sorted sequence of report items.</returns>
    public IEnumerable<(string CategoryName, decimal Amount)> Sort(
        IEnumerable<(string CategoryName, decimal Amount)> items)
        => items.OrderBy(x => x.CategoryName);
}