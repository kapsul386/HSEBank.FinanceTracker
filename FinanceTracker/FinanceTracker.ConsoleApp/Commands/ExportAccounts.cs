using FinanceTracker.Application.Commands;
using FinanceTracker.Application.Services;
using System.Globalization;
using System.Text;

namespace FinanceTracker.ConsoleApp.Commands;

/// <summary>
/// Command that exports all accounts to a CSV file.
/// Command: <c>export-accounts</c>.
/// </summary>
public sealed class ExportAccounts(AccountsService accounts) : ICommand
{
    /// <summary>Console command name.</summary>
    public string Name => "export-accounts";

    /// <summary>Short description shown in the help list.</summary>
    public string Description => "Export all accounts to a CSV file";

    /// <summary>
    /// Executes the command:
    /// <list type="number">
    /// <item>Asks for the destination CSV file path.</item>
    /// <item>Creates directories if necessary.</item>
    /// <item>Writes all accounts as rows (Id;Name;Balance).</item>
    /// </list>
    /// </summary>
    public void Run()
    {
        Console.Write("CSV file path (e.g., exports/accounts.csv): ");
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
        sw.WriteLine("Id;Name;Balance");

        foreach (var a in accounts.List())
        {
            var safeName = a.Name.Replace(";", ",");
            var balanceText = a.Balance.ToString(CultureInfo.InvariantCulture);
            sw.WriteLine($"{a.Id};{safeName};{balanceText}");
        }

        Console.WriteLine($"OK: accounts exported => {Path.GetFullPath(path)}");
    }
}