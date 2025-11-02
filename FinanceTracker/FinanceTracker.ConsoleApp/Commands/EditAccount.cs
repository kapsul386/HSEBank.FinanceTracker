using FinanceTracker.Application.Commands;
using FinanceTracker.Application.Services;
using FinanceTracker.Domain.Entities; // для BankAccount

namespace FinanceTracker.ConsoleApp.Commands;

/// <summary>
/// Редактирует счёт: имя и/или баланс по Id.
/// Команда: edit-account
/// </summary>
public sealed class EditAccount : ICommand
{
    private readonly AccountsService _accounts;

    public string Name => "edit-account";
    public string Description => "Переименовать счёт и/или обновить баланс по Id";

    public EditAccount(AccountsService accounts)
    {
        _accounts = accounts;
    }

    public void Run()
    {
        Console.Write("Id счёта: ");
        var idText = Console.ReadLine();

        if (!Guid.TryParse(idText, out var id))
        {
            Console.WriteLine("Ошибка: неверный формат Id.");
            return;
        }

        var acc = _accounts.Get(id);
        if (acc is null)
        {
            Console.WriteLine("Ошибка: счёт с таким Id не найден.");
            return;
        }

        // --- Имя ---
        Console.Write($"Новое имя [{acc.Name}] (Enter — оставить без изменений): ");
        var newName = Console.ReadLine();
        var nameChanged = false;
        if (!string.IsNullOrWhiteSpace(newName) && newName!.Trim() != acc.Name)
        {
            // В домене у категорий есть Rename(...); у счёта — та же логика.
            // Если метода Rename у BankAccount нет — скажи, сделаем быстро.
            try
            {
                // метод домена, который меняет Name с валидацией
                var rename = acc.GetType().GetMethod("Rename");
                if (rename != null)
                {
                    rename.Invoke(acc, new object[] { newName! });
                    nameChanged = true;
                }
                else
                {
                    Console.WriteLine("Предупреждение: метод Rename не найден у BankAccount. Имя не изменено.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка переименования: {ex.Message}");
                return;
            }
        }

        // --- Баланс ---
        Console.Write($"Новый баланс [{acc.Balance}] (Enter — оставить без изменений): ");
        var balText = Console.ReadLine();
        var balanceChanged = false;
        if (!string.IsNullOrWhiteSpace(balText))
        {
            if (!decimal.TryParse(balText, out var newBal))
            {
                Console.WriteLine("Ошибка: баланс должен быть числом.");
                return;
            }

            var delta = newBal - acc.Balance;
            try
            {
                if (delta > 0)
                {
                    acc.Credit(delta);   // пополнение
                    balanceChanged = true;
                }
                else if (delta < 0)
                {
                    acc.Debit(-delta);   // списание
                    balanceChanged = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка изменения баланса: {ex.Message}");
                return;
            }
        }

        if (!nameChanged && !balanceChanged)
        {
            Console.WriteLine("Изменений не внесено.");
            return;
        }

        _accounts.Update(acc);
        Console.WriteLine("OK: счёт обновлён.");
    }
}
