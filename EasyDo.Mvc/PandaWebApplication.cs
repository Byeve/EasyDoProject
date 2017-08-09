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
        public static Bootstrapper Bootstrapper { get; } = Bootstrapper.Create<TStartupModule>();

        protected virtual void Application_Start(object sender, EventArgs e)
        {
            //MVC
            Bootstrapper.IocManager.ContainerBuilder.RegisterControllers(typeof(TStartupModule).Assembly);

            //WebAPI
            Bootstrapper.IocManager.ContainerBuilder.RegisterApiControllers(typeof(TStartupModule).Assembly);

            Bootstrapper.Initialize();

            DependencyResolver.SetResolver(new AutofacDependencyResolver(Bootstrapper.IocManager.Container));
            GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(Bootstrapper.IocManager.Container);


        }

        protected virtual void Application_End(object sender, EventArgs e)
        {
            Bootstrapper.Dispose();
        }
    }
}
