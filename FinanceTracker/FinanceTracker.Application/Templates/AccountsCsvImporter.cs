using FinanceTracker.Application.Abstractions;
using FinanceTracker.Domain.Entities;
using FinanceTracker.Domain.Factories;

namespace FinanceTracker.Application.Templates;

public sealed class AccountsCsvImporter
    : ImportTemplate<AccountsCsvImporter.Row, BankAccount>
{
    public sealed record Row(
        string Name,
        decimal Balance);

    private readonly IRepository<BankAccount> _accountsRepo;
    private readonly IDomainFactory _factory;

    public AccountsCsvImporter(
        IRepository<BankAccount> accountsRepo,
        IDomainFactory factory)
    {
        _accountsRepo = accountsRepo;
        _factory = factory;
    }

    protected override bool TryParse(string raw, out Row row)
    {
        row = default!;
        // columns: name,balance
        var parts = raw.Split(',', StringSplitOptions.TrimEntries);
        if (parts.Length < 2) return false;

        var name = parts[0];
        if (string.IsNullOrWhiteSpace(name)) return false;

        if (!decimal.TryParse(parts[1], out var balance)) return false;

        row = new Row(name, balance);
        return true;
    }

    protected override bool Validate(Row row)
    {
        // Не допускаем отрицательный баланс и пустые имена.
        if (row.Balance < 0) return false;
        if (string.IsNullOrWhiteSpace(row.Name)) return false;
        return true;
    }

    protected override BankAccount Map(Row row) =>
        _factory.CreateBankAccount(row.Name, row.Balance);

    protected override void Save(BankAccount entity) => _accountsRepo.Add(entity);
}