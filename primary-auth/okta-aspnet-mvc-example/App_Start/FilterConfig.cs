using System.Web;
using System.Web.Mvc;

namespace okta_aspnet_mvc_example
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
