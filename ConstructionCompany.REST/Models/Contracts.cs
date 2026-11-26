using System.ComponentModel.DataAnnotations;
namespace ConstructionCompany.REST.Models;
public record ProjectInput([Required, StringLength(200)] string Title, [Range(0, double.MaxValue)] decimal Budget, [Required] string Address, bool IsCompleted);
public record ProjectDto(Guid Id, string Title, decimal Budget, string Address, bool IsCompleted);
public record BuilderInput([Required] string Name, [Range(0, double.MaxValue)] decimal HourlyRate, [Required] string Specialty, [Range(0, 80)] int ExperienceYears, [Required] string SafetyCertificate);
public record BuilderDto(Guid Id, string Name, decimal HourlyRate, string Specialty, int ExperienceYears, string SafetyCertificate);
