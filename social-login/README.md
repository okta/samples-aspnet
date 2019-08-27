# ASP.NET MVC & Social Sign-In Example

This example shows you how to use the `Okta.AspNet` library to sign in a user via other identity providers such as Google or Facebook. The user's browser is first redirected to the self-hosted sign-in page on your ASP.NET MVC application with multiple login options. Once the user is successfully authenticated, ASP.NET MVC automatically populates `HttpContext.User` with the information that the identity provider sends back about the user.


## Prerequisites

Before running this sample, you will need the following:

* An Okta Developer Account, you can sign up for one at https://developer.okta.com/signup/.
* An Okta Application, configured for Web mode. This is done from the Okta Developer Console and you can find instructions [here][OIDC Web Application Setup Instructions].  When following the wizard, use the default properties.  They are designed to work with our sample applications.
* The desired identity providers configured in Okta. This is done from the Okta Developer Console and you can find instructions [here](https://developer.okta.com/docs/guides/add-an-external-idp/).

**Note:** Make sure to add `http://localhost:8080` as a [Trusted Origin].


## Running This Example

Clone this repo and replace the okta configuration placeholders in the `Web.Config` with your configuration values from the Okta Developer Console. 
You can see all the available configuration options in the [okta-aspnet GitHub](https://github.com/okta/okta-aspnet/blob/master/README.md).
For step-by-step instructions, visit the Okta [ASP.NET MVC Guide]. The guide will walk you through adding Okta sign-in to your ASP.NET application. You can also check out the [ASP.NET MVC quickstart].

Now start your server and navigate to http://localhost:8080 in your browser.

If you see a home page that allows you to sign in, then things are working!  Clicking the **Log in** link will redirect you to the self-hosted sign-in page with multiple sign-in options.

If you want to sign in via Okta, you can sign in with the same account that you created when signing up for your Developer Org, or you can use a known username and password from your Okta Directory.

**Note:** If you are currently using your Developer Console, you already have a Single Sign-On (SSO) session for your Org.  You will be automatically signed in into your application as the same user that is using the Developer Console if you sign in via Okta.  You may want to use an incognito tab to test the flow from a blank slate.

[OIDC Middleware Library]: https://github.com/okta/okta-aspnet
[Authorization Code Flow]: https://developer.okta.com/authentication-guide/implementing-authentication/auth-code
[OIDC Web Application Setup Instructions]: https://developer.okta.com/authentication-guide/implementing-authentication/auth-code#1-setting-up-your-application
[ASP.NET MVC guide]:https://developer.okta.com/docs/guides/sign-into-web-app/aspnet/before-you-begin/
[ASP.NET MVC quickstart]:https://developer.okta.com/quickstart/#/okta-sign-in-page/dotnet/aspnet4
[Trusted Origin]:https://developer.okta.com/docs/api/getting_started/enabling_cors