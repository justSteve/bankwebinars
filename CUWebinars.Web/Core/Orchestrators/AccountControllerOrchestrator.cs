using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BrockAllen.MembershipReboot;
using CUWebinars.Business.AccountService;
using CUWebinars.Business.Constants;
using CUWebinars.Business.Services;
using CUWebinars.Web.Helpers;
using CUWebinars.Web.Models;
using CUWebinars.Web.Services;
using CUWebinars.Web.ViewModel;
using Ninject.Extensions.Logging;

namespace CUWebinars.Web.Core.Orchestrators
{
    public class AccountControllerOrchestrator : IAccountControllerOrchestrator
    {
        GlobalConfig _globals = GlobalConfig.GlobalConfigSingleton;
        public HttpRequest Request { get; set; }
        private readonly ILogger _logger;
        private readonly IMembershipService _membershipService;
        private readonly IStateService _stateService;
        private readonly IOrderManagementService _orderManagementService;
        private bool _disposed;

        public AccountControllerOrchestrator(ILogger logger,
            IMembershipService membershipService,
            IOrderManagementService orderManagementService,
            IStateService stateService,
            HttpRequest request)

        {
            Request = request;
            _logger = logger;
            _membershipService = membershipService;
            _orderManagementService = orderManagementService;
            _stateService = stateService;
            
        }

        public bool SignUserIn(SignInModel model, out string userMustVerify)
        {
            if (_membershipService.LogInUser(_globals.Tenant, model.Email, model.Password, model.RememberMe, out userMustVerify))
            {
                if (!ReferenceEquals(Request.ApplicationPath, null) && !ReferenceEquals(Request.Url, null))
                {
                    var retUrl = model.ReturnUrl.Replace(
                            string.Format(@"{0}://{1}{2}/",
                            Request.Url.Scheme,
                            Request.Url.Authority,
                            Request.ApplicationPath.TrimEnd('/')),
                            string.Empty
                            );

                    _logger.Info("Account.SignIn Post Success. Session={0}, Redirecting to: {1}", AppHelper.GetUserAuditInfo(), retUrl);
                }

                return true;
            }

            return false;
        }

        public bool UserConfirmed(CreateUserConfirmedViewModel model)
        {
            if (_membershipService.VerifyUserByEmail(_globals.Tenant, model.Email))
            {
                _stateService.SetValue(DomainConstants.UserCreatedViaNewOrder, true);
                _membershipService.ResetPassword(_globals.Tenant, model.Email);

                var verificationKey = _stateService.GetValue<string>(DomainConstants.VerificationKey);

                _stateService.ClearValue(DomainConstants.UserCreatedViaNewOrder);

                _membershipService.ChangePasswordFromResetKey(verificationKey, model.NewPassword);

                _stateService.ClearValue(DomainConstants.VerificationKey);

                _membershipService.LogInUser(_globals.Tenant, model.Email, model.NewPassword, true);

                _logger.Info("Account.Confirmed POST. Session={0}", AppHelper.GetUserAuditInfo());
                
                return true;
            }

            return false;
        }

        public LoginModel LogUserIn(string returnUrl)
        {
            var urlHelper = new UrlHelper(Request.RequestContext); 

            var loginModel = new LoginModel
            {
                SignIn = new SignInModel(),
                ResetPassword = new ResetPasswordModel(),
                Register = new RegisterViewModel
                {
                    RegisterFields = new RegisterModel
                    {
                        AccountDetailsTitle = WebUiConstants.Register,
                        BillingAddress = new AddressModel { TypeOfAddress = AddressType.Billing },
                        ShippingAddress = new AddressModel { TypeOfAddress = AddressType.Shipping }
                    }
                }
            };

            //So that the user can be referred back to where they were when they click logon
            if (string.IsNullOrEmpty(returnUrl) && Request.UrlReferrer != null)
            {
                returnUrl = HttpUtility.UrlDecode(Request.UrlReferrer.PathAndQuery);
                _logger.Info("Login|returnURL was: " + returnUrl + " Session=" + AppHelper.GetUserAuditInfo());
            }

            if (urlHelper.IsLocalUrl(returnUrl) && !string.IsNullOrEmpty(returnUrl))
            {
                if (returnUrl.Contains("PasswordResetConfirm") || returnUrl.Equals("Account/Signin") || returnUrl.Equals("Account/Login"))
                    returnUrl = "/Account/MyWebinars";

                loginModel.ReturnUrl = returnUrl;
                loginModel.SignIn.ReturnUrl = returnUrl;
            }

            loginModel.ActiveTab = "login";
            
            return loginModel;
        }

        public CreateUserConfirmedViewModel ConfirmUser(string email, string surname)
        {
            var changeEmailFromKeyInputModel = new CreateUserConfirmedViewModel
            {
                Email = email,
                OldPassword = surname,
                NewPassword = string.Empty,
                ConfirmPassword = string.Empty,
                ScreenMessage = string.Empty,
                UserIsLoggedIn = Request.IsAuthenticated
            };

            var userAccount = _membershipService.GetUserAccountByEmail(_globals.Tenant, email);

            if (ReferenceEquals(userAccount, null)) throw new Exception("User does not exist in system");

            bool hasAlreadyVerifiedAccount = !userAccount.HasClaim(ClaimTypes.HasNotVerified);

            if (hasAlreadyVerifiedAccount)
            {
                changeEmailFromKeyInputModel.ScreenMessage = "You've already verified your account with us. Thank you.";
                return changeEmailFromKeyInputModel;
            }

            if (userAccount.HasClaim(ClaimTypes.HasNotVerified, ClaimValues.ManualRegistration))
            {
                changeEmailFromKeyInputModel.ScreenMessage = "Thank you for verifying your account with us.";
                return changeEmailFromKeyInputModel;
            }

            return changeEmailFromKeyInputModel;
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                var logger = _logger as IDisposable;
                if (logger != null)
                    logger.Dispose();

                _orderManagementService.Dispose();
                _membershipService.Dispose();

                _disposed = true;
            }
        }
    }
}