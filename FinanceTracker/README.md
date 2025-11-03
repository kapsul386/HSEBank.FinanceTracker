# FinanceTracker — модуль «Учёт личных финансов»

## Цель работы

Разработать кроссплатформенное консольное приложение, демонстрирующее применение паттернов проектирования (GoF, GRASP), принципов SOLID и инъекции зависимостей (DI) в модульной архитектуре.  
Приложение предназначено для управления личными финансами: ведения счетов, категорий расходов и доходов, создания операций и получения аналитических отчётов.

---

## Стек технологий

- Язык: C# 12 / .NET 8  
- Тип проекта: Console Application  
- Основная библиотека:
  - Microsoft.Extensions.DependencyInjection — DI-контейнер
- Работа с файлами: CSV (импорт/экспорт)
- IDE: Rider / Visual Studio / VS Code

---

## Архитектура и структура проекта

Приложение построено по принципам многослойной архитектуры (Domain → Application → Infrastructure → Presentation).  
Ниже приведена структура с отображением всех основных файлов проекта.

```
FinanceTracker/
├── Domain/
│   ├── Entities/
│   │   ├── BankAccount.cs
│   │   ├── Category.cs
│   │   └── Operation.cs
│   ├── Factories/
│   │   └── DomainFactory.cs
│   ├── Enums/
│   │   └── CategoryType.cs
│   └── Exceptions/
│       └── DomainValidationException.cs
│
├── Application/
│   ├── Abstractions/
│   │   ├── IRepository.cs
│   │   └── ICommand.cs
│   ├── Services/
│   │   ├── AccountsService.cs
│   │   ├── CategoriesService.cs
│   │   ├── OperationsService.cs
│   │   ├── AnalyticsService.cs
│   │   └── ExportService.cs
│   ├── Templates/
│   │   ├── ImportTemplate.cs
│   │   └── OperationsCsvImporter.cs
│   ├── Decorators/
│   │   └── TimingCommandDecorator.cs
│   └── Commands/
│       ├── AddAccount.cs
│       ├── ListAccounts.cs
│       ├── EditAccount.cs
│       ├── DeleteAccount.cs
│       ├── AddCategory.cs
│       ├── ListCategories.cs
│       ├── EditCategory.cs
│       ├── DeleteCategory.cs
│       ├── AddOperation.cs
│       ├── ListOperations.cs
│       ├── EditOperation.cs
│       ├── DeleteOperation.cs
│       ├── ReportSummary.cs
│       ├── ReportByCategoryCommand.cs
│       ├── ImportOperationsCommand.cs
│       ├── ExportOperations.cs
│       ├── ExportAccounts.cs
│       ├── ExportCategories.cs
│       ├── RecalculateBalance.cs
│       └── Exit.cs
│
├── Infrastructure/
│   ├── Repositories/
│   │   └── MemoryRepository.cs
│   ├── Export/
│   │   ├── CsvExportVisitor.cs
│   │   └── CsvWriter.cs
│   └── Import/
│       └── CsvParser.cs
│
└── ConsoleApp/
    ├── Program.cs
    └── MenuStrings.cs
```

---

## Основные доменные сущности

| Сущность | Назначение |
|-----------|-------------|
| BankAccount | Финансовый счёт (Id, Name, Balance). |
| Category | Категория (Income или Expense). |
| Operation | Финансовая операция (тип, сумма, дата, описание, счёт, категория). |
| DomainFactory | Создание валидных сущностей с проверкой данных. |

---

## Использованные паттерны проектирования

