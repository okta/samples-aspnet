using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Okta.Auth.Sdk;
using Okta.Sdk.Abstractions;
using okta_aspnet_mvc_example.Models;

namespace okta_aspnet_mvc_example.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthenticationManager _authenticationManager;
        private readonly IAuthenticationClient _oktaAuthenticationClient;

        public AccountController(IAuthenticationManager authenticationManager, IAuthenticationClient oktaAuthenticationClient)
        {
            _authenticationManager = authenticationManager;
            _oktaAuthenticationClient = oktaAuthenticationClient;
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
        public async Task<ActionResult> LoginAsync(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Login");
            }

            var authnOptions = new AuthenticateOptions()
            {
                Username = model.UserName,
                Password = model.Password,
            };

            try
            {
                var authnResponse = await _oktaAuthenticationClient.AuthenticateAsync(authnOptions).ConfigureAwait(false);

                if (authnResponse.AuthenticationStatus == AuthenticationStatus.Success)
                {
                    var identity = new ClaimsIdentity(
                        new[] { new Claim(ClaimTypes.Name, model.UserName) },
                        DefaultAuthenticationTypes.ApplicationCookie);

                    _authenticationManager.SignIn(new AuthenticationProperties { IsPersistent = model.RememberMe }, identity);

                    return RedirectToAction("Index", "Home");
                }
                else if (authnResponse.AuthenticationStatus == AuthenticationStatus.PasswordExpired)
                {
                    Session["stateToken"] = authnResponse.StateToken;

                    return RedirectToAction("ChangePassword", "Manage");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, $"Invalid login attempt: {authnResponse.AuthenticationStatus}");
                    return View("Login", model);
                }
            }
            catch (OktaApiException exception)
            {
                ModelState.AddModelError(string.Empty, $"Invalid login attempt: {exception.ErrorSummary}");
                return View("Login", model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            _authenticationManager.SignOut();
            return RedirectToAction("Login", "Account");
        }
    }
}
