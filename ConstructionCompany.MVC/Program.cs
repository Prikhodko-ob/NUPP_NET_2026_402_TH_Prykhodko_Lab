using ConstructionCompany.Infrastructure;
using Microsoft.EntityFrameworkCore;
var builder=WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ConstructionCompanyContext>(o=>o.UseSqlite(builder.Configuration.GetConnectionString("Construction") ?? "Data Source=construction-ef.db"));
builder.Services.AddControllersWithViews();
var app=builder.Build();
using(var scope=app.Services.CreateScope()) await scope.ServiceProvider.GetRequiredService<ConstructionCompanyContext>().Database.MigrateAsync();
app.MapControllerRoute("default","{controller=Projects}/{action=Index}/{id?}");
app.Run();
