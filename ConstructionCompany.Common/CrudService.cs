using System.Text.Json;
namespace ConstructionCompany.Common;
public interface ICrudService<T>
{
    void Create(T element);
    T Read(Guid id);
    IEnumerable<T> ReadAll();
    void Update(T element);
    void Remove(T element);
}
// Делегат
public delegate void CrudChangedHandler<T>(string operation, T element);
public class CrudService<T> : ICrudService<T> where T : IEntity
{
    private Dictionary<Guid, T> items = new();
    // Подія
    public event CrudChangedHandler<T>? Changed;
    public void Create(T element) { items.Add(element.Id, element); Changed?.Invoke("CREATE", element); }
    public T Read(Guid id) => items[id];
    public IEnumerable<T> ReadAll() => items.Values.ToArray();
    public void Update(T element)
    {
        if (!items.ContainsKey(element.Id)) throw new KeyNotFoundException("Unknown ID");
        items[element.Id] = element;
        Changed?.Invoke("UPDATE", element);
    }
    public void Remove(T element)
    {
        if (!items.Remove(element.Id)) throw new KeyNotFoundException("Unknown ID");
        Changed?.Invoke("REMOVE", element);
    }
    // Додаткове завдання: серіалізація та десеріалізація JSON
    public void Save(string FilePath) => File.WriteAllText(FilePath,
        JsonSerializer.Serialize(ReadAll(), new JsonSerializerOptions { WriteIndented = true }));
    public void Load(string FilePath)
    {
        var data = JsonSerializer.Deserialize<List<T>>(File.ReadAllText(FilePath))
            ?? throw new InvalidDataException("Invalid JSON data");
        items = data.ToDictionary(x => x.Id);
    }
}
