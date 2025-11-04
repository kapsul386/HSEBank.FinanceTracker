namespace FinanceTracker.Application.Templates;

/// <summary>
/// Abstract base class for importing data from text (e.g., CSV) files.
/// Implements the <b>Template Method</b> pattern — defining a common workflow
/// for importing data, while delegating parsing, validation, mapping, and saving
/// logic to subclasses.
/// </summary>
/// <typeparam name="TRow">Type representing a parsed data row.</typeparam>
/// <typeparam name="TEntity">Type of the target domain entity.</typeparam>
public abstract class ImportTemplate<TRow, TEntity>
{
    /// <summary>
    /// Imports data from a specified file path.
    /// </summary>
    /// <param name="path">Absolute or relative path to the input file.</param>
    /// <returns>The number of successfully imported entities.</returns>
    /// <exception cref="ArgumentException">Thrown if the path is null or empty.</exception>
    /// <exception cref="FileNotFoundException">Thrown if the file does not exist.</exception>
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

    /// <summary>
    /// Reads raw data lines from the input file.
    /// By default, skips the first line (header row).
    /// </summary>
    /// <param name="lines">All lines from the input file.</param>
    /// <returns>Enumerable of data lines for processing.</returns>
    protected virtual IEnumerable<string> ReadData(string[] lines)
        => lines.Skip(1);

    /// <summary>
    /// Parses a raw line of text into a strongly typed row representation.
    /// </summary>
    /// <param name="raw">Raw text line.</param>
    /// <param name="row">Parsed row if successful.</param>
    /// <returns><c>true</c> if parsing succeeded; otherwise, <c>false</c>.</returns>
    protected abstract bool TryParse(string raw, out TRow row);

    /// <summary>
    /// Validates the parsed row according to domain rules.
    /// </summary>
    /// <param name="row">Parsed row to validate.</param>
    /// <returns><c>true</c> if the row is valid; otherwise, <c>false</c>.</returns>
    protected abstract bool Validate(TRow row);

    /// <summary>
    /// Converts a valid parsed row into a domain entity.
    /// </summary>
    /// <param name="row">Validated parsed row.</param>
    /// <returns>Constructed domain entity.</returns>
    protected abstract TEntity Map(TRow row);

    /// <summary>
    /// Persists the created domain entity into the repository.
    /// </summary>
    /// <param name="entity">Entity to save.</param>
    protected abstract void Save(TEntity entity);
}
