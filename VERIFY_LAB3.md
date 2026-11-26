# Лабораторна №3 — перевірка
Приходько О. Б., група 402.

З кореня репозиторію:
```sh
dotnet restore ConstructionCompany.sln
dotnet build ConstructionCompany.sln
dotnet test ConstructionCompany.sln
dotnet run --project ConstructionCompany.Console
```
Відкрити construction-ef.db у DB Browser for SQLite, перевірити всі таблиці та __EFMigrationsHistory. Для MongoDB запустити локальний сервер і виконати dotnet run --project ConstructionCompany.Console -- --mongo mongodb://localhost:27017. Додати справжні скриншоти до PDF. Наявний PDF не підтверджує запуск EF чи MongoDB.
