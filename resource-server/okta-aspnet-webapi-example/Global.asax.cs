using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

#pragma warning disable SA1300 // Element should begin with upper-case letter
namespace okta_aspnet_webapi_example
#pragma warning restore SA1300 // Element should begin with upper-case letter
{
#pragma warning disable SA1649 // File name should match first type name
    public class WebApiApplication : System.Web.HttpApplication
#pragma warning restore SA1649 // File name should match first type name
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}
