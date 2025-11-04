using FinanceTracker.Application.Commands;
using FinanceTracker.Application.Templates;

namespace FinanceTracker.ConsoleApp.Commands;

/// <summary>
/// Command that imports accounts from a CSV file.
/// Expected format: <c>name,balance</c>.
/// Command: <c>import-accounts</c>.
/// </summary>
public sealed class ImportAccounts : ICommand
{
    private readonly AccountsCsvImporter _importer;

    /// <summary>Console command name.</summary>
    public string Name => "import-accounts";

    /// <summary>Short description shown in the help list.</summary>
    public string Description => "Import accounts from a CSV file (format: name,balance)";

    public ImportAccounts(AccountsCsvImporter importer)
    {
        _importer = importer;
    }

    /// <summary>
    /// Executes the command:
    /// <list type="number">
    /// <item>Asks for the CSV file path.</item>
    /// <item>Validates input and runs the import process.</item>
    /// <item>Reports success or displays an error message.</item>
    /// </list>
    /// </summary>
    public void Run()
    {
        Console.Write("Enter CSV file path (e.g., data/accounts.csv): ");
        var path = (Console.ReadLine() ?? "").Trim();

        if (string.IsNullOrWhiteSpace(path))
        {
            Console.WriteLine("Error: file path not specified.");
            return;
        }

        try
        {
            _importer.Import(path);
            Console.WriteLine("OK: accounts imported successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Import error: {ex.Message}");
        }
    }
}