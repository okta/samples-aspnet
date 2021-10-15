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
            var isMfaRequiredFlow = (bool)Session["isMfaRequiredFlow"];
            
            if (isMfaRequiredFlow)
            {
                // Assuming Phone: Send code to phone
                var verifyFactorOptions = new VerifySmsFactorOptions
                {
                    StateToken = Session["stateToken"].ToString(),
                    FactorId = Session["factorId"].ToString(),
                };

                _oktaAuthenticationClient.VerifyFactorAsync(verifyFactorOptions).ConfigureAwait(false);
            }

            var viewModel = new VerifyFactorViewModel
            {
                IsMfaRequiredFlow = isMfaRequiredFlow,
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyFactorAsync(VerifyFactorViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("VerifyFactor", model);
            }


            if (model.IsMfaRequiredFlow)
            {
                // Assuming Phone: Send code to phone
                var verifyFactorOptions = new VerifySmsFactorOptions
                {
                    StateToken = Session["stateToken"].ToString(),
                    FactorId = Session["factorId"].ToString(),
                    PassCode = model.Code,
                };

                try
                {
                    var authnResponse = await _oktaAuthenticationClient.VerifyFactorAsync(verifyFactorOptions)
                        .ConfigureAwait(false);

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
            else
            {
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

        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPasswordAsync(ForgotPasswordViewModel model)
        {

            if (!ModelState.IsValid)
            {
                return View("ForgotPassword", model);
            }

            try
            {
                var authResponse = await _oktaAuthenticationClient.ForgotPasswordAsync(new ForgotPasswordOptions
                {
                    FactorType = FactorType.Email,
                    UserName = model.UserName,
                }).ConfigureAwait(false);

                return RedirectToAction("VerifyRecoveryToken", "Manage");
            }
            catch (Exception exception)
            {
                ModelState.AddModelError(string.Empty, exception.Message);
                return View("ForgotPassword", model);
            }
        }


        public ActionResult VerifyRecoveryToken()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyRecoveryTokenAsync(VerifyRecoveryTokenViewModel model)
        {

            if (!ModelState.IsValid)
            {
                return View("VerifyRecoveryToken", model);
            }

            try
            {
                var authResponse = await _oktaAuthenticationClient.VerifyRecoveryTokenAsync(
                    new VerifyRecoveryTokenOptions
                {
                    RecoveryToken = model.RecoveryToken,
                }).ConfigureAwait(false);

                if (authResponse.AuthenticationStatus == AuthenticationStatus.Recovery)
                {

                    var question =  authResponse.Embedded.GetProperty<Resource>("user")?
                        .GetProperty<Resource>("recovery_question")?
                        .GetProperty<string>("question");

                    Session["securityQuestion"] = question;
                    Session["stateToken"] = authResponse.StateToken;

                    return RedirectToAction("VerifySecurityQuestion", "Manage");
                }

                throw new NotImplementedException($"Unhandled Authentication Status {authResponse.AuthenticationStatus}");
            }
            catch (Exception exception)
            {
                ModelState.AddModelError(string.Empty, exception.Message);
                return View("VerifyRecoveryToken", model);
            }
        }

        public ActionResult VerifySecurityQuestion()
        {
            var viewModel = new VerifySecurityQuestionViewModel
            {
                Question = Session["securityQuestion"]?.ToString(),
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifySecurityQuestionAsync(VerifySecurityQuestionViewModel model)
        {

            if (!ModelState.IsValid)
            {
                return View("VerifySecurityQuestion", model);
            }

            try
            {
                var authResponse = await _oktaAuthenticationClient.VerifyFactorAsync(
                    new VerifySecurityQuestionFactorOptions
                    {
                        Answer = model.Answer,
                        StateToken = Session["stateToken"].ToString(),

                    }).ConfigureAwait(false);

                if (authResponse.AuthenticationStatus == AuthenticationStatus.PasswordReset)
                {

                    return RedirectToAction("ChangePassword", "Manage");
                }

                throw new NotImplementedException($"Unhandled Authentication Status {authResponse.AuthenticationStatus}");
            }
            catch (Exception exception)
            {
                ModelState.AddModelError(string.Empty, exception.Message);
                return View("VerifySecurityQuestion", model);
            }
        }
    }
}
