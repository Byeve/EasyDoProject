﻿using Autofac;
using Autofac.Extras.DynamicProxy;
using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
namespace EasyDo.Dependency
{
    public class IocManager
    {
        public IContainer Container { get; private set; }
        public ContainerBuilder ContainerBuilder { get; private set; }
        static IocManager()
        {
           
        }

        private IocManager()
        {
            ContainerBuilder = new ContainerBuilder();
        }
        public static IocManager Instance { get; } = new IocManager();

        public void InitContainer()
        {
            Container = ContainerBuilder.Build();
        }

        public T Resolve<T>()
        {
            return Container.Resolve<T>();
        }

        public void RegisterAssemblyByConvention(Assembly assembly)
        {
            //Interceptor
            var interceptors = assembly.GetExportedTypes()
                .Where(m => typeof(IInterceptor).IsAssignableFrom(m) && m.IsClass && !m.IsAbstract);
            foreach (var interceptor in interceptors)
            {
                ContainerBuilder.RegisterType(interceptor);
            }

            //Singleton
            var singletonDependencies = assembly.GetExportedTypes()
                .Where(m => typeof(ISingletonDependency).IsAssignableFrom(m) && m.IsClass && !m.IsAbstract);

            RegisterTypeWithInterceptor(singletonDependencies, DependencyLifeStyle.Singleton);

            //Transient
            var transientDependencies = assembly.GetExportedTypes()
                .Where(m => typeof(ITransientDependency).IsAssignableFrom(m) && m.IsClass && !m.IsAbstract);
            RegisterTypeWithInterceptor(transientDependencies, DependencyLifeStyle.Transient);

        }

        private void RegisterTypeWithInterceptor(IEnumerable<Type> registerTypes, DependencyLifeStyle dependencyLifeStyle)
        {
            var enumerable = registerTypes as Type[] ?? registerTypes.ToArray();
            if (!enumerable.Any()) return;

            foreach (var registerType in enumerable)
            {
                var interceptorAttributes = GetInterceptorAttributes(registerType);

                if (interceptorAttributes.Any())
                {
                    InterceptorRegisterType(dependencyLifeStyle, registerType, interceptorAttributes);
                }
                else
                {
                    NormalRegisterType(dependencyLifeStyle, registerType);
                }
            }

        }
        /// <summary>
        /// 查找 类型里方法是否有拦截Attribute
        /// </summary>
        /// <param name="registerType"></param>
        /// <returns></returns>
        private List<AspectAttribute> GetInterceptorAttributes(Type registerType)
        {
            var methods = registerType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            var interceptorAttributes = new List<AspectAttribute>();
            foreach (var method in methods)
            {
                var aspectAttributes = method.GetCustomAttributes(true).Where(m => m.GetType().IsAssignableTo<AspectAttribute>()).Cast<AspectAttribute>();
                var attributes = aspectAttributes as AspectAttribute[] ?? aspectAttributes.ToArray();
                if (attributes.Any())
                {
                    interceptorAttributes.AddRange(attributes);
                }
            }

            return interceptorAttributes;
        }

        /// <summary>
        /// 包含截器AspectAttribute类型注册 
        /// </summary>
        /// <param name="dependencyLifeStyle"></param>
        /// <param name="registerType"></param>
        /// <param name="interceptorAttributes"></param>
        private void InterceptorRegisterType(DependencyLifeStyle dependencyLifeStyle, Type registerType, IEnumerable<AspectAttribute> interceptorAttributes)
        {
            var interceptorTypes = interceptorAttributes.Select(m => m.InterceptorType).Distinct().ToArray();
            if (dependencyLifeStyle == DependencyLifeStyle.Singleton)
            {
                ContainerBuilder.RegisterType(registerType).AsSelf().EnableClassInterceptors().InterceptedBy(interceptorTypes).SingleInstance();
                ContainerBuilder.RegisterType(registerType).AsImplementedInterfaces().EnableInterfaceInterceptors().InterceptedBy(interceptorTypes).SingleInstance();
            }
            else
            {
                ContainerBuilder.RegisterType(registerType).AsSelf().EnableClassInterceptors().InterceptedBy(interceptorTypes).InstancePerDependency();
                ContainerBuilder.RegisterType(registerType).AsImplementedInterfaces().EnableInterfaceInterceptors().InterceptedBy(interceptorTypes).InstancePerDependency();
            }
        }
       
        /// <summary>
        /// 包含截器AspectAttribute类型注册 
        /// </summary>
        /// <param name="dependencyLifeStyle"></param>
        /// <param name="registerType"></param>
        private void NormalRegisterType(DependencyLifeStyle dependencyLifeStyle, Type registerType)
        {
            if (dependencyLifeStyle == DependencyLifeStyle.Singleton)
            {
                ContainerBuilder.RegisterType(registerType).AsSelf().SingleInstance();
                ContainerBuilder.RegisterType(registerType).AsImplementedInterfaces().SingleInstance();
            }
            else
            {
                ContainerBuilder.RegisterType(registerType).AsSelf().InstancePerDependency();
                ContainerBuilder.RegisterType(registerType).AsSelf().AsImplementedInterfaces().InstancePerDependency();
            }
        }
    }
}
