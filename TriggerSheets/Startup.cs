using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TriggerSheets.Startup))]
namespace TriggerSheets
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
