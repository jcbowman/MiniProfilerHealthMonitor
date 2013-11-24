using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MiniProfilerHealthMonitor.Startup))]
namespace MiniProfilerHealthMonitor
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
