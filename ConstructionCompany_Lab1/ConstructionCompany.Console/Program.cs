using ConstructionCompany.Common;
using System.Globalization;
using System.Threading;
CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
if (args.Contains("--lab1")) { Lab1Demo.Run(); return; }
var service = new CrudServiceAsync<Employee>("employees_lab2.json");
var projects = new CrudServiceAsync<ConstructionProject>("projects_lab2.json");
int created = 0;
object sync = new();
// Parallel.ForEachAsync виконує асинхронні операції паралельно.
await Parallel.ForEachAsync(Enumerable.Range(0, 1000), async (i, ct) =>
{
    Employee employee = i % 2 == 0 ? Builder.CreateNew() : Engineer.CreateNew();
    if (await service.CreateAsync(employee)) lock (sync) { created++; }
});
await Parallel.ForEachAsync(Enumerable.Range(0, 1000), async (_, ct) =>
{ await projects.CreateAsync(ConstructionProject.CreateNew()); });
Console.WriteLine($"Employees created: {created}");
Console.WriteLine($"Projects created: {projects.Count()}");
var all = (await service.ReadAllAsync()).ToArray();
void Stats(string label, IEnumerable<decimal> source)
{
    var values = source.ToArray();
    Console.WriteLine($"{label}: min={values.Min():F2}, max={values.Max():F2}, avg={values.Average():F2}");
}
Stats("Hourly rate", all.Select(x => x.HourlyRate));
Stats("Builder experience", all.OfType<Builder>().Select(x => (decimal)x.ExperienceYears));
Stats("Engineer completed projects", all.OfType<Engineer>().Select(x => (decimal)x.CompletedProjects));
Stats("Project budget", projects.Select(x => x.Budget));
Console.WriteLine($"Page 2 (20 per page): {(await service.ReadAllAsync(2, 20)).Count()}");
var sample = new Builder("CRUD demo", 250, "Painter", 5, "SC-DEMO");
Console.WriteLine($"Create: {await service.CreateAsync(sample)}");
Console.WriteLine($"Read: {(await service.ReadAsync(sample.Id)).Name}");
sample.HourlyRate = 300;
Console.WriteLine($"Update: {await service.UpdateAsync(sample)}; rate={(await service.ReadAsync(sample.Id)).HourlyRate}");
Console.WriteLine($"Remove: {await service.RemoveAsync(sample)}");
// Semaphore: максимум два одночасні учасники; лічильник захищений lock.
using (var semaphore = new Semaphore(2, 2))
{
    int active = 0, peak = 0;
    Parallel.For(0, 8, _ =>
    {
        semaphore.WaitOne();
        try { lock (sync) { active++; peak = Math.Max(peak, active); } Thread.Sleep(15); lock (sync) { active--; } }
        finally { semaphore.Release(); }
    });
    Console.WriteLine($"Semaphore peak: {peak} (limit 2)");
}
// AutoResetEvent: сигнал передається одному потоку очікування.
using (var ready = new AutoResetEvent(false))
{
    var worker = new Thread(() => { ready.WaitOne(); Console.WriteLine("AutoResetEvent: signal received"); });
    worker.Start(); ready.Set(); worker.Join();
}
Console.WriteLine($"Saved employees_lab2.json: {await service.SaveAsync()}");
Console.WriteLine($"Saved projects_lab2.json: {await projects.SaveAsync()}");
