using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Domain.Factories;

public interface IDomainFactory
{
    BankAccount CreateBankAccount(string name, decimal initialBalance = 0);
    Category    CreateCategory(MoneyFlowType type, string name);
    Operation   CreateOperation(
        MoneyFlowType type,
        Guid accountId,
        decimal amount,
        DateOnly date,
        Guid categoryId,
        string? description = null);
}