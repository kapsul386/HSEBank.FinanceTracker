using Microsoft.Extensions.DependencyInjection;
using FinanceTracker.Application.Abstractions;
using FinanceTracker.Application.Services;
using FinanceTracker.Domain.Entities;
using FinanceTracker.Domain.Factories;
using FinanceTracker.Infrastructure.Repositories;
using FinanceTracker.Application.Commands;
using FinanceTracker.ConsoleApp.Commands;

// создаем контейнер DI
var services = new ServiceCollection();

// регистрируем фабрику
services.AddSingleton<IDomainFactory, DomainFactory>();

// регистрируем репозитории (по типам)
services.AddSingleton<IRepository<BankAccount>>(sp => new MemoryRepository<BankAccount>(x => x.Id));
services.AddSingleton<IRepository<Category>>(sp => new MemoryRepository<Category>(x => x.Id));
services.AddSingleton<IRepository<Operation>>(sp => new MemoryRepository<Operation>(x => x.Id));

// регистрируем сервисы (фасады)
services.AddSingleton<AccountsService>();
services.AddSingleton<CategoriesService>();
services.AddSingleton<OperationsService>();

var provider = services.BuildServiceProvider();

// тест: создадим счёт и выведем
// получаем сервисы
var accounts = provider.GetRequiredService<AccountsService>();
var factory  = provider.GetRequiredService<IDomainFactory>();

// собираем список команд (оборачиваем в декоратор тайминга)
List<ICommand> commands = new()
{
    new TimingCommandDecorator(new AddAccount(accounts, factory)),
    new TimingCommandDecorator(new ListAccounts(accounts)),
    new Exit() // выход не таймим — по желанию можно тоже завернуть
};

// цикл меню
while (true)
{
    Console.WriteLine("\nДоступные команды:");
    foreach (var c in commands)
        Console.WriteLine($" - {c.Name}: {c.Description}");

    Console.Write("\n> ");
    var input = (Console.ReadLine() ?? "").Trim();

    var cmd = commands.FirstOrDefault(c => c.Name.Equals(input, StringComparison.OrdinalIgnoreCase));
    if (cmd is null)
    {
        Console.WriteLine("Неизвестная команда");
        continue;
    }
    var categories = provider.GetRequiredService<CategoriesService>();
    commands.Add(new TimingCommandDecorator(new AddCategory(categories, factory)));
    commands.Add(new TimingCommandDecorator(new ListCategories(categories)));

    var ops = provider.GetRequiredService<OperationsService>();
    commands.Add(new TimingCommandDecorator(new AddOperation(ops, accounts, categories, factory)));
    commands.Add(new TimingCommandDecorator(new ListOperations(ops)));

    cmd.Run();
}