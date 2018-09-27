using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Web.Admin.Startup))]
namespace Web.Admin
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
