using System.Configuration;
using okta_aspnet_mvc_example.Models;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;
using Okta.Auth.Sdk;
using Okta.Sdk.Abstractions;
using Okta.Sdk.Abstractions.Configuration;

namespace okta_aspnet_mvc_example.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthenticationManager _authenticationManager;

        public AccountController(IAuthenticationManager authenticationManager)
        {
            this._authenticationManager = authenticationManager;
        }

        // GET: Account
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View();

            var authClient = new AuthenticationClient(
                new OktaClientConfiguration
                {
                    OktaDomain = ConfigurationManager.AppSettings["okta:OktaDomain"],
                    Token = ConfigurationManager.AppSettings["okta:Token"],
                });
                        
            var authnOptions = new AuthenticateOptions()
            {
                Username = model.UserName,
                Password = model.Password,
            };

            try
            {
                var authnResponse = await authClient.AuthenticateAsync(authnOptions);

                if (authnResponse.AuthenticationStatus == AuthenticationStatus.Success)
                {
                    var identity = new ClaimsIdentity(new[] {new Claim(ClaimTypes.Name, model.UserName),},
                        DefaultAuthenticationTypes.ApplicationCookie);

                    this._authenticationManager.SignIn(new AuthenticationProperties
                    {
                        IsPersistent = model.RememberMe
                    }, identity);

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", $"Invalid login attempt: {authnResponse.AuthenticationStatus}");
                    return View(model);
                }
            }
            catch (OktaApiException exception)
            {
                ModelState.AddModelError("", $"Invalid login attempt: {exception.ErrorSummary}");
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            this._authenticationManager.SignOut();
            return RedirectToAction("Login", "Account");
        }

    }
}