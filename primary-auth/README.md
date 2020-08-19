# ASP.NET MVC & Okta Authentication SDK Example

This example shows you how to use the `Okta.Auth.Sdk` library to:

* Log in a user 
* Change a user's password (only if the password has expired)

The user's browser is first redirected to the self-hosted login page on your ASP.NET MVC application. Once the user is successfully authenticated via Okta, a `ClaimsIdentity` is created with the user's information.
If the user's password has expired, the user will be redirected to a change password form.

## Prerequisites

Before running this sample, you will need the following:

* An Okta Developer Account, you can sign up for one at https://developer.okta.com/signup/.
* An [API token]

## Running This Example

Clone this repo and replace the okta configuration placeholders in the `Web.Config` with your configuration values from the Okta Developer Console. 
You can see all the available configuration options in the [Okta.Auth.Sdk GitHub].

Now start your server and navigate to https://localhost:44314 in your browser.

If you see a home page that allows you to login, then things are working!  Clicking the **Log in** link will redirect you to the self-hosted sign-in page.

You can login with the same account that you created when signing up for your Developer Org, or you can use a known username and password from your Okta Directory.

> **Note:** Because of recent changes in Set-Cookie behavior (SameSite) this code will only work properly if it's configured to use https. Check out [Work with SameSite cookies in ASP.NET](https://docs.microsoft.com/en-us/aspnet/samesite/system-web-samesite) for more details.


[API token]:https://developer.okta.com/docs/api/getting_started/getting_a_token
[Okta.Auth.Sdk GitHub]: https://github.com/okta/okta-auth-dotnet#configuration-reference