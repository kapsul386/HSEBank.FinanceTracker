using FinanceTracker.Application.Commands;
using FinanceTracker.Application.Templates;

namespace FinanceTracker.ConsoleApp.Commands;

public sealed class ImportOperationsCommand : ICommand
{
    private readonly OperationsCsvImporter _importer;

    public string Name => "import-operations";
    public string Description => "Импорт операций из CSV (type,accountId,amount,date,categoryId,description)";

    public ImportOperationsCommand(OperationsCsvImporter importer) => _importer = importer;

    public void Run()
    {
        Console.Write("Путь к CSV: ");
        var path = Console.ReadLine() ?? "";
        try
        {
            var count = _importer.Import(path);
            Console.WriteLine($"OK: импортировано {count} операций");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка импорта: {ex.Message}");
        }
    }
}