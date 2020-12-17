using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security;
using Okta.Auth.Sdk;
using Okta.Sdk.Abstractions.Configuration;
using Unity;
using Unity.Injection;
using Unity.Mvc5;

namespace okta_aspnet_mvc_example
{
    public static class UnityConfig
    {
        [ExcludeFromCodeCoverage]
        public static void RegisterComponents()
        {
            var container = new UnityContainer();

            // register all your components with the container here
            // it is NOT necessary to register your controllers

            container.RegisterFactory<IAuthenticationManager>(o => HttpContext.Current.GetOwinContext().Authentication);
            container.RegisterFactory<IAuthenticationClient>(o => new AuthenticationClient(
                new OktaClientConfiguration()
                {
                    OktaDomain = ConfigurationManager.AppSettings["okta:OktaDomain"],
                }));

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}