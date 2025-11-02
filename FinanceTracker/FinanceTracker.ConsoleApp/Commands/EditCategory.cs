using FinanceTracker.Application.Commands;
using FinanceTracker.Application.Services;

namespace FinanceTracker.ConsoleApp.Commands;

/// <summary>
/// Редактирует категорию: имя и/или тип (Income/Expense).
/// Команда: edit-category
/// </summary>
public sealed class EditCategory : ICommand
{
    private readonly CategoriesService _categories;

    public string Name => "edit-category";
    public string Description => "Переименовать категорию и/или изменить тип по Id";

    public EditCategory(CategoriesService categories)
    {
        _categories = categories;
    }

    public void Run()
    {
        Console.Write("Id категории: ");
        var idText = Console.ReadLine();

        if (!Guid.TryParse(idText, out var id))
        {
            Console.WriteLine("Ошибка: неверный формат Id.");
            return;
        }

        var cat = _categories.Get(id);
        if (cat is null)
        {
            Console.WriteLine("Ошибка: категория с таким Id не найдена.");
            return;
        }

        // --- Имя ---
        Console.Write($"Новое имя [{cat.Name}] (Enter — оставить без изменений): ");
        var newName = Console.ReadLine();
        var nameChanged = false;
        if (!string.IsNullOrWhiteSpace(newName) && newName!.Trim() != cat.Name)
        {
            try
            {
                // Ищем доменный метод переименования
                var rename = cat.GetType().GetMethod("Rename");
                if (rename != null)
                {
                    rename.Invoke(cat, new object[] { newName! });
                    nameChanged = true;
                }
                else
                {
                    Console.WriteLine("Предупреждение: у Category не найден метод Rename. Имя не изменено.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка переименования: {ex.Message}");
                return;
            }
        }

        // --- Тип ---
        Console.Write($"Новый тип [{cat.Type}] (Income/Expense, Enter — без изменений): ");
        var typeText = Console.ReadLine();
        var typeChanged = false;

        if (!string.IsNullOrWhiteSpace(typeText))
        {
            var typeStr = typeText!.Trim();

            // Попробуем распарсить enum того же типа, что и свойство cat.Type
            var typeProp = cat.GetType().GetProperty("Type");
            if (typeProp == null)
            {
                Console.WriteLine("Предупреждение: у Category нет свойства Type. Тип не изменён.");
            }
            else
            {
                var enumType = typeProp.PropertyType; // ожидаем enum, например CategoryType
                try
                {
                    if (Enum.TryParse(enumType, typeStr, ignoreCase: true, out var newEnum))
                    {
                        // Ищем один из доменных методов смены типа
                        var candidateNames = new[] { "ChangeType", "SetType", "UpdateType" };
                        var method = candidateNames
                            .Select(n => cat.GetType().GetMethod(n, new[] { enumType }))
                            .FirstOrDefault(m => m != null);

                        if (method != null)
                        {
                            method!.Invoke(cat, new[] { newEnum! });
                            typeChanged = true;
                        }
                        else
                        {
                            Console.WriteLine("Предупреждение: метод смены типа не найден (ChangeType/SetType/UpdateType). Тип не изменён.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Ошибка: допустимые значения типа — Income или Expense.");
                        return;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка смены типа: {ex.Message}");
                    return;
                }
            }
        }

        if (!nameChanged && !typeChanged)
        {
            Console.WriteLine("Изменений не внесено.");
            return;
        }

        _categories.Update(cat);
        Console.WriteLine("OK: категория обновлена.");
    }
}
