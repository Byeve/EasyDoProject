using Autofac;
using EasyDo.Dependency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EasyDo.Domain
{
    public class EntityManager
    {
        private  Dictionary<Type, EntityDescribe> moduelDescribes = new Dictionary<Type, EntityDescribe>();

        private EntityManager()
        {

        }

        public static EntityManager Bulid(IocManager IocManager, List<Assembly> EasyDoDependcyAssemblies)
        {
            var entityManager = new EntityManager();
            entityManager.Scan(EasyDoDependcyAssemblies);
            //自我注册
            IocManager.ContainerBuilder.RegisterInstance(entityManager).SingleInstance();

            return entityManager;
        }
        /// <summary>
        /// 扫描程序集 实体对象
        /// </summary>
        /// <param name="Assemblies"></param>
        private void Scan(List<Assembly> Assemblies)
        {
            foreach (var assembly in Assemblies)
            {
                var types = assembly.GetExportedTypes().Where(m => m.GetCustomAttribute<EntityAttribute>() != null);
                if (!types.Any())
                {
                    continue;
                }

                foreach (var type in types)
                {
                    if (moduelDescribes.ContainsKey(type))
                    {
                        continue;
                    }
                    var entityAttribute = type.GetCustomAttribute<EntityAttribute>();
                    moduelDescribes.Add(type, new EntityDescribe { DbName = entityAttribute.DbName, TableName = entityAttribute.TableName, ReadSecondary = entityAttribute.ReadSecondary });
                }

            }
        }

        public  EntityDescribe GetEntityDescribe(Type type)
        {
            if (moduelDescribes.ContainsKey(type))
            {
                return moduelDescribes[type];
            }
            throw new ArgumentException(string.Format("非法的实体对象，请检查类型:{0}是否已经注册", type.FullName));
        }
    }
}
