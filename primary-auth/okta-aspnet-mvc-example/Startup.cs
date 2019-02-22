using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(okta_aspnet_mvc_example.Startup))]

namespace okta_aspnet_mvc_example
{
#pragma warning disable SA1601 // Partial elements should be documented
    public partial class Startup
#pragma warning restore SA1601 // Partial elements should be documented
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
