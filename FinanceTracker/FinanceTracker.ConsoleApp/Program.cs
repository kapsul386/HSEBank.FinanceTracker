using Microsoft.Extensions.DependencyInjection;
using FinanceTracker.Application.Abstractions;
using FinanceTracker.Application.Services;
using FinanceTracker.Application.Commands;
using FinanceTracker.Domain.Entities;
using FinanceTracker.Domain.Factories;
using FinanceTracker.Infrastructure.Repositories;
using FinanceTracker.ConsoleApp.Commands;

// создаём контейнер DI
var services = new ServiceCollection();

// регистрируем фабрику доменных сущностей
services.AddSingleton<IDomainFactory, DomainFactory>();

// регистрируем репозитории (по типам)
services.AddSingleton<IRepository<BankAccount>>(sp =>
    new MemoryRepository<BankAccount>(x => x.Id));
services.AddSingleton<IRepository<Category>>(sp =>
    new MemoryRepository<Category>(x => x.Id));
services.AddSingleton<IRepository<Operation>>(sp =>
    new MemoryRepository<Operation>(x => x.Id));

// регистрируем фасады (сервисы)
services.AddSingleton<AccountsService>();
services.AddSingleton<CategoriesService>();
services.AddSingleton<OperationsService>();
services.AddSingleton<AnalyticsService>();

// строим контейнер
var provider = services.BuildServiceProvider();

// получаем зависимости
var factory     = provider.GetRequiredService<IDomainFactory>();
var accounts    = provider.GetRequiredService<AccountsService>();
var categories  = provider.GetRequiredService<CategoriesService>();
var operations  = provider.GetRequiredService<OperationsService>();
var analytics   = provider.GetRequiredService<AnalyticsService>();

// создаём список команд
List<ICommand> commands =
[
    // Accounts
    new TimingCommandDecorator(new AddAccount(accounts, factory)),
    new TimingCommandDecorator(new ListAccounts(accounts)),

    // Categories
    new TimingCommandDecorator(new AddCategory(categories, factory)),
    new TimingCommandDecorator(new ListCategories(categories)),

    // Operations
    new TimingCommandDecorator(new AddOperation(operations, accounts, categories, factory)),
    new TimingCommandDecorator(new ListOperations(operations)),

    // Analytics
    new TimingCommandDecorator(new ReportSummary(analytics)),
    new TimingCommandDecorator(new ReportByCategoryCommand(analytics)),

    // Exit
    new Exit()
];

// основной цикл приложения
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
