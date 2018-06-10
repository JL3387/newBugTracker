using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(newBugTracker.Startup))]
namespace newBugTracker
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
