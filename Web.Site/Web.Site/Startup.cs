using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Web.Site.Startup))]
namespace Web.Site
{
	public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
