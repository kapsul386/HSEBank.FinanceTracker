using FinanceTracker.Application.Abstractions;
using FinanceTracker.Application.Services;
using FinanceTracker.Domain.Entities;
using FinanceTracker.Domain.Factories;

namespace FinanceTracker.Application.Templates;

/// <summary>
/// CSV importer for <see cref="Operation"/> entities.
/// Implements the <b>Template Method</b> pattern by extending
/// <see cref="ImportTemplate{TRow, TEntity}"/>.
/// </summary>
public sealed class OperationsCsvImporter
    : ImportTemplate<OperationsCsvImporter.Row, Operation>
{
    /// <summary>
    /// Strongly-typed representation of a parsed CSV row.
    /// Expected columns (comma-separated):
    /// <c>type,accountId,amount,date,categoryId,description?</c>
    /// </summary>
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

    /// <summary>
    /// Initializes a new instance of the <see cref="OperationsCsvImporter"/> class.
    /// </summary>
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

    /// <summary>
    /// Parses a raw CSV line into a <see cref="Row"/>.
    /// Expected columns: <c>type,accountId,amount,date,categoryId,description?</c>.
    /// </summary>
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

        var desc = parts.Length >= 6
            ? (string.IsNullOrWhiteSpace(parts[5]) ? null : parts[5])
            : null;

        row = new Row(type, accountId, amount, date, categoryId, desc);
        return true;
        }

    /// <summary>
    /// Validates domain constraints for a parsed row:
    /// - Amount must be positive
    /// - Account and Category must exist
    /// </summary>
    protected override bool Validate(Row row)
    {
        if (row.Amount <= 0) return false;
        if (_accounts.Get(row.AccountId) is null) return false;
        if (_categories.Get(row.CategoryId) is null) return false;
        return true;
    }

    /// <summary>
    /// Maps a valid row into a domain <see cref="Operation"/> using the factory.
    /// </summary>
    protected override Operation Map(Row row) =>
        _factory.CreateOperation(
            row.Type, row.AccountId, row.Amount, row.Date, row.CategoryId, row.Description);

    /// <summary>
    /// Persists the created entity into the repository.
    /// </summary>
    protected override void Save(Operation entity) => _opsRepo.Add(entity);
}
