using FinanceTracker.Application.Commands;
using FinanceTracker.Application.Services;
using System.IO;
using System.Text;

namespace FinanceTracker.ConsoleApp.Commands;

public sealed class ExportCategories : ICommand
{
    private readonly CategoriesService _categories;
    public string Name => "export-categories";
    public string Description => "Экспорт категорий в CSV";

    public ExportCategories(CategoriesService categories) => _categories = categories;

    public void Run()
    {
        Console.Write("Путь к CSV (например, exports/categories.csv): ");
        var path = (Console.ReadLine() ?? "").Trim();
        if (string.IsNullOrWhiteSpace(path)) { Console.WriteLine("Путь пуст."); return; }

        var dir = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir)) Directory.CreateDirectory(dir);

        using var sw = new StreamWriter(path, false, new UTF8Encoding(true));
        sw.WriteLine("Id;Name;Type");
        foreach (var c in _categories.List())
            sw.WriteLine($"{c.Id};{c.Name.Replace(";", ",")};{c.Type}");

        Console.WriteLine($"OK: экспорт категорий → {Path.GetFullPath(path)}");
    }
}