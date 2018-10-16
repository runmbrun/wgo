using Microsoft.Extensions.DependencyInjection;
using Microsoft.Owin;
using Owin;
using System.Web.Mvc;

[assembly: OwinStartupAttribute(typeof(WGO.Startup))]
namespace WGO
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            // Dependency Injection
            IServiceCollection services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            ConfigureServices(services);
            var resolver = new DefaultDependencyResolver(services.BuildServiceProvider());
            DependencyResolver.SetResolver(resolver);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Add application services.
            services.AddTransient<IRankedCharacterRepository, RankedCharacterRepository>();
        }
    }
}
