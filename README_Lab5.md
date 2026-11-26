# Лабораторна №5 — ASP.NET Core Identity
Приходько О. Б., група 402.

Запуск: dotnet run --project ConstructionCompany.REST --urls http://localhost:5080
Swagger: http://localhost:5080/swagger

POST /auth/register, POST /auth/login?useCookies=false (Bearer token), GET /api/projects, POST/PUT /api/projects або /api/builders, DELETE за GUID.
Три ролі: Viewer (читання), Manager (читання, створення, зміна), Admin (усі дії та керування ролями). Читання публічне; нові зареєстровані користувачі не отримують привілейованих ролей. Admin створюється при першому запуску через змінні середовища BootstrapAdmin__Email та BootstrapAdmin__Password; секрети не зберігати в Git. Використовуйте окрему тестову адресу. Існуючий користувач не підвищується автоматично. Admin може призначати Viewer/Manager через PUT /api/users/{id}/role. Після зміни ролі треба увійти заново; старі access tokens можуть залишатися чинними до закінчення їх строку.

Перевірити 200/201/204, 401 без токена, 403 з недостатньою роллю. Для звіту зробити справжні скриншоти без паролів і токенів. Identity bearer tokens не є JWT. Міграція написана вручну; компіляція, застосування міграції та HTTP-запити ще не підтверджені. Хмарне розгортання не виконане.
