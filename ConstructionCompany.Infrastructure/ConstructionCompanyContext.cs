using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using ConstructionCompany.Infrastructure.Models;
namespace ConstructionCompany.Infrastructure;
public class ConstructionCompanyContext(DbContextOptions<ConstructionCompanyContext> options) : DbContext(options) {
 public DbSet<EmployeeModel> Employees => Set<EmployeeModel>();
 public DbSet<BuilderModel> Builders => Set<BuilderModel>();
 public DbSet<EngineerModel> Engineers => Set<EngineerModel>();
 public DbSet<ConstructionProjectModel> Projects => Set<ConstructionProjectModel>();
 public DbSet<EmployeeProfileModel> Profiles => Set<EmployeeProfileModel>();
 public DbSet<ProjectTaskModel> Tasks => Set<ProjectTaskModel>();
 protected override void OnModelCreating(ModelBuilder b) {
  b.Entity<EmployeeModel>().UseTptMappingStrategy().ToTable("Employees");
  b.Entity<EmployeeModel>().HasKey(x=>x.Key); b.Entity<EmployeeModel>().HasIndex(x=>x.Id).IsUnique();
  b.Entity<BuilderModel>().ToTable("Builders"); b.Entity<EngineerModel>().ToTable("Engineers");
  b.Entity<ConstructionProjectModel>().ToTable("Projects").HasKey(x=>x.Key); b.Entity<ConstructionProjectModel>().HasIndex(x=>x.Id).IsUnique();
  b.Entity<EmployeeProfileModel>().ToTable("Profiles").HasKey(x=>x.EmployeeKey);
  b.Entity<EmployeeModel>().HasOne(x=>x.Profile).WithOne(x=>x.Employee).HasForeignKey<EmployeeProfileModel>(x=>x.EmployeeKey);
  b.Entity<ProjectTaskModel>().ToTable("Tasks").HasKey(x=>x.Key);
  b.Entity<ConstructionProjectModel>().HasMany(x=>x.Tasks).WithOne(x=>x.Project).HasForeignKey(x=>x.ProjectKey);
  b.Entity<EmployeeModel>().HasMany(x=>x.Projects).WithMany(x=>x.Employees).UsingEntity<Dictionary<string,object>>("EmployeeProjects", r=>r.HasOne<ConstructionProjectModel>().WithMany().HasForeignKey("ProjectKey"), l=>l.HasOne<EmployeeModel>().WithMany().HasForeignKey("EmployeeKey"), j=>{j.ToTable("EmployeeProjects");j.HasKey("EmployeeKey","ProjectKey");});
 }
}
public class ContextFactory : IDesignTimeDbContextFactory<ConstructionCompanyContext> {
 public ConstructionCompanyContext CreateDbContext(string[] args) => new(new DbContextOptionsBuilder<ConstructionCompanyContext>().UseSqlite("Data Source="+(args.Length>0?args[0]:"construction-ef.db")).Options);
}
