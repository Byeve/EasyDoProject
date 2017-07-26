using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using EasyDo.Module;
using System;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace EasyDo.Mvc
{
    public abstract class EasyDoWebApplication<TStartupModule> : HttpApplication
       where TStartupModule : EasyDoModule
    {
        public static Bootstrapper _bootstrapper { get; } = Bootstrapper.Create<TStartupModule>();

        protected virtual void Application_Start(object sender, EventArgs e)
        {
            //MVC
            _bootstrapper.IocManager.ContainerBuilder.RegisterControllers(typeof(TStartupModule).Assembly);

            //WebAPI
            _bootstrapper.IocManager.ContainerBuilder.RegisterApiControllers(typeof(TStartupModule).Assembly);

            _bootstrapper.Initialize();

            DependencyResolver.SetResolver(new AutofacDependencyResolver(_bootstrapper.IocManager.Container));
            GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(_bootstrapper.IocManager.Container);


        }

        protected virtual void Application_End(object sender, EventArgs e)
        {
            _bootstrapper.Dispose();
        }
    }
}
