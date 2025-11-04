using FinanceTracker.Application.Commands;

namespace FinanceTracker.ConsoleApp.Commands;

public sealed class Exit : ICommand
{
    public string Name => "exit";
    public string Description => "Exit the application";
    public void Run() => Environment.Exit(0);
}