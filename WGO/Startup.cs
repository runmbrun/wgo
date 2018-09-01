using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WGO.Startup))]
namespace WGO
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
