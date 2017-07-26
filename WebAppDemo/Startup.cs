using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WebAppDemo.Startup))]
namespace WebAppDemo
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
