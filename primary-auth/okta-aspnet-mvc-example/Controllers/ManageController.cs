using System.Configuration;
using System.Threading.Tasks;
using System.Web.Mvc;
using okta_aspnet_mvc_example.Models;
using Okta.Auth.Sdk;
using Okta.Sdk.Abstractions;
using Okta.Sdk.Abstractions.Configuration;

namespace okta_aspnet_mvc_example.Controllers
{
    public class ManageController : Controller
    {
        private readonly IAuthenticationClient _oktaAuthenticationClient;

        public ManageController(IAuthenticationClient oktaAuthenticationClient)
        {
            _oktaAuthenticationClient = oktaAuthenticationClient;
        }

        // GET: Manage
        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /Manage/ChangePassword
        public ActionResult ChangePassword()
        {
            return View();
        }


        //
        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            
            var changePasswordOptions = new ChangePasswordOptions()
            {
                OldPassword = model.OldPassword,
                NewPassword = model.NewPassword,
                StateToken = Session["stateToken"]?.ToString(),
            };

            try
            {
                var authnResponse = await _oktaAuthenticationClient.ChangePasswordAsync(changePasswordOptions);

                if (authnResponse.AuthenticationStatus == AuthenticationStatus.Success)
                {
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    ModelState.AddModelError("Oops! Something went wrong:", authnResponse.AuthenticationStatus);
                    return View(model);
                }
            }
            catch (OktaApiException exception)
            {
                ModelState.AddModelError("", exception.ErrorSummary);
                return View(model);
            }
        }
    }
}