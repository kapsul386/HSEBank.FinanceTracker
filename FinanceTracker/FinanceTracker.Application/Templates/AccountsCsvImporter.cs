using FinanceTracker.Application.Abstractions;
using FinanceTracker.Domain.Entities;
using FinanceTracker.Domain.Factories;

namespace FinanceTracker.Application.Templates;

/// <summary>
/// CSV importer for <see cref="BankAccount"/> entities.
/// Implements the <b>Template Method</b> pattern by extending <see cref="ImportTemplate{TInput, TEntity}"/>.
/// </summary>
public sealed class AccountsCsvImporter
    : ImportTemplate<AccountsCsvImporter.Row, BankAccount>
{
    /// <summary>
    /// Represents a single parsed CSV row for a bank account.
    /// </summary>
    public sealed record Row(
        string Name,
        decimal Balance);

    private readonly IRepository<BankAccount> _accountsRepo;
    private readonly IDomainFactory _factory;

    /// <summary>
    /// Initializes a new instance of the <see cref="AccountsCsvImporter"/> class.
    /// </summary>
    /// <param name="accountsRepo">Repository for storing imported bank accounts.</param>
    /// <param name="factory">Factory for creating valid domain entities.</param>
    public AccountsCsvImporter(
        IRepository<BankAccount> accountsRepo,
        IDomainFactory factory)
    {
        _accountsRepo = accountsRepo;
        _factory = factory;
    }

    /// <summary>
    /// Parses a raw CSV line into a <see cref="Row"/> record.
    /// </summary>
    /// <param name="raw">Raw CSV line.</param>
    /// <param name="row">Parsed result if successful.</param>
    /// <returns><c>true</c> if the line was parsed successfully; otherwise, <c>false</c>.</returns>
    protected override bool TryParse(string raw, out Row row)
    {
        row = default!;
        // Expected columns: name,balance
        var parts = raw.Split(',', StringSplitOptions.TrimEntries);
        if (parts.Length < 2) return false;

        var name = parts[0];
        if (string.IsNullOrWhiteSpace(name)) return false;

        if (!decimal.TryParse(parts[1], out var balance)) return false;

        row = new Row(name, balance);
        return true;
    }

    /// <summary>
    /// Validates a parsed row to ensure domain consistency.
    /// </summary>
    /// <remarks>Rejects empty names and negative balances.</remarks>
    protected override bool Validate(Row row)
    {
        if (row.Balance < 0) return false;
        if (string.IsNullOrWhiteSpace(row.Name)) return false;
        return true;
    }

    /// <summary>
    /// Maps a validated row to a domain entity using <see cref="DomainFactory"/>.
    /// </summary>
    protected override BankAccount Map(Row row) =>
        _factory.CreateBankAccount(row.Name, row.Balance);

    /// <summary>
    /// Persists the created domain entity into the repository.
    /// </summary>
    protected override void Save(BankAccount entity) => _accountsRepo.Add(entity);
}
