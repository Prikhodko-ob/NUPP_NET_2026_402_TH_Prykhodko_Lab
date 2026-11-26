using ConstructionCompany.Common;
using ConstructionCompany.Infrastructure;
using ConstructionCompany.Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ConstructionCompanyContext>(o => o.UseSqlite(builder.Configuration.GetConnectionString("Construction") ?? "Data Source=construction-ef.db"));
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped(typeof(ICrudServiceAsync<>), typeof(DatabaseCrudService<>));
builder.Services.AddIdentityApiEndpoints<ApplicationUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ConstructionCompanyContext>();
builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o => {
    o.AddSecurityDefinition("Bearer",new OpenApiSecurityScheme { Type=SecuritySchemeType.Http, Scheme="bearer", Description="Вставте accessToken із /auth/login" });
    o.AddSecurityRequirement(new OpenApiSecurityRequirement { { new OpenApiSecurityScheme { Reference=new OpenApiReference { Type=ReferenceType.SecurityScheme, Id="Bearer" } }, Array.Empty<string>() } });
});
var app = builder.Build();
using (var scope = app.Services.CreateScope()) {
    var sp=scope.ServiceProvider;
    await sp.GetRequiredService<ConstructionCompanyContext>().Database.MigrateAsync();
    var roles=sp.GetRequiredService<RoleManager<IdentityRole>>();
    foreach(var role in new[]{"Viewer","Manager","Admin"}) {
        if(!await roles.RoleExistsAsync(role)) {
            var result=await roles.CreateAsync(new IdentityRole(role));
            if(!result.Succeeded) throw new InvalidOperationException(string.Join("; ",result.Errors.Select(e=>e.Description)));
        }
    }
    var email=builder.Configuration["BootstrapAdmin:Email"];
    var password=builder.Configuration["BootstrapAdmin:Password"];
    if(!string.IsNullOrWhiteSpace(email) && !string.IsNullOrWhiteSpace(password)) {
        var users=sp.GetRequiredService<UserManager<ApplicationUser>>();
        var user=await users.FindByEmailAsync(email);
        if(user==null) {
            user=new ApplicationUser{UserName=email,Email=email};
            var result=await users.CreateAsync(user,password);
            if(!result.Succeeded) throw new InvalidOperationException(string.Join("; ",result.Errors.Select(e=>e.Description)));
            var assigned=await users.AddToRoleAsync(user,"Admin");
            if(!assigned.Succeeded) throw new InvalidOperationException("Cannot assign bootstrap role.");
        }
    }
}
app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();
app.UseAuthorization();
app.MapGroup("/auth").MapIdentityApi<ApplicationUser>();
app.MapControllers();
app.Run();
