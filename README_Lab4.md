# Лабораторна №4 — ASP.NET Core REST API
Приходько О. Б., група 402.

REST: dotnet run --project ConstructionCompany.REST --urls http://localhost:5080
Swagger: http://localhost:5080/swagger
MVC: dotnet run --project ConstructionCompany.MVC --urls http://localhost:5081

Ресурси /api/projects та /api/builders: GET (200), POST (201), PUT (204), DELETE (204), невідомий GUID (404). Пагінація page/amount, помилкові параметри (400). DTO відокремлені від EF-моделей. DI використовує репозиторій та CRUD-сервіс lab3.

Потрібно виконати запити, зберегти реальні результати у PDF. MVC реалізований вручну на Razor, не генератором scaffolding. Компіляцію та запуск не підтверджено.
