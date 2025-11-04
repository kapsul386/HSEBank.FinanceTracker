using FinanceTracker.Application.Abstractions;

namespace FinanceTracker.Application.Services.ReportSort;

/// <summary>
/// Strategy for sorting analytical report results
/// by amount in descending order (largest first),
/// then alphabetically by category name.
/// </summary>
/// <remarks>
/// Implements the <b>Strategy</b> pattern — one of several interchangeable
/// sorting algorithms for category-based analytical reports.
/// </remarks>
public sealed class AmountDescSort : IReportSortStrategy
{
    /// <summary>
    /// Unique name of this sorting strategy.
    /// Used for selecting the strategy from the console.
    /// </summary>
    public string Name => "amount-desc";

    /// <summary>
    /// Sorts the given report items by <c>Amount</c> descending,
    /// and then by <c>CategoryName</c> ascending for ties.
    /// </summary>
    /// <param name="items">Collection of category–amount pairs.</param>
    /// <returns>Sorted sequence of report items.</returns>
    public IEnumerable<(string CategoryName, decimal Amount)> Sort(
        IEnumerable<(string CategoryName, decimal Amount)> items)
        => items
            .OrderByDescending(x => x.Amount)
            .ThenBy(x => x.CategoryName);
}