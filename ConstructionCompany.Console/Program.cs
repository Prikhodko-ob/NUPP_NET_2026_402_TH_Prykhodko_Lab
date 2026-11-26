using ConstructionCompany.Infrastructure;
using ConstructionCompany.Infrastructure.Models;
using ConstructionCompany.Nosql;
using Microsoft.EntityFrameworkCore;
if(args.Length>0 && args[0]=="--mongo") {await MongoDemo.Run(args.Length>1?args[1]:"mongodb://localhost:27017");return;}
var existing=args.Length>0 && args[0]=="--existing";
var path=existing?(args.Length>1?args[1]:"construction.db"):(args.Length>0?args[0]:"construction-ef.db");
var options=new DbContextOptionsBuilder<ConstructionCompanyContext>().UseSqlite($"Data Source={path}").Options;
await using(var db=new ConstructionCompanyContext(options)) {
 if(!existing) await db.Database.MigrateAsync();
 if(!await db.Employees.AnyAsync()) {
  var builder=new BuilderModel{Name="Ivan",HourlyRate=350,Specialty="Bricklayer",ExperienceYears=5,SafetyCertificate="SC-001",Profile=new(){Phone="0500000001"}};
  var engineer=new EngineerModel{Name="Olena",HourlyRate=600,Qualification="Civil engineer",Department="Design",CompletedProjects=8,Profile=new(){Phone="0500000002"}};
  var project=new ConstructionProjectModel{Title="Residential house",Budget=1500000,Address="Kharkiv",Employees={builder,engineer},Tasks={new(){Title="Foundation"},new(){Title="Design",Done=true}}};db.Projects.Add(project);await db.SaveChangesAsync();
 }
 System.Console.WriteLine($"SQLite: {Path.GetFullPath(path)}");
 System.Console.WriteLine($"Employees: {await db.Employees.CountAsync()}, builders: {await db.Builders.CountAsync()}, engineers: {await db.Engineers.CountAsync()}");
 foreach(var p in await db.Projects.Include(x=>x.Employees).Include(x=>x.Tasks).ToListAsync())System.Console.WriteLine($"{p.Title}: {p.Employees.Count} employees, {p.Tasks.Count} tasks");
}
var service=new DatabaseCrudService<ConstructionProjectModel>(new Repository<ConstructionProjectModel>(options));
var temp=new ConstructionProjectModel{Title="CRUD demo",Budget=100000,Address="Poltava"};
System.Console.WriteLine($"CREATE: {await service.CreateAsync(temp)}");
System.Console.WriteLine($"READ: {(await service.ReadAsync(temp.Id)).Title}");
temp.Budget=125000;System.Console.WriteLine($"UPDATE: {await service.UpdateAsync(temp)}; Budget: {(await service.ReadAsync(temp.Id)).Budget}");
System.Console.WriteLine($"PAGE 1: {(await service.ReadAllAsync(1,2)).Count()} records");
System.Console.WriteLine($"DELETE: {await service.RemoveAsync(temp)}");
System.Console.WriteLine($"SAVE: {await service.SaveAsync()}");
System.Console.WriteLine($"Remaining: {(await service.ReadAllAsync()).Count()}");
