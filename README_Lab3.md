# Лабораторна №3 — ConstructionCompany (.NET 8)

У корені папки `ConstructionCompany_Lab1` відкрийте рішення `ConstructionCompany.sln`. Попередні проєкти та звіти збережено. Нові проєкти Infrastructure та Nosql.

## Запуск та докази
- Встановіть .NET 8 SDK і відновіть NuGet: `dotnet restore`.
- `dotnet run --project ConstructionCompany.Console` створює **construction-ef.db** через `Database.MigrateAsync()` та заповнює її. Зробіть скриншот консолі. Для повторного запуску використовується та сама база.
- `dotnet run --project ConstructionCompany.Console -- --existing construction.db` працює з наданою SQLite БД, створеною SQL, без повторного застосування міграції.
- У комплекті `construction.db` — реальна SQLite база, створена SQL-скриптом `schema.sql`; перевірка цілісності й зовнішніх ключів успішна. Зображення Evidence — візуалізовані фактичні результати SELECT з цієї бази, не скриншоти EF.
- Для перевірки саме міграції: `dotnet tool install --global dotnet-ef --version 8.0.11` (якщо не встановлено), `dotnet ef database update --project ConstructionCompany.Infrastructure --startup-project ConstructionCompany.Console -- construction-ef.db`. Не застосовуйте початкову міграцію до `construction.db` (таблиці вже існують). Початкова міграція написана вручну SQL; автоматичне генерування та застосування EF не перевірено. Для наступних міграцій потрібен знімок моделі, який слід згенерувати EF у чистому проєкті.
- Відкрийте `construction-ef.db` у DB Browser for SQLite, вкладка Browse Data, зробіть скриншоти таблиць і `__EFMigrationsHistory`. Додайте їх та консоль до звіту PDF.
- MongoDB: встановіть сервер або `docker run -d --name construction-mongo -p 127.0.0.1:27017:27017 mongo:7`. Запуск `dotnet run --project ConstructionCompany.Console -- --mongo mongodb://localhost:27017`. У MongoDB Compass відкрийте `construction_company.projects`, зробіть скриншот документів. MongoDB не запускався в середовищі підготовки.
- `dotnet test` — тести попередньої лабораторної; компіляцію та виконання не перевірено (SDK та доступ до NuGet відсутні).

## GitHub
Створіть lab3 від lab2. Завантажте саме вміст цієї папки поверх `ConstructionCompany_Lab1` у lab3, не створюючи вкладену копію. Додайте фактичний PDF і `construction-ef.db` після успішного запуску. Якщо git ігнорує БД: `git add -f ConstructionCompany_Lab1/construction-ef.db`. PR lab3 → lab2, якщо lab2 не злито в master; інакше lab3 → master.
