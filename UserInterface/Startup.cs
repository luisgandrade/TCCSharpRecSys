using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(UserInterface.Startup))]
namespace UserInterface
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
