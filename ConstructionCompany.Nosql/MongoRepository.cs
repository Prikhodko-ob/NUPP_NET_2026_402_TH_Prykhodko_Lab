using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using ConstructionCompany.Infrastructure;
using ConstructionCompany.Infrastructure.Models;
namespace ConstructionCompany.Nosql;
// Для документної моделі використовується ConstructionProjectModel без циклічних навігацій.
public sealed class MongoRepository<T> : IRepository<T> where T:class,IDbEntity {
 private readonly IMongoCollection<T> items;
 private readonly IMongoCollection<BsonDocument> counters;
 public MongoRepository(IMongoDatabase db,string collection) {items=db.GetCollection<T>(collection);counters=db.GetCollection<BsonDocument>(collection+"_counters");items.Indexes.CreateOne(new CreateIndexModel<T>(Builders<T>.IndexKeys.Ascending(x=>x.Key),new CreateIndexOptions{Unique=true}));
 // MongoDB автоматично створює унікальний індекс _id для Id.
 }
 public async Task<T> GetByIdAsync(int id)=>await items.Find(x=>x.Key==id).FirstOrDefaultAsync() ?? throw new KeyNotFoundException();
 public async Task<IEnumerable<T>> GetAllAsync()=>await items.Find(Builders<T>.Filter.Empty).SortBy(x=>x.Key).ToListAsync();
 public async Task AddAsync(T entity) {var counter=await counters.FindOneAndUpdateAsync(Builders<BsonDocument>.Filter.Eq("_id","key"),Builders<BsonDocument>.Update.Inc("value",1),new FindOneAndUpdateOptions<BsonDocument>{IsUpsert=true,ReturnDocument=ReturnDocument.After});entity.Key=counter["value"].ToInt32();await items.InsertOneAsync(entity);}
 public async Task Update(T entity) {var result=await items.ReplaceOneAsync(x=>x.Key==entity.Key,entity);if(result.MatchedCount==0)throw new KeyNotFoundException();}
 public async Task Delete(T entity) {var result=await items.DeleteOneAsync(x=>x.Key==entity.Key);if(result.DeletedCount==0)throw new KeyNotFoundException();}
}
public static class MongoDemo {
 public static async Task Run(string connection) {
  BsonDefaults.GuidRepresentationMode = GuidRepresentationMode.V3;
  BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
  var repo=new MongoRepository<ConstructionProjectModel>(new MongoClient(connection).GetDatabase("construction_company"),"projects");
  var p=new ConstructionProjectModel{Title="Mongo demo",Budget=50000,Address="Kharkiv"}; await repo.AddAsync(p);p.Budget=55000;await repo.Update(p);System.Console.WriteLine((await repo.GetByIdAsync(p.Key)).ToJson());
  var temporary=new ConstructionProjectModel{Title="Temporary"};await repo.AddAsync(temporary);await repo.Delete(temporary);
  foreach(var item in await repo.GetAllAsync())System.Console.WriteLine(item.ToJson());
 }
}
