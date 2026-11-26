using ConstructionCompany.Common;
using System.Text.Json;
using Xunit;
namespace ConstructionCompany.Tests;
public class CrudTests : IDisposable
{
    private readonly string path = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".json");
    private CrudServiceAsync<Employee> NewService() => new(path);
    [Fact] public async Task CreateReadAndDuplicate()
    { var s = NewService(); var b = Builder.CreateNew(); Assert.True(await s.CreateAsync(b)); Assert.False(await s.CreateAsync(b)); Assert.Equal(b.Id, (await s.ReadAsync(b.Id)).Id); }
    [Fact] public async Task UpdateAndRemove()
    { var s = NewService(); var b = Builder.CreateNew(); Assert.False(await s.UpdateAsync(b)); await s.CreateAsync(b); b.HourlyRate = 999; Assert.True(await s.UpdateAsync(b)); Assert.Equal(999m, (await s.ReadAsync(b.Id)).HourlyRate); Assert.True(await s.RemoveAsync(b)); Assert.False(await s.RemoveAsync(b)); await Assert.ThrowsAsync<KeyNotFoundException>(() => s.ReadAsync(b.Id)); }
    [Fact] public async Task ParallelCreationAndPages()
    { var s = NewService(); await Parallel.ForEachAsync(Enumerable.Range(0, 1000), async (_, ct) => { Assert.True(await s.CreateAsync(Builder.CreateNew())); }); Assert.Equal(1000, s.Count()); var a = await s.ReadAllAsync(1, 20); var b = await s.ReadAllAsync(2, 20); Assert.Equal(20, a.Count()); Assert.Empty(a.Select(x => x.Id).Intersect(b.Select(x => x.Id))); Assert.Empty(await s.ReadAllAsync(51, 20)); Assert.Throws<ArgumentOutOfRangeException>(() => { _ = s.ReadAllAsync(0, 20); }); }
    [Fact] public async Task ConcurrentSavePreservesTypes()
    { var s = NewService(); await s.CreateAsync(Builder.CreateNew()); await s.CreateAsync(Engineer.CreateNew()); Assert.All(await Task.WhenAll(s.SaveAsync(), s.SaveAsync()), x => Assert.True(x)); var data = JsonSerializer.Deserialize<Employee[]>(await File.ReadAllTextAsync(path))!; Assert.Contains(data, x => x is Builder); Assert.Contains(data, x => x is Engineer); Assert.Equal(2, data.Length); }
    [Fact] public async Task SnapshotDoesNotExposeMutableState()
    { var s = NewService(); var b = Builder.CreateNew(); await s.CreateAsync(b); var original = b.HourlyRate; b.HourlyRate = -1; var copy = await s.ReadAsync(b.Id); copy.HourlyRate = -2; Assert.Equal(original, (await s.ReadAsync(b.Id)).HourlyRate); }
    public void Dispose() { if (File.Exists(path)) File.Delete(path); }
}
