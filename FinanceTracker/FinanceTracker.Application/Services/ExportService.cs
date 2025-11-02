using FinanceTracker.Application.Abstractions;
using FinanceTracker.Application.Export;
using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Application.Services;

public sealed class ExportService
{
    private readonly IRepository<Operation> _ops;

    public ExportService(IRepository<Operation> ops) => _ops = ops;

    public void ExportOperationsToCsv(string path)
    {
        // заголовок под наш формат Visit(Operation)
        using var v = new CsvExportVisitor(
            path,
            "id,type,bankAccountId,amount,date,categoryId,description");

        foreach (var op in _ops.GetAll())
            v.Visit(op);
    }

    // при желании можно добавить:
    // public void ExportAccountsToCsv(string path) { ... }
    // public void ExportCategoriesToCsv(string path) { ... }
}