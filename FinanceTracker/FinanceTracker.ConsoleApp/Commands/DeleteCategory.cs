using FinanceTracker.Application.Commands;
using FinanceTracker.Application.Services;

namespace FinanceTracker.ConsoleApp.Commands;

/// <summary>
/// Command that deletes a financial category by its ID.
/// Implements the <b>Command</b> design pattern — encapsulates a single user action.
/// </summary>
public sealed class DeleteCategory : ICommand
{
    private readonly CategoriesService _categories;

    /// <summary>
    /// Console command name.
    /// </summary>
    public string Name => "delete-category";

    /// <summary>
    /// Short description displayed in the help list.
    /// </summary>
    public string Description => "Delete a category by ID";

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteCategory"/> command.
    /// </summary>
    /// <param name="categories">Service responsible for managing categories.</param>
    public DeleteCategory(CategoriesService categories)
    {
        _categories = categories;
    }

    /// <summary>
    /// Executes the command: requests a category ID, confirms the action, and deletes it.
    /// </summary>
    public void Run()
    {
        Console.Write("Enter category ID to delete: ");
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

        Console.Write($"Are you sure you want to delete category '{cat.Name}'? (y/n): ");
        var confirm = Console.ReadLine()?.Trim().ToLowerInvariant();

        if (confirm != "y")
        {
            Console.WriteLine("Operation cancelled by user.");
            return;
        }

        try
        {
            _categories.Delete(id);
            Console.WriteLine("Category deleted successfully.");
        }
        catch (Exception ex)
        {
            // If referential integrity (linked operations) is enforced, this block catches those exceptions.
            Console.WriteLine($"Error while deleting category: {ex.Message}");
        }
    }
}
