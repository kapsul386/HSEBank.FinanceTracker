using Microsoft.Extensions.DependencyInjection;
using FinanceTracker.Application.Abstractions;
using FinanceTracker.Application.Services;
using FinanceTracker.Application.Commands;
using FinanceTracker.Application.Templates;
using FinanceTracker.Domain.Entities;
using FinanceTracker.Domain.Factories;
using FinanceTracker.Infrastructure.Repositories;
using FinanceTracker.ConsoleApp.Commands;
using FinanceTracker.Application.Services.ReportSort;         
using FinanceTracker.Application.Templates;   // AccountsCsvImporter
using FinanceTracker.ConsoleApp.Commands;    // ImportAccountsCommand
// DI-контейнер
// ------------------------------
var services = new ServiceCollection();

// Фабрика доменных сущностей
services.AddSingleton<IDomainFactory, DomainFactory>();

// Репозитории + Proxy (кэширующий прокси над памятью)
services.AddSingleton<IRepository<BankAccount>>(sp =>
    new CachedRepositoryProxy<BankAccount>(
        new MemoryRepository<BankAccount>(x => x.Id), x => x.Id));

services.AddSingleton<IRepository<Category>>(sp =>
    new CachedRepositoryProxy<Category>(
        new MemoryRepository<Category>(x => x.Id), x => x.Id));

services.AddSingleton<IRepository<Operation>>(sp =>
    new CachedRepositoryProxy<Operation>(
        new MemoryRepository<Operation>(x => x.Id), x => x.Id));

// Фасады/сервисы
services.AddSingleton<AccountsService>();
services.AddSingleton<CategoriesService>();
services.AddSingleton<OperationsService>();
services.AddSingleton<AnalyticsService>();
services.AddSingleton<ExportService>();
services.AddSingleton<AccountsCsvImporter>();

// Импорт (Template Method)
services.AddSingleton<OperationsCsvImporter>();

// Strategy для сортировки отчёта по категориям
services.AddSingleton<IReportSortStrategy, AmountDescSort>();
services.AddSingleton<IReportSortStrategy, NameAscSort>();

// ------------------------------
// Построение контейнера и получение сервисов
// ------------------------------
var provider  = services.BuildServiceProvider();
var importAccounts = provider.GetRequiredService<AccountsCsvImporter>();


var factory    = provider.GetRequiredService<IDomainFactory>();
var accounts   = provider.GetRequiredService<AccountsService>();
var categories = provider.GetRequiredService<CategoriesService>();
var operations = provider.GetRequiredService<OperationsService>();
var analytics  = provider.GetRequiredService<AnalyticsService>();
var importer   = provider.GetRequiredService<OperationsCsvImporter>();
var export     = provider.GetRequiredService<ExportService>();
var strategies = provider.GetServices<IReportSortStrategy>();

// ------------------------------
// Регистрация команд
// ------------------------------
List<ICommand> commands =
[
    // Счета
    new TimingCommandDecorator(new AddAccount(accounts, factory)),
    new TimingCommandDecorator(new ListAccounts(accounts)),
    new TimingCommandDecorator(new EditAccount(accounts)),
    new TimingCommandDecorator(new DeleteAccount(accounts)),

    // Категории
    new TimingCommandDecorator(new AddCategory(categories, factory)),
    new TimingCommandDecorator(new ListCategories(categories)),
    new TimingCommandDecorator(new EditCategory(categories)),
    new TimingCommandDecorator(new DeleteCategory(categories)),

    // Операции
    new TimingCommandDecorator(new AddOperation(operations, accounts, categories, factory)),
    new TimingCommandDecorator(new ListOperations(operations)),
    new TimingCommandDecorator(new EditOperation(operations, accounts, categories)),
    new TimingCommandDecorator(new DeleteOperation(operations)),

    // Аналитика
    new TimingCommandDecorator(new ReportSummary(analytics)),
    new TimingCommandDecorator(new ReportByCategory(analytics, categories, strategies)),

    // Импорт / Экспорт (CSV)
    new TimingCommandDecorator(new ImportOperations(importer)),
    new TimingCommandDecorator(new ImportAccounts(importAccounts)),

    new TimingCommandDecorator(new ExportOperations(export)),
    new TimingCommandDecorator(new ExportAccounts(accounts)),
    new TimingCommandDecorator(new ExportCategories(categories)),

    // Управление данными
    new TimingCommandDecorator(new RecalculateBalance(accounts, operations, categories)),

    // Завершение
    new Exit()
];

// ------------------------------
// Главный цикл
// ------------------------------
while (true)
{
    Console.WriteLine("\nДоступные команды:");
    foreach (var c in commands)
        Console.WriteLine($" - {c.Name}: {c.Description}");

    Console.Write("\n> ");
    var input = (Console.ReadLine() ?? "").Trim();

    var cmd = commands.FirstOrDefault(c =>
        c.Name.Equals(input, StringComparison.OrdinalIgnoreCase));

    if (cmd is null)
    {
        Console.WriteLine("Неизвестная команда");
        continue;
    }

    try
    {
        cmd.Run();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Ошибка: {ex.Message}");
    }
}