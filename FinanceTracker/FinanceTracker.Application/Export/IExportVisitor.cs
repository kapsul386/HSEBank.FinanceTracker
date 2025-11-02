using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Application.Export;

public interface IExportVisitor
{
    // для одиночных сущностей
    void Visit(BankAccount a);
    void Visit(Category c);
    void Visit(Operation o);
}