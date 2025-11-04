using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Application.Export;

public interface IExportVisitor
{
    void Visit(BankAccount a);
    void Visit(Category c);
    void Visit(Operation o);
}