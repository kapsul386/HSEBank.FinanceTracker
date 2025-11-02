using FinanceTracker.Application.Commands;
using FinanceTracker.Application.Services;

namespace FinanceTracker.ConsoleApp.Commands;

/// <summary>
/// Удаляет счёт по Id.
/// Команда: delete-account
/// </summary>
public sealed class DeleteAccount : ICommand
{
    private readonly AccountsService _accounts;

    public string Name => "delete-account";
    public string Description => "Удалить счёт по Id";

    public DeleteAccount(AccountsService accounts)
    {
        _accounts = accounts;
    }

    public void Run()
    {
        Console.Write("Id счёта для удаления: ");
        var idText = Console.ReadLine();

        if (!Guid.TryParse(idText, out var id))
        {
            Console.WriteLine("Ошибка: неверный формат Id.");
            return;
        }

        var acc = _accounts.Get(id);
        if (acc is null)
        {
            Console.WriteLine("Ошибка: счёт не найден.");
            return;
        }

        Console.Write($"Вы уверены, что хотите удалить счёт '{acc.Name}'? (y/n): ");
        var confirm = Console.ReadLine()?.Trim().ToLowerInvariant();
        if (confirm != "y")
        {
            Console.WriteLine("Отменено пользователем.");
            return;
        }

        try
        {
            _accounts.Delete(id);
            Console.WriteLine("OK: счёт удалён.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при удалении: {ex.Message}");
        }
    }
}