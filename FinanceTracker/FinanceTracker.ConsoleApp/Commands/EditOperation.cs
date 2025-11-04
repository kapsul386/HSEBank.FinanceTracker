using FinanceTracker.Application.Commands;
using FinanceTracker.Application.Services;
using System.Globalization;

namespace FinanceTracker.ConsoleApp.Commands;

/// <summary>
/// Edits an operation: amount, date, description, category, and/or account.
/// Command: <c>edit-operation</c>.
/// </summary>
public sealed class EditOperation(
    OperationsService operations,
    AccountsService accounts,
    CategoriesService categories)
    : ICommand
{
    /// <summary>Console command name.</summary>
    public string Name => "edit-operation";

    /// <summary>Short description shown in the help list.</summary>
    public string Description => "Edit amount/date/description/category/account of an operation by Id";

    /// <summary>
    /// Runs the interactive editing flow:
    /// asks for operation Id, shows current values as hints,
    /// and updates fields that the user provides.
    /// </summary>
    public void Run()
    {
        Console.Write("Operation Id: ");
        var idText = Console.ReadLine();
        if (!Guid.TryParse(idText, out var id))
        {
            Console.WriteLine("Error: invalid Id format.");
            return;
        }

        var op = operations.Get(id);
        if (op is null)
        {
            Console.WriteLine("Error: operation not found.");
            return;
        }

        // Current state for hints (via safe reflection getters)
        var currentAmount = GetPropDecimal(op, "Amount");
        var currentDate   = GetPropDate(op, "Date");
        var currentDesc   = GetPropString(op, "Description");
        var currentAccId  = GetPropGuid(op, "BankAccountId");
        var currentCatId  = GetPropGuid(op, "CategoryId");

        Console.Write($"New amount [{currentAmount}]: ");
        var amountText = Console.ReadLine();

        Console.Write($"New date [{currentDate:yyyy-MM-dd}] (format yyyy-MM-dd): ");
        var dateText = Console.ReadLine();

        Console.Write($"New description [{currentDesc}] (Enter — keep current): ");
        var descText = Console.ReadLine();

        Console.Write($"New account Id [{currentAccId}] (Enter — keep current): ");
        var accText = Console.ReadLine();

        Console.Write($"New category Id [{currentCatId}] (Enter — keep current): ");
        var catText = Console.ReadLine();

        var changed = false;

        // --- Amount ---
        if (!string.IsNullOrWhiteSpace(amountText))
        {
            if (!decimal.TryParse(amountText, NumberStyles.Number, CultureInfo.InvariantCulture, out var newAmount) ||
                newAmount <= 0)
            {
                Console.WriteLine("Error: amount must be a positive number.");
                return;
            }

            // Try a few method name variants to stay compatible with the domain model
            if (!TryInvoke(op, ["ChangeAmount", "SetAmount", "UpdateAmount"], [newAmount]))
            {
                Console.WriteLine("Warning: amount-changing method not found. Field not updated.");
            }
            else changed = true;
        }

        // --- Date ---
        if (!string.IsNullOrWhiteSpace(dateText))
        {
            if (!DateTime.TryParseExact(dateText, "yyyy-MM-dd", CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out var newDate))
            {
                Console.WriteLine("Error: invalid date format. Expected yyyy-MM-dd.");
                return;
            }

            if (!TryInvoke(op, ["ChangeDate", "SetDate", "UpdateDate"], [newDate]))
            {
                Console.WriteLine("Warning: date-changing method not found. Field not updated.");
            }
            else changed = true;
        }

        // --- Description ---
        if (!string.IsNullOrWhiteSpace(descText))
        {
            if (!TryInvoke(op, ["ChangeDescription", "SetDescription", "UpdateDescription"], [descText]))
            {
                Console.WriteLine("Warning: description-changing method not found. Field not updated.");
            }
            else changed = true;
        }

        // --- Account ---
        if (!string.IsNullOrWhiteSpace(accText))
        {
            if (!Guid.TryParse(accText, out var newAccId))
            {
                Console.WriteLine("Error: invalid account Id.");
                return;
            }

            if (accounts.Get(newAccId) is null)
            {
                Console.WriteLine("Error: specified account does not exist.");
                return;
            }

            if (!TryInvoke(op, ["ReassignAccount", "ChangeAccount", "SetBankAccount"], [newAccId]))
            {
                Console.WriteLine("Warning: account-changing method not found. Field not updated.");
            }
            else changed = true;
        }

        // --- Category ---
        if (!string.IsNullOrWhiteSpace(catText))
        {
            if (!Guid.TryParse(catText, out var newCatId))
            {
                Console.WriteLine("Error: invalid category Id.");
                return;
            }

            if (categories.Get(newCatId) is null)
            {
                Console.WriteLine("Error: specified category does not exist.");
                return;
            }

            if (!TryInvoke(op, ["ReassignCategory", "ChangeCategory", "SetCategory"], [newCatId]))
            {
                Console.WriteLine("Warning: category-changing method not found. Field not updated.");
            }
            else changed = true;
        }

        if (!changed)
        {
            Console.WriteLine("No changes were made.");
            return;
        }

        try
        {
            operations.Update(op);
            Console.WriteLine("OK: operation updated.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error while saving: {ex.Message}");
        }
    }

    // --- Helpers (reflection & safe getters) ---

    /// <summary>
    /// Tries to invoke the first available method by name that matches the provided argument types.
    /// </summary>
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

    /// <summary>Safely read a decimal property by name; returns 0 if missing or mismatched.</summary>
    private static decimal GetPropDecimal(object target, string name)
        => target.GetType().GetProperty(name)?.GetValue(target) is decimal v ? v : 0m;

    /// <summary>Safely read a <see cref="DateTime"/> property by name; returns <see cref="DateTime.MinValue"/> if missing or mismatched.</summary>
    private static DateTime GetPropDate(object target, string name)
        => target.GetType().GetProperty(name)?.GetValue(target) is DateTime v ? v : DateTime.MinValue;

    /// <summary>Safely read a string property by name; returns empty string if missing or mismatched.</summary>
    private static string GetPropString(object target, string name)
        => target.GetType().GetProperty(name)?.GetValue(target) as string ?? "";

    /// <summary>Safely read a <see cref="Guid"/> property by name; returns <see cref="Guid.Empty"/> if missing or mismatched.</summary>
    private static Guid GetPropGuid(object target, string name)
        => target.GetType().GetProperty(name)?.GetValue(target) is Guid v ? v : Guid.Empty;
}
