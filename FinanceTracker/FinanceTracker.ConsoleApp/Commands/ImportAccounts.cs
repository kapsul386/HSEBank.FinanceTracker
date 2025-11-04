using FinanceTracker.Application.Commands;
using FinanceTracker.Application.Templates;

namespace FinanceTracker.ConsoleApp.Commands;

/// <summary>
/// Импорт счетов из CSV. Формат: name,balance
/// </summary>
public sealed class ImportAccounts : ICommand
{
    private readonly AccountsCsvImporter _importer;

    public string Name => "import-accounts";
    public string Description => "Импорт счетов из CSV (формат: name,balance)";

    public ImportAccounts(AccountsCsvImporter importer)
    {
        _importer = importer;
    }

    public void Run()
    {
        Console.Write("Введите путь к CSV (например: data/accounts.csv): ");
        var path = (Console.ReadLine() ?? "").Trim();

        if (string.IsNullOrWhiteSpace(path))
        {
            Console.WriteLine("Путь не указан.");
            return;
        }

        try
        {
            _importer.Import(path);
            Console.WriteLine("OK: импорт счетов выполнен.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка импорта: {ex.Message}");
        }
    }
}