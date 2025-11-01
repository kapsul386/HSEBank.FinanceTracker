namespace FinanceTracker.Application.Commands;

public interface ICommand
{
    string Name { get; }
    string Description { get; }
    void Execute();
}