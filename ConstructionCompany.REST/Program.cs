using ConstructionCompany.Common;
using ConstructionCompany.Infrastructure;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ConstructionCompanyContext>(o => o.UseSqlite(builder.Configuration.GetConnectionString("Construction") ?? "Data Source=construction-ef.db"));
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped(typeof(ICrudServiceAsync<>), typeof(DatabaseCrudService<>));
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();
using (var scope = app.Services.CreateScope())
    await scope.ServiceProvider.GetRequiredService<ConstructionCompanyContext>().Database.MigrateAsync();
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.Run();
