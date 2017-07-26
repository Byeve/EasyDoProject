using EasyDo.Dependency;
using MongoDB.Driver;

namespace EasyDo.Mongo
{
    public interface IMongoDbContext:ISingletonDependency
    {
        bool EnableSoftDelete<TEntity>();
        IMongoCollection<TEntity> PrimaryMongoCollection<TEntity>() where TEntity : class;
        IMongoCollection<TEntity> SecondaryMongoCollection<TEntity>() where TEntity : class;
    }
}