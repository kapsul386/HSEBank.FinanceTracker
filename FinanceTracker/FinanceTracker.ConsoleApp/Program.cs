using Microsoft.Extensions.DependencyInjection;
using FinanceTracker.Application.Abstractions;
using FinanceTracker.Application.Services;
using FinanceTracker.Domain.Entities;
using FinanceTracker.Domain.Factories;
using FinanceTracker.Infrastructure.Repositories;

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
var factory = provider.GetRequiredService<IDomainFactory>();
var accounts = provider.GetRequiredService<AccountsService>();

var account = factory.CreateBankAccount("Основной счёт", 1000);
accounts.Add(account);

Console.WriteLine($"Создан счёт: {account.Name}, баланс {account.Balance}₽");
Console.WriteLine($"Всего счетов: {accounts.List().Count}");