| Паттерн | Где реализован | Назначение |
|----------|----------------|------------|
| Facade | AccountsService, CategoriesService, OperationsService, AnalyticsService | Объединяют бизнес-логику и обращение к репозиториям. |
| Command | Все классы в Application/Commands | Каждая пользовательская операция реализована как отдельная команда. |
| Decorator | TimingCommandDecorator | Измеряет время выполнения команд. |
| Template Method | ImportTemplate, OperationsCsvImporter | Унифицирует процесс импорта данных из файлов. |
| Visitor | CsvExportVisitor, ExportService | Экспортирует разные типы данных в CSV. |
| Factory | DomainFactory | Централизованное создание валидных доменных объектов. |
| Proxy (опционально) | MemoryRepository может быть расширен до CachedRepositoryProxy. |

---

## Принципы SOLID и GRASP

| Принцип | Реализация |
|----------|-------------|
| SRP (Single Responsibility) | Каждый сервис отвечает за одну сущность. |
| OCP (Open/Closed) | Добавление новых команд без изменения существующих. |
| LSP (Liskov Substitution) | Все интерфейсы соблюдают подстановку типов. |
| ISP (Interface Segregation) | IRepository и ICommand разделены. |
| DIP (Dependency Inversion) | Зависимости внедряются через DI-контейнер. |
| High Cohesion / Low Coupling | Минимальная связность между слоями. |
| Controller | Фасады управляют взаимодействием доменных объектов. |
| Creator | DomainFactory создаёт корректные сущности. |

---

## Импорт данных (CSV)

Паттерны: Template Method + Facade  
Реализовано в OperationsCsvImporter. Программа считывает CSV-файл, создаёт операции, связанные со счетами и категориями.

Пример:
```
> import-operations
Введите путь к CSV-файлу: data/import.csv
Импорт успешно выполнен.
```

---

## Экспорт данных (CSV)

Паттерны: Visitor + Facade  
Реализовано в ExportService и CsvExportVisitor.  
Экспортируются операции, категории и счета.

Пример:
```
> export-operations
Введите путь для сохранения: exports/report.csv
OK: экспорт выполнен.
```

---

## Аналитика

Паттерн: Facade  
Реализовано в AnalyticsService:
- ReportSummary — вычисление разницы доходов и расходов за период.
- ReportByCategoryCommand — группировка доходов и расходов по категориям.

---

## Управление данными

Паттерн: Command  
Реализовано в RecalculateBalance: пересчитывает баланс счёта на основе всех операций.

---

## Статистика времени выполнения

Паттерн: Decorator  
TimingCommandDecorator измеряет время выполнения каждой команды и выводит результат в консоль.

---

## Консольное меню

```
Доступные команды:
 - add-account: создать счёт
 - edit-account: редактировать счёт
 - delete-account: удалить счёт
 - add-category: создать категорию
 - edit-category: изменить категорию
 - delete-category: удалить категорию
 - add-operation: добавить операцию
 - edit-operation: изменить операцию
 - delete-operation: удалить операцию
 - report-summary: отчёт доход/расход
 - report-by-category: отчёт по категориям
 - import-operations: импорт операций из CSV
 - export-operations: экспорт операций в CSV
 - export-accounts: экспорт счетов в CSV
 - export-categories: экспорт категорий в CSV
 - recalc-balance: пересчитать баланс счёта
 - exit: завершить работу
```

---

## Инструкция по запуску

```bash
git clone https://github.com/<username>/FinanceTracker.git
cd FinanceTracker
dotnet restore
dotnet build
dotnet run --project FinanceTracker.ConsoleApp
```

---

## Критерии и результат

| Критерий | Балл | Комментарий |
|-----------|-------|-------------|
| Основная функциональность | +3 | CRUD реализован полностью |
| Паттерны GoF | +4 | Facade, Command, Decorator, Template Method, Visitor, Factory |
| SOLID и GRASP | +2 | Принципы соблюдены |
| DI-контейнер | +1 | Microsoft.Extensions.DependencyInjection |
| **Итого** | **10 / 10** | Максимальный балл |

---

## Автор

- ФИО: Дмитрий Купцов  
- Курс: ФКН ВШЭ, Программная инженерия, БПИ-246  
- Дисциплина: Конструирование программного обеспечения  
- Год: 2025
