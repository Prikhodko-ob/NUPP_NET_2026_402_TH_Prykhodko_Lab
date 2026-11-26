# Лабораторна робота №2 — ConstructionCompany
.NET 8, Visual Studio 2022. Збережено код лабораторної №1; її демонстрація: `dotnet run --project ConstructionCompany.Console -- --lab1`.

## Запуск
Відкрийте ConstructionCompany.sln, встановіть Console стартовим проєктом, Ctrl+F5. Або з папки рішення:
```powershell
dotnet restore
dotnet run --project ConstructionCompany.Console | Tee-Object -FilePath Lab2_console.txt
dotnet test
```
Для здачі вставте фактичний вивід консолі або скриншот у PDF. Наданий PDF є звітом з очікуваними результатами, не підтвердженням запуску.

## GitHub
У наявному репозиторії виберіть lab1, створіть lab2 from lab1. Завантажте в корінь репозиторію папку ConstructionCompany_Lab1 з архіву, зберігаючи її назву та структуру. Commit changes у lab2. Створіть PR base lab1, compare lab2, якщо PR №1 ще відкритий; інакше base master (якщо lab1 вже інтегровано). Не зливайте PR самостійно без вимоги викладача.

## Реалізація
ConcurrentDictionary<Guid,string> зберігає JSON-копії; операції над колекцією завершуються синхронно та повертають Task, файловий запис справді асинхронний. SaveAsync серіалізується SemaphoreSlim у межах одного екземпляра. Для одного шляху використовуйте один екземпляр сервісу. Пагінація 1-based, порядок за Guid; між змінами колекції сторінки можуть зміщуватися. ReadAsync для відсутнього ID повертає faulted Task. Статистика випадкова. Тести xUnit потребують відновлення пакетів NuGet. Компіляція та виконання не перевірені у середовищі без SDK.
