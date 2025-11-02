using FinanceTracker.Application.Commands;
using FinanceTracker.Application.Services;
using System.IO;

namespace FinanceTracker.ConsoleApp.Commands;

/// <summary>
/// Экспортирует операции в CSV.
/// Команда: export-operations
/// </summary>
public sealed class ExportOperations : ICommand
{
    private readonly ExportService _export;

    public string Name => "export-operations";
    public string Description => "Экспорт операций в CSV-файл";

    public ExportOperations(ExportService export)
    {
        _export = export;
    }

    public void Run()
    {
        Console.Write("Путь к CSV (например, exports/operations.csv): ");
        var path = (Console.ReadLine() ?? "").Trim();

        if (string.IsNullOrWhiteSpace(path))
        {
            Console.WriteLine("Ошибка: путь не может быть пустым.");
            return;
        }

        try
        {
            var dir = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            _export.ExportOperationsToCsv(path);
            Console.WriteLine($"OK: экспорт выполнен → {Path.GetFullPath(path)}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка экспорта: {ex.Message}");
        }
    }
}