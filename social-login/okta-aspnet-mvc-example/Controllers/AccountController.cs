using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Okta.AspNet;
using System.Web;
using System.Web.Mvc;

namespace okta_aspnet_mvc_example.Controllers
{
    public class AccountController : Controller
    {
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(FormCollection form)
        {
            if (!HttpContext.User.Identity.IsAuthenticated)
            {
                var properties = new AuthenticationProperties();
                properties.Dictionary.Add("sessionToken", form.Get("sessionToken"));
                properties.RedirectUri = "/Home/About";

                HttpContext.GetOwinContext().Authentication.Challenge(properties,
                    OktaDefaults.MvcAuthenticationType);

                return new HttpUnauthorizedResult();
            }

            return RedirectToAction("Index", "Home");
        }

        public ActionResult Login()
        {
            return View();
        }

        public ActionResult LoginWithIdp(string idp)
        {
            if (!HttpContext.User.Identity.IsAuthenticated)
            {
                var properties = new AuthenticationProperties();
                properties.Dictionary.Add("idp", idp);
                properties.RedirectUri = "/Home/About";

                HttpContext.GetOwinContext().Authentication.Challenge(properties,
                    OktaDefaults.MvcAuthenticationType);

                return new HttpUnauthorizedResult();
            }

            return RedirectToAction("Index", "Home");
        }



        [HttpPost]
        public ActionResult Logout()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                HttpContext.GetOwinContext().Authentication.SignOut(
                    CookieAuthenticationDefaults.AuthenticationType,
                    OktaDefaults.MvcAuthenticationType);
            }

            return RedirectToAction("Index", "Home");
        }

        public ActionResult PostLogout()
        {
            return RedirectToAction("Index", "Home");
        }
    }
}