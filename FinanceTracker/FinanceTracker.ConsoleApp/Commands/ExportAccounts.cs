using FinanceTracker.Application.Commands;
using FinanceTracker.Application.Services;
using System.Globalization;
using System.IO;
using System.Text;

namespace FinanceTracker.ConsoleApp.Commands;

public sealed class ExportAccounts : ICommand
{
    private readonly AccountsService _accounts;
    public string Name => "export-accounts";
    public string Description => "Экспорт счетов в CSV";

    public ExportAccounts(AccountsService accounts) => _accounts = accounts;

    public void Run()
    {
        Console.Write("Путь к CSV (например, exports/accounts.csv): ");
        var path = (Console.ReadLine() ?? "").Trim();
        if (string.IsNullOrWhiteSpace(path)) { Console.WriteLine("Путь пуст."); return; }

        var dir = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir)) Directory.CreateDirectory(dir);

        using var sw = new StreamWriter(path, false, new UTF8Encoding(true));
        sw.WriteLine("Id;Name;Balance");
        foreach (var a in _accounts.List())
            sw.WriteLine($"{a.Id};{a.Name.Replace(";", ",")};{a.Balance.ToString(CultureInfo.InvariantCulture)}");

        Console.WriteLine($"OK: экспорт счетов → {Path.GetFullPath(path)}");
    }
}