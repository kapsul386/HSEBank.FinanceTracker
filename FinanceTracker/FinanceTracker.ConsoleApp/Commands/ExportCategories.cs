using FinanceTracker.Application.Commands;
using FinanceTracker.Application.Services;
using System.Text;

namespace FinanceTracker.ConsoleApp.Commands;

/// <summary>
/// Command that exports all categories to a CSV file.
/// Command: <c>export-categories</c>.
/// </summary>
public sealed class ExportCategories : ICommand
{
    private readonly CategoriesService _categories;

    /// <summary>Console command name.</summary>
    public string Name => "export-categories";

    /// <summary>Short description shown in the help list.</summary>
    public string Description => "Export all categories to a CSV file";

    public ExportCategories(CategoriesService categories) => _categories = categories;

    /// <summary>
    /// Executes the command:
    /// <list type="number">
    /// <item>Asks for the destination CSV file path.</item>
    /// <item>Creates directories if necessary.</item>
    /// <item>Writes all categories as rows (Id;Name;Type).</item>
    /// </list>
    /// </summary>
    public void Run()
    {
        Console.Write("CSV file path (e.g., exports/categories.csv): ");
        var path = (Console.ReadLine() ?? "").Trim();

        if (string.IsNullOrWhiteSpace(path))
        {
            Console.WriteLine("Error: path cannot be empty.");
            return;
        }

        var dir = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        using var sw = new StreamWriter(path, false, new UTF8Encoding(true));
        sw.WriteLine("Id;Name;Type");

        foreach (var c in _categories.List())
        {
            var safeName = c.Name.Replace(";", ",");
            sw.WriteLine($"{c.Id};{safeName};{c.Type}");
        }

        Console.WriteLine($"OK: categories exported => {Path.GetFullPath(path)}");
    }
}