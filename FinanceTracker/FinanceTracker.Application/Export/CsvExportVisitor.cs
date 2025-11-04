using System.Text;
using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Application.Export;

/// <summary>
/// CSV export visitor that writes domain entities to a file.
/// Implements the Visitor pattern for BankAccount, Category, and Operation.
/// </summary>
public sealed class CsvExportVisitor : IExportVisitor, IDisposable
{
    private readonly StreamWriter _writer;
    private bool _disposed;

    /// <summary>
    /// Creates a new CSV writer with optional header line.
    /// UTF-8 encoding without BOM for better cross-platform compatibility.
    /// </summary>
    public CsvExportVisitor(string path, string header)
    {
        _writer = new StreamWriter(path, false, new UTF8Encoding(false));
        if (!string.IsNullOrWhiteSpace(header))
            _writer.WriteLine(header);
    }

    /// <summary>
    /// Writes a BankAccount entity to the CSV file.
    /// </summary>
    public void Visit(BankAccount a)
        => _writer.WriteLine($"{a.Id},{Escape(a.Name)},{a.Balance}");

    /// <summary>
    /// Writes a Category entity to the CSV file.
    /// </summary>
    public void Visit(Category c)
        => _writer.WriteLine($"{c.Id},{c.Type},{Escape(c.Name)}");

    /// <summary>
    /// Writes an Operation entity to the CSV file.
    /// </summary>
    public void Visit(Operation o)
        => _writer.WriteLine($"{o.Id},{o.Type},{o.BankAccountId},{o.Amount},{o.Date},{o.CategoryId},{Escape(o.Description)}");

    /// <summary>
    /// Escapes commas and quotes according to CSV rules.
    /// Wraps the value in quotes if necessary and doubles inner quotes.
    /// </summary>
    private static string Escape(string? s)
    {
        if (string.IsNullOrEmpty(s)) return "";
        var needsQuotes = s.Contains(',') || s.Contains('"');
        var t = s.Replace("\"", "\"\"");
        return needsQuotes ? $"\"{t}\"" : t;
    }

    /// <summary>
    /// Ensures that the file stream is properly closed and flushed.
    /// </summary>
    public void Dispose()
    {
        if (_disposed) return;
        _writer.Flush();
        _writer.Dispose();
        _disposed = true;
    }
}
