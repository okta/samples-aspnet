using System.Configuration;
using Microsoft.Owin;
using Okta.AspNet;
using Owin;

[assembly: OwinStartup(typeof(okta_aspnet_webapi_example.Startup))]

#pragma warning disable SA1300 // Element should begin with upper-case letter
namespace okta_aspnet_webapi_example
#pragma warning restore SA1300 // Element should begin with upper-case letter
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseOktaWebApi(new OktaWebApiOptions()
            {
                OktaDomain = ConfigurationManager.AppSettings["okta:OktaDomain"],
            });
        }
    }
}
