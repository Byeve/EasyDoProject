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
        private readonly Dictionary<Type, EntityDescribe> _moduelDescribes = new Dictionary<Type, EntityDescribe>();

        private EntityManager()
        {

        }

        public static EntityManager Bulid(IocManager iocManager, List<Assembly> easyDoDependcyAssemblies)
        {
            var entityManager = new EntityManager();
            entityManager.Scan(easyDoDependcyAssemblies);
            //自我注册
            iocManager.ContainerBuilder.RegisterInstance(entityManager).SingleInstance();

            return entityManager;
        }
        /// <summary>
        /// 扫描程序集 实体对象
        /// </summary>
        /// <param name="assemblies"></param>
        private void Scan(List<Assembly> assemblies)
        {
            foreach (var assembly in assemblies)
            {
                var types = assembly.GetExportedTypes().Where(m => m.GetCustomAttribute<EntityAttribute>() != null);
                var enumerable = types as Type[] ?? types.ToArray();
                if (!enumerable.Any())
                {
                    continue;
                }

                foreach (var type in enumerable)
                {
                    if (_moduelDescribes.ContainsKey(type))
                    {
                        continue;
                    }
                    var entityAttribute = type.GetCustomAttribute<EntityAttribute>();
                    _moduelDescribes.Add(type, new EntityDescribe { DbName = entityAttribute.DbName, TableName = entityAttribute.TableName, ReadSecondary = entityAttribute.ReadSecondary });
                }

            }
        }

        public  EntityDescribe GetEntityDescribe(Type type)
        {
            if (_moduelDescribes.ContainsKey(type))
            {
                return _moduelDescribes[type];
            }
            throw new ArgumentException($"非法的实体对象，请检查类型:{type.FullName}是否已经注册");
        }
    }
}
