using FinanceTracker.Application.Commands;
using FinanceTracker.Application.Services;
using System.Globalization;

namespace FinanceTracker.ConsoleApp.Commands;

/// <summary>
/// Редактирует операцию: сумму, дату, описание, категорию и/или счёт.
/// Команда: edit-operation
/// </summary>
public sealed class EditOperation : ICommand
{
    private readonly OperationsService _operations;
    private readonly AccountsService _accounts;
    private readonly CategoriesService _categories;

    public string Name => "edit-operation";
    public string Description => "Изменить сумму/дату/описание/категорию/счёт операции по Id";

    public EditOperation(
        OperationsService operations,
        AccountsService accounts,
        CategoriesService categories)
    {
        _operations = operations;
        _accounts = accounts;
        _categories = categories;
    }

    public void Run()
    {
        Console.Write("Id операции: ");
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

        // Текущее состояние для подсказок
        var currentAmount = GetPropDecimal(op, "Amount");
        var currentDate   = GetPropDate(op, "Date");
        var currentDesc   = GetPropString(op, "Description");
        var currentAccId  = GetPropGuid(op, "BankAccountId");
        var currentCatId  = GetPropGuid(op, "CategoryId");

        Console.Write($"Новая сумма [{currentAmount}]: ");
        var amountText = Console.ReadLine();

        Console.Write($"Новая дата [{currentDate:yyyy-MM-dd}] (формат yyyy-MM-dd): ");
        var dateText = Console.ReadLine();

        Console.Write($"Новое описание [{currentDesc}] (Enter — без изменений): ");
        var descText = Console.ReadLine();

        Console.Write($"Новый Id счёта [{currentAccId}] (Enter — без изменений): ");
        var accText = Console.ReadLine();

        Console.Write($"Новый Id категории [{currentCatId}] (Enter — без изменений): ");
        var catText = Console.ReadLine();

        var changed = false;

        // --- Сумма ---
        if (!string.IsNullOrWhiteSpace(amountText))
        {
            if (!decimal.TryParse(amountText, NumberStyles.Number, CultureInfo.InvariantCulture, out var newAmount) ||
                newAmount <= 0)
            {
                Console.WriteLine("Ошибка: сумма должна быть положительным числом.");
                return;
            }

            if (!TryInvoke(op, new[] { "ChangeAmount", "SetAmount", "UpdateAmount" }, new object[] { newAmount }))
            {
                Console.WriteLine("Предупреждение: метод смены суммы не найден. Поле не изменено.");
            }
            else changed = true;
        }

        // --- Дата ---
        if (!string.IsNullOrWhiteSpace(dateText))
        {
            if (!DateTime.TryParseExact(dateText, "yyyy-MM-dd", CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out var newDate))
            {
                Console.WriteLine("Ошибка: неверный формат даты. Ожидается yyyy-MM-dd.");
                return;
            }

            if (!TryInvoke(op, new[] { "ChangeDate", "SetDate", "UpdateDate" }, new object[] { newDate }))
            {
                Console.WriteLine("Предупреждение: метод смены даты не найден. Поле не изменено.");
            }
            else changed = true;
        }

        // --- Описание ---
        if (!string.IsNullOrWhiteSpace(descText))
        {
            if (!TryInvoke(op, new[] { "ChangeDescription", "SetDescription", "UpdateDescription" }, new object[] { descText! }))
            {
                Console.WriteLine("Предупреждение: метод смены описания не найден. Поле не изменено.");
            }
            else changed = true;
        }

        // --- Счёт ---
        if (!string.IsNullOrWhiteSpace(accText))
        {
            if (!Guid.TryParse(accText, out var newAccId))
            {
                Console.WriteLine("Ошибка: неверный Id счёта.");
                return;
            }

            if (_accounts.Get(newAccId) is null)
            {
                Console.WriteLine("Ошибка: такого счёта не существует.");
                return;
            }

            if (!TryInvoke(op, new[] { "ReassignAccount", "ChangeAccount", "SetBankAccount" }, new object[] { newAccId }))
            {
                Console.WriteLine("Предупреждение: метод смены счёта не найден. Поле не изменено.");
            }
            else changed = true;
        }

        // --- Категория ---
        if (!string.IsNullOrWhiteSpace(catText))
        {
            if (!Guid.TryParse(catText, out var newCatId))
            {
                Console.WriteLine("Ошибка: неверный Id категории.");
                return;
            }

            if (_categories.Get(newCatId) is null)
            {
                Console.WriteLine("Ошибка: такой категории не существует.");
                return;
            }

            if (!TryInvoke(op, new[] { "ReassignCategory", "ChangeCategory", "SetCategory" }, new object[] { newCatId }))
            {
                Console.WriteLine("Предупреждение: метод смены категории не найден. Поле не изменено.");
            }
            else changed = true;
        }

        if (!changed)
        {
            Console.WriteLine("Изменений не внесено.");
            return;
        }

        try
        {
            _operations.Update(op);
            Console.WriteLine("OK: операция обновлена.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при сохранении: {ex.Message}");
        }
    }

    // --- helpers (reflection & safe getters) ---

    private static bool TryInvoke(object target, string[] methodNames, object[] args)
    {
        var types = args.Select(a => a.GetType()).ToArray();
        foreach (var name in methodNames)
        {
            var m = target.GetType().GetMethod(name, types);
            if (m != null)
            {
                m.Invoke(target, args);
                return true;
            }
        }
        return false;
    }

    private static decimal GetPropDecimal(object target, string name)
        => target.GetType().GetProperty(name)?.GetValue(target) is decimal v ? v : 0m;

    private static DateTime GetPropDate(object target, string name)
        => target.GetType().GetProperty(name)?.GetValue(target) is DateTime v ? v : DateTime.MinValue;

    private static string GetPropString(object target, string name)
        => target.GetType().GetProperty(name)?.GetValue(target) as string ?? "";

    private static Guid GetPropGuid(object target, string name)
        => target.GetType().GetProperty(name)?.GetValue(target) is Guid v ? v : Guid.Empty;
}
