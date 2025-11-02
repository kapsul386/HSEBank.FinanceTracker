using FinanceTracker.Application.Abstractions;
using FinanceTracker.Application.Services;
using FinanceTracker.Domain.Entities;
using FinanceTracker.Domain.Factories;

namespace FinanceTracker.Application.Templates;

public sealed class OperationsCsvImporter
    : ImportTemplate<OperationsCsvImporter.Row, Operation>
{
    public sealed record Row(
        MoneyFlowType Type,
        Guid AccountId,
        decimal Amount,
        DateOnly Date,
        Guid CategoryId,
        string? Description);

    private readonly IRepository<Operation> _opsRepo;
    private readonly AccountsService _accounts;
    private readonly CategoriesService _categories;
    private readonly IDomainFactory _factory;

    public OperationsCsvImporter(
        IRepository<Operation> opsRepo,
        AccountsService accounts,
        CategoriesService categories,
        IDomainFactory factory)
    {
        _opsRepo = opsRepo;
        _accounts = accounts;
        _categories = categories;
        _factory = factory;
    }

    protected override bool TryParse(string raw, out Row row)
    {
        row = default!;
        var parts = raw.Split(',', StringSplitOptions.TrimEntries);
        if (parts.Length < 5) return false;

        // columns: type,accountId,amount,date,categoryId,description?
        if (!Enum.TryParse<MoneyFlowType>(parts[0], true, out var type)) return false;
        if (!Guid.TryParse(parts[1], out var accountId)) return false;
        if (!decimal.TryParse(parts[2], out var amount)) return false;
        if (!DateOnly.TryParse(parts[3], out var date)) return false;
        if (!Guid.TryParse(parts[4], out var categoryId)) return false;

        var desc = parts.Length >= 6 ? (string.IsNullOrWhiteSpace(parts[5]) ? null : parts[5]) : null;

        row = new Row(type, accountId, amount, date, categoryId, desc);
        return true;
    }

    protected override bool Validate(Row row)
    {
        if (row.Amount <= 0) return false;
        if (_accounts.Get(row.AccountId) is null) return false;
        if (_categories.Get(row.CategoryId) is null) return false;
        return true;
    }

    protected override Operation Map(Row row) =>
        _factory.CreateOperation(
            row.Type, row.AccountId, row.Amount, row.Date, row.CategoryId, row.Description);

    protected override void Save(Operation entity) => _opsRepo.Add(entity);
}
