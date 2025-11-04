using FinanceTracker.Application.Abstractions;
using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Application.Services;

/// <summary>
/// Provides analytical calculations based on <see cref="Operation"/> data.
/// Implements domain logic for reporting income, expenses, and category totals.
/// </summary>
public sealed class AnalyticsService
{
    private readonly IRepository<Operation> _ops;

    /// <summary>
    /// Initializes a new instance of the <see cref="AnalyticsService"/> class.
    /// </summary>
    /// <param name="ops">Repository containing all financial operations.</param>
    public AnalyticsService(IRepository<Operation> ops) => _ops = ops;

    /// <summary>
    /// Calculates total income, total expense, and net balance for a given period.
    /// </summary>
    /// <param name="from">Start date (inclusive).</param>
    /// <param name="to">End date (inclusive).</param>
    /// <returns>
    /// A tuple of three values:
    /// <list type="bullet">
    /// <item><description><c>income</c> — sum of all income operations.</description></item>
    /// <item><description><c>expense</c> — sum of all expense operations.</description></item>
    /// <item><description><c>net</c> — income minus expense.</description></item>
    /// </list>
    /// </returns>
    public (decimal income, decimal expense, decimal net) Summary(DateOnly from, DateOnly to)
    {
        var all = _ops.GetAll().Where(o => o.Date >= from && o.Date <= to);
        var operations = all as Operation[] ?? all.ToArray();

        var income  = operations.Where(o => o.Type == MoneyFlowType.Income).Sum(o => o.Amount);
        var expense = operations.Where(o => o.Type == MoneyFlowType.Expense).Sum(o => o.Amount);

        return (income, expense, income - expense);
    }

    /// <summary>
    /// Groups operations by category and returns total amount for each category in a given date range.
    /// </summary>
    /// <param name="from">Start date (inclusive).</param>
    /// <param name="to">End date (inclusive).</param>
    /// <returns>
    /// A list of tuples (<c>categoryId</c>, <c>total</c>), ordered by total descending.
    /// </returns>
    public IReadOnlyList<(Guid categoryId, decimal total)> ByCategory(DateOnly from, DateOnly to)
    {
        var all = _ops.GetAll().Where(o => o.Date >= from && o.Date <= to);

        return all
            .GroupBy(o => o.CategoryId)
            .Select(g => (g.Key, g.Sum(x => x.Amount)))
            .OrderByDescending(x => x.Item2)
            .ToList();
    }
}
