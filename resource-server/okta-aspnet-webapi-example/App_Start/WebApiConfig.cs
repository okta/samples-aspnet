using System.Web.Http;

#pragma warning disable SA1300 // Element should begin with upper-case letter
namespace okta_aspnet_webapi_example
#pragma warning restore SA1300 // Element should begin with upper-case letter
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.EnableCors();
            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional });
        }
    }
}
