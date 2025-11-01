namespace FinanceTracker.Application.Commands;

public sealed class Exit : ICommand
{
    public string Name => "exit";
    public string Description => "Выйти из приложения";
    public void Run() => Environment.Exit(0);
}