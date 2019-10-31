using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(PRGTrainer.WebAPI.Startup))]

namespace PRGTrainer.WebAPI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
