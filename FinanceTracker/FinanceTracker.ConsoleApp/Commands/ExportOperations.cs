using FinanceTracker.Application.Commands;
using FinanceTracker.Application.Services;

namespace FinanceTracker.ConsoleApp.Commands;

/// <summary>
/// Command that exports all operations to a CSV file.
/// Command: <c>export-operations</c>.
/// </summary>
public sealed class ExportOperations : ICommand
{
    private readonly ExportService _export;

    /// <summary>Console command name.</summary>
    public string Name => "export-operations";

    /// <summary>Short description shown in the help list.</summary>
    public string Description => "Export all operations to a CSV file";

    public ExportOperations(ExportService export)
    {
        _export = export;
    }

    /// <summary>
    /// Executes the command:
    /// <list type="number">
    /// <item>Asks for the destination CSV file path.</item>
    /// <item>Creates directories if necessary.</item>
    /// <item>Exports all operations to the specified file.</item>
    /// </list>
    /// </summary>
    public void Run()
    {
        Console.Write("CSV file path (e.g., exports/operations.csv): ");
        var path = (Console.ReadLine() ?? "").Trim();

        if (string.IsNullOrWhiteSpace(path))
        {
            Console.WriteLine("Error: path cannot be empty.");
            return;
        }

        try
        {
            var dir = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            _export.ExportOperationsToCsv(path);
            Console.WriteLine($"OK: operations exported => {Path.GetFullPath(path)}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Export error: {ex.Message}");
        }
    }
}