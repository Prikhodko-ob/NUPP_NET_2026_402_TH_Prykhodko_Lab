using ConstructionCompany.Common;
namespace ConstructionCompany.Infrastructure.Models;
public interface IDbEntity : IEntity { int Key { get; set; } }
public abstract class EmployeeModel : IDbEntity {
 public int Key { get; set; } public Guid Id { get; set; } = Guid.NewGuid();
 public string Name { get; set; } = ""; public decimal HourlyRate { get; set; }
 public EmployeeProfileModel? Profile { get; set; }
 public ICollection<ConstructionProjectModel> Projects { get; set; } = new List<ConstructionProjectModel>();
}
public class BuilderModel : EmployeeModel {
 public string Specialty { get; set; } = ""; public int ExperienceYears { get; set; } public string SafetyCertificate { get; set; } = "";
}
public class EngineerModel : EmployeeModel {
 public string Qualification { get; set; } = ""; public string Department { get; set; } = ""; public int CompletedProjects { get; set; }
}
public class ConstructionProjectModel : IDbEntity {
 public int Key { get; set; } public Guid Id { get; set; } = Guid.NewGuid(); public string Title { get; set; } = ""; public decimal Budget { get; set; } public string Address { get; set; } = ""; public bool IsCompleted { get; set; }
 public ICollection<EmployeeModel> Employees { get; set; } = new List<EmployeeModel>();
 public ICollection<ProjectTaskModel> Tasks { get; set; } = new List<ProjectTaskModel>();
}
public class EmployeeProfileModel { public int EmployeeKey { get; set; } public string Phone { get; set; } = ""; public EmployeeModel Employee { get; set; } = null!; }
public class ProjectTaskModel { public int Key { get; set; } public int ProjectKey { get; set; } public string Title { get; set; } = ""; public bool Done { get; set; } public ConstructionProjectModel Project { get; set; } = null!; }
