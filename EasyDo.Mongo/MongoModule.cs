using Autofac;
using EasyDo.Domain;
using EasyDo.Module;
using System.Reflection;

namespace EasyDo.Mongo
{
    [Depend(typeof(KernelModule))]
    public  class MongoModule:EasyDoModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
            IocManager.ContainerBuilder.RegisterGeneric(typeof(MongoRepository<,>)).As(typeof(IRepositoryRoot<,>)).InstancePerDependency();
            //iocManager.ContainerBuilder.RegisterGeneric(typeof(MongoRepositoryOfStringPrimaryKey<>)).As(typeof(IRepository<>)).InstancePerDependency();
        }
    }
}
