using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace okta_aspnet_mvc_example
{
    [ExcludeFromCodeCoverage]
#pragma warning disable SA1649 // File name should match first type name
    public class MvcApplication : System.Web.HttpApplication
#pragma warning restore SA1649 // File name should match first type name
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            UnityConfig.RegisterComponents();

            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.Name;
        }
    }
}
