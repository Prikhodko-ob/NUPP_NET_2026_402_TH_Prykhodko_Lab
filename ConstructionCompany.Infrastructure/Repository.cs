using Microsoft.EntityFrameworkCore;
using ConstructionCompany.Infrastructure.Models;
namespace ConstructionCompany.Infrastructure;
public interface IRepository<T> where T:class {
 Task<T> GetByIdAsync(int id); Task<IEnumerable<T>> GetAllAsync(); Task AddAsync(T entity); Task Update(T entity); Task Delete(T entity);
}
public sealed class Repository<T>(DbContextOptions<ConstructionCompanyContext> options) : IRepository<T> where T:class,IDbEntity {
 public async Task<T> GetByIdAsync(int id) { await using var db=new ConstructionCompanyContext(options); return await db.Set<T>().AsNoTracking().SingleOrDefaultAsync(x=>x.Key==id) ?? throw new KeyNotFoundException(); }
 public async Task<IEnumerable<T>> GetAllAsync() { await using var db=new ConstructionCompanyContext(options); return await db.Set<T>().AsNoTracking().OrderBy(x=>x.Key).ToListAsync(); }
 public async Task AddAsync(T entity) { await using var db=new ConstructionCompanyContext(options); await db.Set<T>().AddAsync(entity); await db.SaveChangesAsync(); }
 public async Task Update(T entity) { await using var db=new ConstructionCompanyContext(options); var old=await db.Set<T>().FindAsync(entity.Key) ?? throw new KeyNotFoundException(); db.Entry(old).CurrentValues.SetValues(entity); await db.SaveChangesAsync(); }
 public async Task Delete(T entity) { await using var db=new ConstructionCompanyContext(options); var old=await db.Set<T>().FindAsync(entity.Key) ?? throw new KeyNotFoundException(); db.Remove(old); await db.SaveChangesAsync(); }
}
