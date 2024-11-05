# ASP.NET MVC 4.x Sample Applications for Okta

This repository contains several sample applications that show you how to integrate various Okta use-cases into your ASP.NET MVC applications.

Please find the sample that fits your use-case from the table below.

| Sample | Description | Use-Case |
|--------|-------------|----------|
| [Okta-Hosted Login](/okta-hosted-login) | An application server that uses the hosted login page on your Okta org, then creates a cookie session for the user in the ASP.NET MVC application. | Traditional web applications with server-side rendered pages. |
| [Resource Server](/resource-server) | This is a sample API resource server that shows you how to authenticate requests with access tokens that have been issued by Okta. | Single-Page applications. |
| [Self-Hosted Login](/self-hosted-login) | An application server that uses a self-hosted login page on your ASP.NET MVC application. | Traditional web applications with server-side rendered pages. |
| [Primary Authentication with Okta.Auth.Sdk](/primary-auth) | An application server that uses a self-hosted login page and `Okta.Auth.Sdk` on your ASP.NET MVC application. | Traditional web applications with server-side rendered pages. |
| [Social Login](/social-login) | An application server that uses a self-hosted login page with multiple login options on your ASP.NET MVC application. | Traditional web applications with server-side rendered pages. |

## Contributing
 
We're happy to accept contributions and PRs! Please see the [contribution guide](CONTRIBUTING.md) to understand how to structure a contribution.

 
## Okta-Hosted-Login with dotnet48 MVC webapp and Okta OIDC
This webapp is able to authenticate with Okta and fetch back the user claims and id_token payload in the Owin context. However, the below issues are present and will not be readily resolved (MSFT Owin framework issue). Therefore, we would **not** recommend using dotnet48 with Okta OIDC. Instead, please upgrade to dotnet core to use Okta OIDC or if that is not possible, use dotnet48 with Okta SAML.
 
## Known Issues
1. The Owin context does not contain the access token. It is null at runtime. This is a known issue with the Owin framework and is not resolved by Microsoft. Therefore, the access token cannot be fetched from the Owin context. If you need to call external APIs, this will be an issue. The proposed solution here is to create an OktaAdapter that will fetch and validate the access token.
2. Global signout does not work. This app's signout will result in a redirect to the global Okta org's configured error page. Instead, you may need to try Okta's Single Logout URL (see link below) or manually clearing the cookies, Okta session, and local session. If using JWT to manage user session, configure a low expiry access token and long refresh token approach.
3. Okta's prescribed solution with app.UseOktaMvc in Startup.cs does not work. It will result in an infinite redirect loop between the webapp and Okta's AuthZ server due to a thrown error. This looks to be an issue with Okta's aspnet library and dotnet48 Owin's middleware. Instead, I modified the service to instead call app.UseOpenIdConnectAuthentication directly - which works. I believe the user claims are pulled from the user-info endpoint though and not the id_token.

## References
- [Okta Single Logout](https://help.okta.com/en-us/content/topics/apps/apps_single_logout.htm)