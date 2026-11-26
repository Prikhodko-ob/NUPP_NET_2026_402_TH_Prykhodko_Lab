using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
namespace ConstructionCompany.Infrastructure.Migrations;
[DbContext(typeof(ConstructionCompanyContext))]
[Migration("20261126000100_InitialCreate")]
public sealed class InitialCreate : Migration {
 protected override void Up(MigrationBuilder m) {
  m.Sql("CREATE TABLE Employees (Key INTEGER PRIMARY KEY AUTOINCREMENT, Id TEXT NOT NULL, Name TEXT NOT NULL, HourlyRate TEXT NOT NULL);");
  m.Sql("CREATE UNIQUE INDEX IX_Employees_Id ON Employees(Id);");
  m.Sql("CREATE TABLE Builders (Key INTEGER PRIMARY KEY, Specialty TEXT NOT NULL, ExperienceYears INTEGER NOT NULL, SafetyCertificate TEXT NOT NULL, FOREIGN KEY(Key) REFERENCES Employees(Key) ON DELETE CASCADE);");
  m.Sql("CREATE TABLE Engineers (Key INTEGER PRIMARY KEY, Qualification TEXT NOT NULL, Department TEXT NOT NULL, CompletedProjects INTEGER NOT NULL, FOREIGN KEY(Key) REFERENCES Employees(Key) ON DELETE CASCADE);");
  m.Sql("CREATE TABLE Projects (Key INTEGER PRIMARY KEY AUTOINCREMENT, Id TEXT NOT NULL, Title TEXT NOT NULL, Budget TEXT NOT NULL, Address TEXT NOT NULL, IsCompleted INTEGER NOT NULL);");
  m.Sql("CREATE UNIQUE INDEX IX_Projects_Id ON Projects(Id);");
  m.Sql("CREATE TABLE Profiles (EmployeeKey INTEGER PRIMARY KEY, Phone TEXT NOT NULL, FOREIGN KEY(EmployeeKey) REFERENCES Employees(Key) ON DELETE CASCADE);");
  m.Sql("CREATE TABLE Tasks (Key INTEGER PRIMARY KEY AUTOINCREMENT, ProjectKey INTEGER NOT NULL, Title TEXT NOT NULL, Done INTEGER NOT NULL, FOREIGN KEY(ProjectKey) REFERENCES Projects(Key) ON DELETE CASCADE);");
  m.Sql("CREATE INDEX IX_Tasks_ProjectKey ON Tasks(ProjectKey);");
  m.Sql("CREATE TABLE EmployeeProjects (EmployeeKey INTEGER NOT NULL, ProjectKey INTEGER NOT NULL, PRIMARY KEY(EmployeeKey,ProjectKey), FOREIGN KEY(EmployeeKey) REFERENCES Employees(Key) ON DELETE CASCADE, FOREIGN KEY(ProjectKey) REFERENCES Projects(Key) ON DELETE CASCADE);");
  m.Sql("CREATE INDEX IX_EmployeeProjects_ProjectKey ON EmployeeProjects(ProjectKey);");
 }
 protected override void Down(MigrationBuilder m) {
  m.Sql("DROP TABLE EmployeeProjects;");
  m.Sql("DROP TABLE Tasks;");
  m.Sql("DROP TABLE Profiles;");
  m.Sql("DROP TABLE Builders;");
  m.Sql("DROP TABLE Engineers;");
  m.Sql("DROP TABLE Projects;");
  m.Sql("DROP TABLE Employees;");
 }
}
