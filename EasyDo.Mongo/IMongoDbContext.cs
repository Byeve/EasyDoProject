using EasyDo.Dependency;
using MongoDB.Driver;

namespace EasyDo.Mongo
{
    public interface IMongoDbContext:ISingletonDependency
    {
        bool EnableSoftDelete<TEntity>();
        IMongoCollection<TEntity> MasterMongoCollection<TEntity>() where TEntity : class;
        IMongoCollection<TEntity> SlaveMongoCollection<TEntity>() where TEntity : class;
    }
}