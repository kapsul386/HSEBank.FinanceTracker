using FinanceTracker.Application.Commands;
using FinanceTracker.Application.Services;
using FinanceTracker.Domain.Entities;

namespace FinanceTracker.ConsoleApp.Commands;

/// <summary>
/// Command that edits a category's name by ID.
/// NOTE: Changing <see cref="Category.Type"/> is not supported by the domain model
/// (property is read-only). To switch type, create a new category and migrate operations.
/// </summary>
public sealed class EditCategory : ICommand
{
    private readonly CategoriesService _categories;

    /// <summary>Console command name.</summary>
    public string Name => "edit-category";

    /// <summary>Short description shown in the help list.</summary>
    public string Description => "Rename a category by ID (type change not supported)";

    public EditCategory(CategoriesService categories)
    {
        _categories = categories;
    }

    /// <summary>
    /// Executes the command: asks for category ID and optionally renames it.
    /// If user enters a new type, we inform that type is immutable.
    /// </summary>
    public void Run()
    {
        Console.Write("Category ID: ");
        var idText = Console.ReadLine();

        if (!Guid.TryParse(idText, out var id))
        {
            Console.WriteLine("Error: invalid ID format.");
            return;
        }

        var cat = _categories.Get(id);
        if (cat is null)
        {
            Console.WriteLine("Error: category not found.");
            return;
        }

        // --- Rename ---
        Console.Write($"New name [{cat.Name}] (press Enter to keep): ");
        var newName = Console.ReadLine();
        var nameChanged = false;

        if (!string.IsNullOrWhiteSpace(newName) && newName.Trim() != cat.Name)
        {
            try
            {
                cat.Rename(newName);
                nameChanged = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Rename error: {ex.Message}");
                return;
            }
        }

        // --- Type (immutable in current domain) ---
        Console.Write($"New type request [{cat.Type}] (Income/Expense, press Enter to skip): ");
        var typeInput = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(typeInput))
        {
            Console.WriteLine("Notice: changing category type is not supported by the domain model.");
            Console.WriteLine("Create a new category with the desired type and migrate operations if needed.");
        }

        if (!nameChanged)
        {
            Console.WriteLine("No changes made.");
            return;
        }

        _categories.Update(cat);
        Console.WriteLine("Category updated successfully.");
    }
}
