using System.Threading.Tasks;
using System.Web.Mvc;
using Okta.Auth.Sdk;
using Okta.Sdk.Abstractions;
using okta_aspnet_mvc_example.Models;

namespace okta_aspnet_mvc_example.Controllers
{
    public class ManageController : Controller
    {
        private readonly IAuthenticationClient _oktaAuthenticationClient;

        public ManageController(IAuthenticationClient oktaAuthenticationClient)
        {
            _oktaAuthenticationClient = oktaAuthenticationClient;
        }

        // GET: /Manage/ChangePassword
        public ActionResult ChangePassword()
        {
            return View();
        }

        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePasswordAsync(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("ChangePassword", model);
            }

            var changePasswordOptions = new ChangePasswordOptions()
            {
                OldPassword = model.OldPassword,
                NewPassword = model.NewPassword,
                StateToken = Session["stateToken"]?.ToString(),
            };

            try
            {
                var authnResponse = await _oktaAuthenticationClient.ChangePasswordAsync(changePasswordOptions).ConfigureAwait(false);

                if (authnResponse.AuthenticationStatus == AuthenticationStatus.Success)
                {
                    return RedirectToAction("Login", "Account");
                }

                ModelState.AddModelError("Oops! Something went wrong:", authnResponse.AuthenticationStatus);
                return View("ChangePassword", model);
            }
            catch (OktaApiException exception)
            {
                ModelState.AddModelError(string.Empty, exception.ErrorSummary);
                return View("ChangePassword", model);
            }
        }
    }
}
