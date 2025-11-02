using FinanceTracker.Application.Abstractions;
using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Application.Services;

public sealed class AnalyticsService
{
    private readonly IRepository<Operation> _ops;

    public AnalyticsService(IRepository<Operation> ops) => _ops = ops;

    public (decimal income, decimal expense, decimal net) Summary(DateOnly from, DateOnly to)
    {
        var all = _ops.GetAll().Where(o => o.Date >= from && o.Date <= to);
        var income  = all.Where(o => o.Type == MoneyFlowType.Income).Sum(o => o.Amount);
        var expense = all.Where(o => o.Type == MoneyFlowType.Expense).Sum(o => o.Amount);
        return (income, expense, income - expense);
    }

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