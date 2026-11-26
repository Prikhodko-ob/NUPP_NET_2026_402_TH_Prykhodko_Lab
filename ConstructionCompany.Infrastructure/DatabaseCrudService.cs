using System.Collections;
using ConstructionCompany.Common;
using ConstructionCompany.Infrastructure.Models;
namespace ConstructionCompany.Infrastructure;
public sealed class DatabaseCrudService<T>(IRepository<T> repository) : ICrudServiceAsync<T> where T:class,IDbEntity {
 private readonly SemaphoreSlim gate = new(1,1);
 private async Task<R> Locked<R>(Func<Task<R>> action) { await gate.WaitAsync(); try {return await action();} finally {gate.Release();} }
 public Task<bool> CreateAsync(T element)=>Locked(async()=>{if((await repository.GetAllAsync()).Any(x=>x.Id==element.Id))return false; await repository.AddAsync(element);return true;});
 public async Task<T> ReadAsync(Guid id)=> (await repository.GetAllAsync()).SingleOrDefault(x=>x.Id==id) ?? throw new KeyNotFoundException();
 public Task<IEnumerable<T>> ReadAllAsync()=>repository.GetAllAsync();
 public async Task<IEnumerable<T>> ReadAllAsync(int page,int amount) {if(page<1||amount<1)throw new ArgumentOutOfRangeException(); long skip=(long)(page-1)*amount; return skip>int.MaxValue?Array.Empty<T>():(await repository.GetAllAsync()).OrderBy(x=>x.Key).Skip((int)skip).Take(amount).ToArray();}
 public Task<bool> UpdateAsync(T element)=>Locked(async()=>{var old=(await repository.GetAllAsync()).SingleOrDefault(x=>x.Id==element.Id);if(old==null)return false;element.Key=old.Key;await repository.Update(element);return true;});
 public Task<bool> RemoveAsync(T element)=>Locked(async()=>{var old=(await repository.GetAllAsync()).SingleOrDefault(x=>x.Id==element.Id);if(old==null)return false;await repository.Delete(old);return true;});
 // Репозиторій зберігає кожну зміну одразу; незбережених змін немає.
 public Task<bool> SaveAsync()=>Task.FromResult(true);
 public IEnumerator<T> GetEnumerator()=>Task.Run(ReadAllAsync).GetAwaiter().GetResult().GetEnumerator();
 IEnumerator IEnumerable.GetEnumerator()=>GetEnumerator();
}
