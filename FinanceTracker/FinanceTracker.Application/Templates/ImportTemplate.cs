namespace FinanceTracker.Application.Templates;

public abstract class ImportTemplate<TRow, TEntity>
{
    public int Import(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("Path is required", nameof(path));
        if (!File.Exists(path))
            throw new FileNotFoundException("File not found", path);

        var lines = File.ReadAllLines(path);
        var count = 0;

        foreach (var raw in ReadData(lines))
        {
            if (!TryParse(raw, out var row)) continue;
            if (!Validate(row)) continue;

            var entity = Map(row);
            Save(entity);
            count++;
        }

        return count;
    }

    protected virtual IEnumerable<string> ReadData(string[] lines)
        => lines.Skip(1); // пропустим заголовок

    protected abstract bool TryParse(string raw, out TRow row);
    protected abstract bool Validate(TRow row);
    protected abstract TEntity Map(TRow row);
    protected abstract void Save(TEntity entity);
}