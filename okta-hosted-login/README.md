# ASP.NET MVC & Okta-Hosted Login Page Example

This example shows you how to use the `Okta.AspNet` library to log in a user. The user's browser is first redirected to the Okta-hosted login page. After the user authenticates, they are redirected back to your application. ASP.NET MVC automatically populates `HttpContext.User` with the information Okta sends back about the user.


## Prerequisites

Before running this sample, you will need the following:

* An Okta Developer Account, you can sign up for one at https://developer.okta.com/signup/.
* An Okta Application, configured for Web mode. This is done from the Okta Developer Console and you can find instructions [here][OIDC Web Application Setup Instructions].  When following the wizard, use the default properties.  They are designed to work with our sample applications.


## Running This Example

Clone this repo and replace the okta configuration placeholders in the `Web.Config` with your configuration values from the Okta Developer Console. 
You can see all the available configuration options in the [okta-aspnet GitHub](https://github.com/okta/okta-aspnet/blob/master/README.md).

Now start your server and navigate to https://localhost:44314 in your browser. 

If you see a home page that allows you to login, then things are working!  Clicking the **Log in** link will redirect you to the Okta hosted sign-in page.

You can login with the same account that you created when signing up for your Developer Org, or you can use a known username and password from your Okta Directory.

> **Notes:** If you are currently using your Developer Console, you already have a Single Sign-On (SSO) session for your Org.  You will be automatically logged into your application as the same user that is using the Developer Console.  You may want to use an incognito tab to test the flow from a blank slate.

> Because of recent changes in [Set-Cookie behavior (SameSite)](https://web.dev/samesite-cookies-explained) this code will only work properly if it's configured to use https. Check out [Work with SameSite cookies in ASP.NET](https://docs.microsoft.com/en-us/aspnet/samesite/system-web-samesite) for more details.


[OIDC Middleware Library]: https://github.com/okta/okta-aspnet
[Authorization Code Flow]: https://developer.okta.com/authentication-guide/implementing-authentication/auth-code
[OIDC Web Application Setup Instructions]: https://developer.okta.com/authentication-guide/implementing-authentication/auth-code#1-setting-up-your-application