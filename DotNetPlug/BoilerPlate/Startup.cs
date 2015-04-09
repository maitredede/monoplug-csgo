using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BoilerPlate.Startup))]
namespace BoilerPlate
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
