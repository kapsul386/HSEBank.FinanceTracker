using System.Text;
using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Application.Export;

public sealed class CsvExportVisitor : IExportVisitor, IDisposable
{
    private readonly StreamWriter _writer;
    private bool _disposed;

    public CsvExportVisitor(string path, string header)
    {
        // UTF-8 без BOM
        _writer = new StreamWriter(path, false, new UTF8Encoding(false));
        if (!string.IsNullOrWhiteSpace(header))
            _writer.WriteLine(header);
    }

    public void Visit(BankAccount a)
        => _writer.WriteLine($"{a.Id},{Escape(a.Name)},{a.Balance}");

    public void Visit(Category c)
        => _writer.WriteLine($"{c.Id},{c.Type},{Escape(c.Name)}");

    public void Visit(Operation o)
        => _writer.WriteLine($"{o.Id},{o.Type},{o.BankAccountId},{o.Amount},{o.Date},{o.CategoryId},{Escape(o.Description)}");

    private static string Escape(string? s)
    {
        if (string.IsNullOrEmpty(s)) return "";
        // простое экранирование — если есть запятая/кавычки, оборачиваем в кавычки и удваиваем кавычки
        var needsQuotes = s.Contains(',') || s.Contains('"');
        var t = s.Replace("\"", "\"\"");
        return needsQuotes ? $"\"{t}\"" : t;
    }

    public void Dispose()
    {
        if (_disposed) return;
        _writer.Flush();
        _writer.Dispose();
        _disposed = true;
    }
}