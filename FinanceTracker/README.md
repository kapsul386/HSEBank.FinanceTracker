# HSEBank.FinanceTracker

Консольное приложение для учёта личных финансов.  
Проект реализован в рамках курса **"Конструирование программного обеспечения"**.

## 1. Общая идея

Приложение моделирует модуль **«Учёт финансов»** — позволяет управлять счетами, категориями и операциями (доходы/расходы), выполнять простую аналитику и импорт/экспорт данных.

Реализованный функционал:

- Создание, редактирование, удаление **счетов**, **категорий** и **операций**
- Просмотр всех сущностей через консольные команды
- Аналитика: сводка доходов/расходов и группировка по категориям
- Измерение времени выполнения сценариев (через `TimingCommandDecorator`)
- Импорт CSV (Template Method)
- Экспорт CSV (Visitor)
- In-memory кэширование (Proxy)
- Использование DI-контейнера `Microsoft.Extensions.DependencyInjection`

---

## 2. Архитектура проекта

```
FinanceTracker/
├── Domain/
│   ├── Entities/ (BankAccount, Category, Operation)
│   └── Factories/ (DomainFactory, IDomainFactory)
│
├── Application/
│   ├── Abstractions/ (IRepository)
│   ├── Services/
│   │   ├── AccountsService, CategoriesService, OperationsService
│   │   ├── AnalyticsService, ExportService
│   │   └── ReportSort/ (AmountDescSort, NameAscSort)
│   ├── Templates/ (ImportTemplate, AccountsCsvImporter, OperationsCsvImporter)
│   └── Export/ (CsvExportVisitor)
│
├── Infrastructure/
│   └── Repositories/ (MemoryRepository, CachedRepositoryProxy)
│
├── ConsoleApp/
│   ├── Program.cs
│   └── Commands/ (... все команды CRUD и аналитики ...)
│
└── Tests/ (модульные тесты при необходимости)
```

---

## 3. Паттерны проектирования

| Паттерн | Где реализован | Назначение |
|----------|----------------|------------|
| **Facade** | Сервисы | Инкапсулируют бизнес-логику |
| **Command** | Все команды | Каждая пользовательская операция — отдельная команда |
| **Decorator** | TimedCommand | Измеряет время выполнения команд |
| **Template Method** | ImportTemplate | Унифицирует процесс импорта |
| **Visitor** | CsvExportVisitor | Экспортирует сущности в CSV |
| **Proxy** | CachedRepositoryProxy | Кэширует доступ к данным |
| **Factory** | DomainFactory | Создаёт валидные объекты |
| **Strategy** | ReportSort | Разные способы сортировки аналитики |

---

## 4. Принципы SOLID и GRASP

| Принцип | Где реализован | Суть |
|----------|----------------|------|
| **S (SRP)** | Сервисы и команды | Один класс = одна ответственность |
| **O (OCP)** | Интерфейсы команд и импорта | Расширение без модификации |
| **L (LSP)** | Репозитории | Любая реализация `IRepository` взаимозаменяема |
| **I (ISP)** | Разделение интерфейсов | Модули зависят только от нужных контрактов |
| **D (DIP)** | DI контейнер | Инверсии зависимостей через интерфейсы |

---

## 5. Примеры CSV

### Импорт операций
```csv
Type,AccountId,Amount,Date,CategoryId,Description
Income,6f5209f7-8f64-4e52-8a4c-97ff247c9ed9,1000,2025-10-01,22e88b2c-6ac3-4d64-9828-5d2c8d50a8b9,Salary
Expense,6f5209f7-8f64-4e52-8a4c-97ff247c9ed9,150,2025-10-03,adbf5e2f-1c4a-4992-b36e-2376b7f3cd41,Coffee
```

### Экспорт операций
```csv
id,type,bankAccountId,amount,date,categoryId,description
b1c5f8a3-1ef3-4dc3-9c39-72cf30b2c9e0,Income,6f5209f7-8f64-4e52-8a4c-97ff247c9ed9,1000,2025-10-01,22e88b2c-6ac3-4d64-9828-5d2c8d50a8b9,Salary
```

---

## 6. Запуск

```bash
git clone https://github.com/kapsul386/HSEBank.FinanceTracker.git
cd HSEBank.FinanceTracker/FinanceTracker.ConsoleApp
dotnet restore
dotnet build
dotnet run
```

---

## 7. Автор

**Имя:** Дмитрий Купцов
**Курс:** ФКН ВШЭ, Программная инженерия, 2 курс, БПИ-246  
**Год:** 2025
