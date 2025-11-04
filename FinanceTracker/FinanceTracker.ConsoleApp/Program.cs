using Microsoft.Extensions.DependencyInjection;

using FinanceTracker.Application.Abstractions;
using FinanceTracker.Application.Services;
using FinanceTracker.Application.Services.ReportSort;
using FinanceTracker.Application.Commands;
using FinanceTracker.Application.Templates;

using FinanceTracker.Domain.Entities;
using FinanceTracker.Domain.Factories;

using FinanceTracker.Infrastructure.Repositories;

using FinanceTracker.ConsoleApp.Commands;



var services = new ServiceCollection();

services.AddSingleton<IDomainFactory, DomainFactory>();

services.AddSingleton<IRepository<BankAccount>>(_ =>
    new CachedRepositoryProxy<BankAccount>(
        new MemoryRepository<BankAccount>(x => x.Id), x => x.Id));

services.AddSingleton<IRepository<Category>>(_ =>
    new CachedRepositoryProxy<Category>(
        new MemoryRepository<Category>(x => x.Id), x => x.Id));

services.AddSingleton<IRepository<Operation>>(_ =>
    new CachedRepositoryProxy<Operation>(
        new MemoryRepository<Operation>(x => x.Id), x => x.Id));

services.AddSingleton<AccountsService>();
services.AddSingleton<CategoriesService>();
services.AddSingleton<OperationsService>();
services.AddSingleton<AnalyticsService>();
services.AddSingleton<ExportService>();
services.AddSingleton<AccountsCsvImporter>();
services.AddSingleton<OperationsCsvImporter>();
services.AddSingleton<IReportSortStrategy, AmountDescSort>();
services.AddSingleton<IReportSortStrategy, NameAscSort>();
var provider = services.BuildServiceProvider();

var importAccounts = provider.GetRequiredService<AccountsCsvImporter>();

var factory    = provider.GetRequiredService<IDomainFactory>();
var accounts   = provider.GetRequiredService<AccountsService>();
var categories = provider.GetRequiredService<CategoriesService>();
var operations = provider.GetRequiredService<OperationsService>();
var analytics  = provider.GetRequiredService<AnalyticsService>();
var importer   = provider.GetRequiredService<OperationsCsvImporter>();
var export     = provider.GetRequiredService<ExportService>();
var strategies = provider.GetServices<IReportSortStrategy>();
List<ICommand> commands =
[
    // Accounts
    new TimingCommandDecorator(new AddAccount(accounts, factory)),
    new TimingCommandDecorator(new ListAccounts(accounts)),
    new TimingCommandDecorator(new EditAccount(accounts)),
    new TimingCommandDecorator(new DeleteAccount(accounts)),

    // Categories
    new TimingCommandDecorator(new AddCategory(categories, factory)),
    new TimingCommandDecorator(new ListCategories(categories)),
    new TimingCommandDecorator(new EditCategory(categories)),
    new TimingCommandDecorator(new DeleteCategory(categories)),

    // Operations
    new TimingCommandDecorator(new AddOperation(operations, accounts, categories, factory)),
    new TimingCommandDecorator(new ListOperations(operations)),
    new TimingCommandDecorator(new EditOperation(operations, accounts, categories)),
    new TimingCommandDecorator(new DeleteOperation(operations)),

    // Analytics
    new TimingCommandDecorator(new ReportSummary(analytics)),
    new TimingCommandDecorator(new ReportByCategory(analytics, categories, strategies)),

    // Import / Export (CSV)
    new TimingCommandDecorator(new ImportOperations(importer)),
    new TimingCommandDecorator(new ImportAccounts(importAccounts)),

    new TimingCommandDecorator(new ExportOperations(export)),
    new TimingCommandDecorator(new ExportAccounts(accounts)),
    new TimingCommandDecorator(new ExportCategories(categories)),

    // Data maintenance
    new TimingCommandDecorator(new RecalculateBalance(accounts, operations, categories)),

    // Exit
    new Exit()
];

while (true)
{
    Console.WriteLine("\nAvailable commands:");
    foreach (var c in commands)
        Console.WriteLine($" - {c.Name}: {c.Description}");

    Console.Write("\n> ");
    var input = (Console.ReadLine() ?? "").Trim();

    var cmd = commands.FirstOrDefault(c =>
        c.Name.Equals(input, StringComparison.OrdinalIgnoreCase));

    if (cmd is null)
    {
        Console.WriteLine("Unknown command");
        continue;
    }

    try
    {
        cmd.Run();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}
