using Microsoft.Extensions.DependencyInjection;
using FinanceTracker.Application.Abstractions;
using FinanceTracker.Application.Services;
using FinanceTracker.Application.Commands;
using FinanceTracker.Application.Templates;
using FinanceTracker.Domain.Entities;
using FinanceTracker.Domain.Factories;
using FinanceTracker.Infrastructure.Repositories;
using FinanceTracker.ConsoleApp.Commands;

// Создание контейнера зависимостей
var services = new ServiceCollection();

// Регистрация фабрики доменных сущностей
services.AddSingleton<IDomainFactory, DomainFactory>();

// Регистрация репозиториев (по типам)
services.AddSingleton<IRepository<BankAccount>>(sp =>
    new MemoryRepository<BankAccount>(x => x.Id));
services.AddSingleton<IRepository<Category>>(sp =>
    new MemoryRepository<Category>(x => x.Id));
services.AddSingleton<IRepository<Operation>>(sp =>
    new MemoryRepository<Operation>(x => x.Id));

// Регистрация фасадов (сервисов)
services.AddSingleton<AccountsService>();
services.AddSingleton<CategoriesService>();
services.AddSingleton<OperationsService>();
services.AddSingleton<AnalyticsService>();

// Регистрация импортёра (Template Method)
services.AddSingleton<OperationsCsvImporter>();

// Регистрация сервиса экспорта (Visitor)
services.AddSingleton<ExportService>();

// Построение контейнера
var provider = services.BuildServiceProvider();

// Получение зависимостей
var factory     = provider.GetRequiredService<IDomainFactory>();
var accounts    = provider.GetRequiredService<AccountsService>();
var categories  = provider.GetRequiredService<CategoriesService>();
var operations  = provider.GetRequiredService<OperationsService>();
var analytics   = provider.GetRequiredService<AnalyticsService>();
var importer    = provider.GetRequiredService<OperationsCsvImporter>();
var export      = provider.GetRequiredService<ExportService>();

// Формирование списка доступных команд
List<ICommand> commands =
[
    // Работа со счетами
    new TimingCommandDecorator(new AddAccount(accounts, factory)),
    new TimingCommandDecorator(new ListAccounts(accounts)),
    new TimingCommandDecorator(new EditAccount(accounts)),         // <- добавлено
    new TimingCommandDecorator(new DeleteAccount(accounts)),       // <- добавлено

    // Работа с категориями
    new TimingCommandDecorator(new AddCategory(categories, factory)),
    new TimingCommandDecorator(new ListCategories(categories)),
    new TimingCommandDecorator(new EditCategory(categories)),      // <- добавлено
    new TimingCommandDecorator(new DeleteCategory(categories)),    // <- добавлено

    // Работа с операциями
    new TimingCommandDecorator(new AddOperation(operations, accounts, categories, factory)),
    new TimingCommandDecorator(new ListOperations(operations)),
    new TimingCommandDecorator(new EditOperation(operations, accounts, categories)), // <- добавлено
    new TimingCommandDecorator(new DeleteOperation(operations)),                     // <- добавлено

    // Аналитические отчёты
    new TimingCommandDecorator(new ReportSummary(analytics)),
    new TimingCommandDecorator(new ReportByCategoryCommand(analytics)),

    // Импорт/экспорт
    new TimingCommandDecorator(new ImportOperationsCommand(importer)),
    new TimingCommandDecorator(new ExportOperations(export)),      // <- добавлено

    // Завершение работы
    new Exit()
];

// Основной цикл приложения
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
