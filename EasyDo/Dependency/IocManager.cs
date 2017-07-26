using Autofac;
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
        private static readonly IocManager instance = new IocManager();
        public IContainer Container { get; private set; }
        public ContainerBuilder ContainerBuilder { get; private set; }
        static IocManager()
        {
           
        }

        private IocManager()
        {
            ContainerBuilder = new ContainerBuilder();
        }
        public static IocManager Instance
        {
            get
            {
                return instance;
            }
        }

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
            if (!registerTypes.Any()) return;

            foreach (var registerType in registerTypes)
            {
                var interceptorAttributes = registerType.GetCustomAttributes<AspectAttribute>(true);
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
