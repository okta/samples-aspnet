using Microsoft.Owin;
using Okta.AspNet;
using Owin;
using System.Configuration;

[assembly: OwinStartup(typeof(okta_aspnet_webapi_example.Startup))]

namespace okta_aspnet_webapi_example
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseOktaWebApi(new OktaWebApiOptions()
            {
                OktaDomain = ConfigurationManager.AppSettings["okta:OktaDomain"]
            });
        }
    }
}
