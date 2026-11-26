using System.Text.Json.Serialization;
namespace ConstructionCompany.Common;
public interface IEntity { Guid Id { get; set; } }
[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(Builder), "builder")]
[JsonDerivedType(typeof(Engineer), "engineer")]
public abstract class Employee : IEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = "";
    public decimal HourlyRate { get; set; }
    // Статичне поле
    public static readonly string CompanyName;
    // Статичний конструктор
    static Employee() { CompanyName = "ConstructionCompany"; }
    // Конструктор
    protected Employee() { }
    protected Employee(string name, decimal rate) { Name = name; HourlyRate = rate; }
    // Метод
    public decimal CalculatePay(int hours) => HourlyRate * hours;
    // Статичний метод
    public static bool IsValidRate(decimal rate) => rate > 0;
    public abstract string Describe();
    public static Employee CreateNew() => Random.Shared.Next(2) == 0 ? Builder.CreateNew() : Engineer.CreateNew();
}
public class Builder : Employee
{
    public string Specialty { get; set; } = "";
    public int ExperienceYears { get; set; }
    public string SafetyCertificate { get; set; } = "";
    // Конструктори
    public Builder() { }
    public Builder(string name, decimal rate, string specialty, int years, string certificate)
        : base(name, rate) { Specialty = specialty; ExperienceYears = years; SafetyCertificate = certificate; }
    public new static Builder CreateNew() => new($"Builder-{Guid.NewGuid():N}", Random.Shared.Next(200, 601), "Bricklayer", Random.Shared.Next(1, 31), "SC-" + Guid.NewGuid().ToString("N"));
    public override string Describe() => $"Builder: {Name}, {Specialty}, {ExperienceYears} years";
}
public class Engineer : Employee
{
    public string Qualification { get; set; } = "";
    public string Department { get; set; } = "";
    public int CompletedProjects { get; set; }
    // Конструктори
    public Engineer() { }
    public Engineer(string name, decimal rate, string qualification, string department, int projects)
        : base(name, rate) { Qualification = qualification; Department = department; CompletedProjects = projects; }
    public new static Engineer CreateNew() => new($"Engineer-{Guid.NewGuid():N}", Random.Shared.Next(350, 901), "Civil engineer", "Design", Random.Shared.Next(0, 51));
    public override string Describe() => $"Engineer: {Name}, {Qualification}, {Department}";
}
public class ConstructionProject : IEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = "";
    public decimal Budget { get; set; }
    public string Address { get; set; } = "";
    public bool IsCompleted { get; set; }
    // Метод
    public static ConstructionProject CreateNew() => new() { Title = $"Project-{Guid.NewGuid():N}", Budget = Random.Shared.Next(100000, 5000001), Address = "Kharkiv" };
    public void Complete() => IsCompleted = true;
}
public static class EmployeeExtensions
{
    // Метод розширення
    public static string PaySummary(this Employee employee, int hours) =>
        $"{employee.Name}: {employee.CalculatePay(hours):0.00} UAH";
}
