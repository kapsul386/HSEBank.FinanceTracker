using FinanceTracker.Application.Commands;
using FinanceTracker.Application.Services;

namespace FinanceTracker.ConsoleApp.Commands;

/// <summary>
/// Удаляет категорию по Id.
/// Команда: delete-category
/// </summary>
public sealed class DeleteCategory : ICommand
{
    private readonly CategoriesService _categories;

    public string Name => "delete-category";
    public string Description => "Удалить категорию по Id";

    public DeleteCategory(CategoriesService categories)
    {
        _categories = categories;
    }

    public void Run()
    {
        Console.Write("Id категории для удаления: ");
        var idText = Console.ReadLine();

        if (!Guid.TryParse(idText, out var id))
        {
            Console.WriteLine("Ошибка: неверный формат Id.");
            return;
        }

        var cat = _categories.Get(id);
        if (cat is null)
        {
            Console.WriteLine("Ошибка: категория не найдена.");
            return;
        }

        Console.Write($"Вы уверены, что хотите удалить категорию '{cat.Name}'? (y/n): ");
        var confirm = Console.ReadLine()?.Trim().ToLowerInvariant();
        if (confirm != "y")
        {
            Console.WriteLine("Отменено пользователем.");
            return;
        }

        try
        {
            _categories.Delete(id);
            Console.WriteLine("OK: категория удалена.");
        }
        catch (Exception ex)
        {
            // Если сервис/репозиторий проверяет ссылочную целостность (операции с этой категорией),
            // здесь поймаем исключение и покажем сообщение пользователю.
            Console.WriteLine($"Ошибка при удалении: {ex.Message}");
        }
    }
}