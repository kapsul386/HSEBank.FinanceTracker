using System.Diagnostics;

namespace FinanceTracker.Application.Commands;

public sealed class TimingCommandDecorator : ICommand
{
    private readonly ICommand _inner;

    public string Name => _inner.Name;
    public string Description => _inner.Description;

    public TimingCommandDecorator(ICommand inner) => _inner = inner;

    public void Execute()
    {
        var sw = Stopwatch.StartNew();
        _inner.Execute();
        sw.Stop();
        Console.WriteLine($"[timer] {Name} took {sw.ElapsedMilliseconds} ms");
    }
}