using FinanceTracker.Application.Commands;
using FinanceTracker.Application.Templates;

namespace FinanceTracker.ConsoleApp.Commands;

/// <summary>
/// Command that imports operations from a CSV file.
/// Expected format: <c>type,accountId,amount,date,categoryId,description</c>.
/// Command: <c>import-operations</c>.
/// </summary>
public sealed class ImportOperations : ICommand
{
    private readonly OperationsCsvImporter _importer;

    /// <summary>Console command name.</summary>
    public string Name => "import-operations";

    /// <summary>Short description shown in the help list.</summary>
    public string Description => "Import operations from a CSV file (type,accountId,amount,date,categoryId,description)";

    public ImportOperations(OperationsCsvImporter importer) => _importer = importer;

    /// <summary>
    /// Executes the command:
    /// <list type="number">
    /// <item>Asks for the CSV file path.</item>
    /// <item>Imports operations using <see cref="OperationsCsvImporter"/>.</item>
    /// <item>Displays the number of imported records or an error message.</item>
    /// </list>
    /// </summary>
    public void Run()
    {
        Console.Write("CSV file path: ");
        var path = Console.ReadLine() ?? "";

        if (string.IsNullOrWhiteSpace(path))
        {
            Console.WriteLine("Error: file path not specified.");
            return;
        }

        try
        {
            var count = _importer.Import(path);
            Console.WriteLine($"OK: {count} operations imported.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Import error: {ex.Message}");
        }
    }
}