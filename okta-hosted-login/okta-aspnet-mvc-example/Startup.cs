using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Services.Description;
using Antlr.Runtime;
using IdentityModel.Client;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Okta.AspNet;
using Owin;
using static IdentityModel.OidcConstants;

[assembly: OwinStartup(typeof(dotnet48_okta_oidc_webapp.Startup))]

namespace dotnet48_okta_oidc_webapp
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Enable TLS 1.2
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;

            /*
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);
 
            //app.UseCookieAuthentication(new CookieAuthenticationOptions()
            //{
            //    LoginPath = new PathString("/Account/Login"),
            //});
 
            app.UseCookieAuthentication(new Microsoft.Owin.Security.Cookies.CookieAuthenticationOptions
            {
                AuthenticationType = "ApplicationCookie",
                LoginPath = new PathString("/Account/Login")
            });
 
            app.UseOktaMvc(new OktaMvcOptions()
            {
                OktaDomain = ConfigurationManager.AppSettings["okta:OktaDomain"],
                ClientId = ConfigurationManager.AppSettings["okta:ClientId"],
                ClientSecret = ConfigurationManager.AppSettings["okta:ClientSecret"],
                AuthorizationServerId = ConfigurationManager.AppSettings["okta:AuthorizationServerId"],
                RedirectUri = ConfigurationManager.AppSettings["okta:RedirectUri"],
                PostLogoutRedirectUri = ConfigurationManager.AppSettings["okta:PostLogoutRedirectUri"],
                Scope = new List<string> { "openid", "profile", "email" },
                //LoginMode = LoginMode.SelfHosted,
            });
            */
            ConfigureAuth(app);


        }
        private void ConfigureAuth(IAppBuilder app)
        {
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = CookieAuthenticationDefaults.AuthenticationType,
            });

            var clientId = ConfigurationManager.AppSettings["okta:ClientId"].ToString();
            var clientSecret = ConfigurationManager.AppSettings["okta:ClientSecret"].ToString();
            var issuer = ConfigurationManager.AppSettings["okta:Issuer"].ToString();
            var redirectUri = ConfigurationManager.AppSettings["okta:RedirectUri"].ToString();
            var postLogoutRedirectUri = ConfigurationManager.AppSettings["okta:PostLogoutRedirectUri"].ToString();
            UserInfoResponse userInfoResponse;

            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
            {
                ClientId = clientId,
                ClientSecret = clientSecret,
                Authority = issuer,
                RedirectUri = redirectUri,
                ResponseType = "id_token token",
                UseTokenLifetime = false,
                Scope = "openid profile email",
                PostLogoutRedirectUri = postLogoutRedirectUri,
                TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "name"
                },
                Notifications = new OpenIdConnectAuthenticationNotifications
                {
                    RedirectToIdentityProvider = context =>
                    {
                        if (context.ProtocolMessage.RequestType == OpenIdConnectRequestType.Logout)
                        {
                            var idToken = context.OwinContext.Authentication.User.Claims.FirstOrDefault(c => c.Type == "id_token")?.Value;
                            context.ProtocolMessage.IdTokenHint = idToken;
                        }

                        return Task.FromResult(true);
                    },
                    AuthorizationCodeReceived = async context =>
                    {
                        // Exchange code for access and ID tokens
                        var tokenClient = new TokenClient(
                            issuer + "/v1/token", clientId, clientSecret);
                        var tokenResponse = await tokenClient.RequestAuthorizationCodeAsync(context.ProtocolMessage.Code, redirectUri);

                        if (tokenResponse.IsError)
                        {
                            throw new Exception(tokenResponse.Error);
                        }

                        var userInfoClient = new UserInfoClient(issuer + "/v1/userinfo");
                        userInfoResponse = await userInfoClient.GetAsync(tokenResponse.AccessToken);

                        var identity = new ClaimsIdentity();
                        identity.AddClaims(userInfoResponse.Claims);
                        identity.AddClaim(new Claim("id_token", tokenResponse.IdentityToken));
                        identity.AddClaim(new Claim("access_token", tokenResponse.AccessToken));
                        if (!string.IsNullOrEmpty(tokenResponse.RefreshToken))
                        {
                            identity.AddClaim(new Claim("refresh_token", tokenResponse.RefreshToken));
                        }

                        var nameClaim = new Claim(ClaimTypes.Name, userInfoResponse.Claims.FirstOrDefault(c => c.Type == "name")?.Value);
                        identity.AddClaim(nameClaim);


                        context.AuthenticationTicket = new AuthenticationTicket(
                            new ClaimsIdentity(identity.Claims, context.AuthenticationTicket.Identity.AuthenticationType),
                            context.AuthenticationTicket.Properties);

                        Console.WriteLine("This is tokenResponse.AccessToken: ");
                        Console.WriteLine(tokenResponse.AccessToken);
                        Trace.WriteLine("This is tokenResponse.AccessToken: ");
                        Trace.WriteLine(tokenResponse.AccessToken);
                    }
                }
            });
            Console.WriteLine("Okta OpenID Connect middleware registered.");
        }

    }
}