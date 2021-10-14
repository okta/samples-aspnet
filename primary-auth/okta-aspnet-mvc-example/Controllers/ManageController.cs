using System;
using System.Collections.Generic;
using System.Linq;
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
    public class ManageController : Controller
    {
        private readonly IAuthenticationManager _authenticationManager;
        private readonly IAuthenticationClient _oktaAuthenticationClient;

        public ManageController(IAuthenticationManager authenticationManager, IAuthenticationClient oktaAuthenticationClient)
        {
            _oktaAuthenticationClient = oktaAuthenticationClient;
            _authenticationManager = authenticationManager;
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

        // GET: /Manage/SelectFactor
        public ActionResult SelectFactor()
        {
            var factors = ((List<Factor>)Session["factors"])
                .Select(x => new FactorViewModel { Type = x.Type }).ToList();
            var viewModel = new SelectFactorViewModel
            {
                Factors = factors,
                FactorType = factors.FirstOrDefault().Type,
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SelectFactorAsync(SelectFactorViewModel model)
        {
            var selectedFactor = ((List<Factor>)Session["factors"])?.FirstOrDefault(x => x.Type == model.FactorType);

            if (selectedFactor != null && selectedFactor.Type == "sms")
            {
                return RedirectToAction("EnrollSms", "Manage");
            }

            throw new NotImplementedException("This flow has not been implemented yet.");
        }

        public ActionResult EnrollSms()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EnrollSmsAsync(EnrollSmsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("EnrollSms", model);
            }

            var enrollOptions = new EnrollSmsFactorOptions()
            {
                PhoneNumber = model.PhoneNumber,
                StateToken = Session["stateToken"].ToString(),
            };

            try
            {
                var authnResponse =
                    await _oktaAuthenticationClient.EnrollFactorAsync(enrollOptions).ConfigureAwait(false);

                if (authnResponse.AuthenticationStatus == AuthenticationStatus.MfaEnrollActivate)
                {
                    Session["factorId"] = authnResponse.Embedded.GetProperty<Factor>("factor").Id;
                    return RedirectToAction("VerifyFactor", "Manage");
                }

                throw new NotImplementedException($"Unhandled Authentication Status {authnResponse.AuthenticationStatus}");
            }
            catch (Exception exception)
            {
                ModelState.AddModelError(string.Empty, exception.Message);
                return View("EnrollSms", model);
            }
        }

        public ActionResult VerifyFactor()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyFactorAsync(VerifyFactorViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("VerifyFactor", model);
            }

            var acitvateFactorOptions = new ActivateFactorOptions
            {
                PassCode = model.Code,
                StateToken = Session["stateToken"].ToString(),
                FactorId = Session["factorId"].ToString(),
            };

            try
            {
                var authnResponse =
                    await _oktaAuthenticationClient.ActivateFactorAsync(acitvateFactorOptions).ConfigureAwait(false);

                if (authnResponse.AuthenticationStatus == AuthenticationStatus.MfaEnroll)
                {
                    // check for skip
                    if (authnResponse.Links["skip"] != null)
                    {
                        authnResponse = await _oktaAuthenticationClient.SkipTransactionStateAsync(
                            new TransactionStateOptions
                            {
                                StateToken = Session["stateToken"].ToString(),
                            }).ConfigureAwait(false);
                    }

                }

                if (authnResponse.AuthenticationStatus == AuthenticationStatus.Success)
                {
                    var identity = new ClaimsIdentity(
                        new[] { new Claim(ClaimTypes.Name, Session["userName"].ToString()) },
                        DefaultAuthenticationTypes.ApplicationCookie);

                    _authenticationManager.SignIn(new AuthenticationProperties { IsPersistent = (bool)Session["rememberMe"] }, identity);

                    return RedirectToAction("Index", "Home");
                }

                throw new NotImplementedException($"Unhandled Authentication Status {authnResponse.AuthenticationStatus}");
            }
            catch (Exception exception)
            {
                ModelState.AddModelError(string.Empty, exception.Message);
                return View("VerifyFactor", model);
            }
        }
    }
}
