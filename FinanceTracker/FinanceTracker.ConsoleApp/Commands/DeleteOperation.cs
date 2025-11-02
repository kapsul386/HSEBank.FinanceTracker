using FinanceTracker.Application.Commands;
using FinanceTracker.Application.Services;

namespace FinanceTracker.ConsoleApp.Commands;

/// <summary>
/// Удаляет операцию по Id.
/// Команда: delete-operation
/// </summary>
public sealed class DeleteOperation : ICommand
{
    private readonly OperationsService _operations;

    public string Name => "delete-operation";
    public string Description => "Удалить операцию по Id";

    public DeleteOperation(OperationsService operations)
    {
        _operations = operations;
    }

    public void Run()
    {
        Console.Write("Id операции для удаления: ");
        var idText = Console.ReadLine();

        if (!Guid.TryParse(idText, out var id))
        {
            Console.WriteLine("Ошибка: неверный формат Id.");
            return;
        }

        var op = _operations.Get(id);
        if (op is null)
        {
            Console.WriteLine("Ошибка: операция не найдена.");
            return;
        }

        Console.Write($"Вы уверены, что хотите удалить операцию на сумму {GetAmount(op)} от {GetDate(op):yyyy-MM-dd}? (y/n): ");
        var confirm = Console.ReadLine()?.Trim().ToLowerInvariant();
        if (confirm != "y")
        {
            Console.WriteLine("Отменено пользователем.");
            return;
        }

        try
        {
            _operations.Delete(id);
            Console.WriteLine("OK: операция удалена.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при удалении: {ex.Message}");
        }
    }

    // Подсказки пользователю в подтверждении
    private static decimal GetAmount(object op)
        => op.GetType().GetProperty("Amount")?.GetValue(op) is decimal v ? v : 0m;

    private static DateTime GetDate(object op)
        => op.GetType().GetProperty("Date")?.GetValue(op) is DateTime v ? v : DateTime.MinValue;
}