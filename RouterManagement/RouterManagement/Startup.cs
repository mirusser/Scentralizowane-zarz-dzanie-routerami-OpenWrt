using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(RouterManagement.Startup))]
namespace RouterManagement
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
