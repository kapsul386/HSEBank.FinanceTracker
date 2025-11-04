using FinanceTracker.Application.Abstractions;
using FinanceTracker.Application.Export;
using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Application.Services;

/// <summary>
/// Service responsible for exporting domain data to external CSV files.
/// Implements the Visitor pattern to separate export logic from entities.
/// </summary>
public sealed class ExportService
{
    private readonly IRepository<Operation> _ops;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExportService"/> class.
    /// </summary>
    /// <param name="ops">Repository containing <see cref="Operation"/> entities.</param>
    public ExportService(IRepository<Operation> ops) => _ops = ops;

    /// <summary>
    /// Exports all <see cref="Operation"/> records from the repository to a CSV file.
    /// </summary>
    /// <param name="path">Absolute or relative path to the target CSV file.</param>
    /// <remarks>
    /// The CSV is encoded in UTF-8 without BOM.  
    /// Columns: <c>id,type,bankAccountId,amount,date,categoryId,description</c>.
    /// </remarks>
    public void ExportOperationsToCsv(string path)
    {
        // Header line matches the format defined in CsvExportVisitor.Visit(Operation)
        using var v = new CsvExportVisitor(
            path,
            "id,type,bankAccountId,amount,date,categoryId,description");

        foreach (var op in _ops.GetAll())
            v.Visit(op);
    }
}