using System.Collections;
using System.Collections.Concurrent;
using System.Text.Json;
namespace ConstructionCompany.Common;

public interface ICrudServiceAsync<T> : IEnumerable<T>
{
    Task<bool> CreateAsync(T element);
    Task<T> ReadAsync(Guid id);
    Task<IEnumerable<T>> ReadAllAsync();
    Task<IEnumerable<T>> ReadAllAsync(int page, int amount);
    Task<bool> UpdateAsync(T element);
    Task<bool> RemoveAsync(T element);
    Task<bool> SaveAsync();
}

// Сервіс зберігає незалежні JSON-копії, щоб зовнішні зміни об'єктів
// не створювали гонок даних у колекції. Не змінюйте аргумент під час виклику.
public sealed class CrudServiceAsync<T> : ICrudServiceAsync<T> where T : IEntity
{
    private readonly ConcurrentDictionary<Guid, string> items = new();
    private readonly SemaphoreSlim saveGate = new(1, 1);
    public string FilePath { get; }
    public CrudServiceAsync(string filePath) => FilePath = Path.GetFullPath(filePath);
    private static string Encode(T value) => JsonSerializer.Serialize<T>(value);
    private static T Decode(string value) => JsonSerializer.Deserialize<T>(value)!;
    private T[] Snapshot() => items.ToArray().OrderBy(x => x.Key).Select(x => Decode(x.Value)).ToArray();
    public Task<bool> CreateAsync(T element) => Task.FromResult(items.TryAdd(element.Id, Encode(element)));
    public Task<T> ReadAsync(Guid id) => items.TryGetValue(id, out var value)
        ? Task.FromResult(Decode(value)) : Task.FromException<T>(new KeyNotFoundException($"Unknown ID: {id}"));
    public Task<IEnumerable<T>> ReadAllAsync() => Task.FromResult<IEnumerable<T>>(Snapshot());
    // Нумерація сторінок починається з 1; сортування за ID забезпечує стабільний порядок знімка.
    public Task<IEnumerable<T>> ReadAllAsync(int page, int amount)
    {
        if (page < 1) throw new ArgumentOutOfRangeException(nameof(page));
        if (amount < 1) throw new ArgumentOutOfRangeException(nameof(amount));
        long skip = (long)(page - 1) * amount;
        return Task.FromResult<IEnumerable<T>>(skip > int.MaxValue ? Array.Empty<T>() : Snapshot().Skip((int)skip).Take(amount).ToArray());
    }
    public Task<bool> UpdateAsync(T element)
    {
        var id = element.Id;
        var replacement = Encode(element);
        while (items.TryGetValue(id, out var old))
            if (items.TryUpdate(id, replacement, old)) return Task.FromResult(true);
        return Task.FromResult(false);
    }
    public Task<bool> RemoveAsync(T element) => Task.FromResult(items.TryRemove(element.Id, out _));
    public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)Snapshot()).GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    // Асинхронний запис; один запис на екземпляр сервісу, тимчасовий файл захищає попередню версію.
    public async Task<bool> SaveAsync()
    {
        await saveGate.WaitAsync();
        string temp = FilePath + "." + Guid.NewGuid().ToString("N") + ".tmp";
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);
            await using (var stream = new FileStream(temp, FileMode.CreateNew, FileAccess.Write, FileShare.None, 4096, true))
                await JsonSerializer.SerializeAsync(stream, Snapshot(), new JsonSerializerOptions { WriteIndented = true });
            File.Move(temp, FilePath, true);
            return true;
        }
        catch (IOException) { return false; }
        catch (UnauthorizedAccessException) { return false; }
        finally { try { if (File.Exists(temp)) File.Delete(temp); } catch (IOException) { } catch (UnauthorizedAccessException) { } finally { saveGate.Release(); } }
    }
}
