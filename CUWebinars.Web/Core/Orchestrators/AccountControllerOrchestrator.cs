using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using BrockAllen.MembershipReboot;
using CUWebinars.Business.AccountService;
using CUWebinars.Business.Constants;
using CUWebinars.Business.Models;
using CUWebinars.Business.Services;
using CUWebinars.Web.Helpers;
using CUWebinars.Web.Membership;
using CUWebinars.Web.Models;
using CUWebinars.Web.Services;
using CUWebinars.Web.ViewModel;
using Ninject.Extensions.Logging;
using ClaimsExtensions = CUWebinars.Web.Helpers.ClaimsExtensions;
using ClaimTypes = CUWebinars.Business.Constants.ClaimTypes;

namespace CUWebinars.Web.Core.Orchestrators
{
    public class AccountControllerOrchestrator : IAccountControllerOrchestrator
    {
        private readonly GlobalConfig _globals = GlobalConfig.GlobalConfigSingleton;
        public HttpRequestWrapper Request { get; set; }
        private readonly ILogger _logger;
        private readonly IMembershipService _membershipService;
        private readonly IStateService _stateService;
        private readonly IOrderManagementService _orderManagementService;
        private bool _disposed;

        public 
            AccountControllerOrchestrator(ILogger logger,
            IMembershipService membershipService,
            IOrderManagementService orderManagementService,
            IStateService stateService,
            HttpRequestWrapper request)

        {
            Request = request;
            _logger = logger;
            _membershipService = membershipService;
            _orderManagementService = orderManagementService;
            _stateService = stateService;
            
        }

        public bool LogUserIn(SignInModel signInModel)
        {
            return _membershipService.LogInUser(_globals.Tenant, signInModel.Email, signInModel.Password, signInModel.RememberMe);
        }

        public void LogUserOut()
        {
            _membershipService.LogOutUser();

            if (_stateService.HasValue(WebUiConstants.AdminUserEmail))
            {
                var adminUserAccount = _membershipService.GetUserAccountByEmail(_globals.Tenant, _stateService.GetValue<string>(WebUiConstants.AdminUserEmail));

                _membershipService.SignIn(adminUserAccount, false);

                _stateService.ClearValue(WebUiConstants.AdminUserEmail);
            }
        }

        public int? ParseZip(string zip)
        {
            int zipAsNumber;

            if (!int.TryParse(zip, out zipAsNumber))
            {
                var zipToParse = zip.Split('-').FirstOrDefault();

                if (zipToParse == null || !int.TryParse(zipToParse, out zipAsNumber))
                {
                    return null;
                }
            }

            return zipAsNumber;
        }

        public void RegisterAndLogInUser(RegisterViewModel model)
        {
            var email = model.RegisterFields.Email.Trim();
            var firstName = model.RegisterFields.FirstName.Trim();
            var lastName = model.RegisterFields.LastName.Trim();
            var city = model.RegisterFields.BillingAddress.City.Trim();
            var state = model.RegisterFields.BillingAddress.State.Trim();
            var zip = model.RegisterFields.BillingAddress.Zip.Trim();

            var myInstitution = _membershipService.ProcessInstitutionForUser(
                model.RegisterFields.Institution.Trim(),
                email,
                city,
                state,
                "N",
                "New",
                zip);

            IList<Address> addresses = new List<Address>
            {
                new Address
                {
                    AddressType =
                        Enum.GetName(typeof (AddressType), model.RegisterFields.BillingAddress.TypeOfAddress),
                    City = model.RegisterFields.BillingAddress.City.Trim(),
                    Country = model.RegisterFields.BillingAddress.Country.Trim(),
                    Name = firstName + ' ' + lastName,
                    Phone = model.RegisterFields.BillingAddress.Phone.Trim(),
                    State = state,
                    StreetAddress = model.RegisterFields.BillingAddress.StreetAddress.Trim(),
                    StreetAddress2 =
                        model.RegisterFields.BillingAddress.StreetAddress2 == null
                            ? model.RegisterFields.BillingAddress.StreetAddress2
                            : model.RegisterFields.BillingAddress.StreetAddress2.Trim(),
                    Zip = zip
                },
                new Address
                {
                    AddressType =
                        Enum.GetName(typeof (AddressType), model.RegisterFields.ShippingAddress.TypeOfAddress),
                    City = model.RegisterFields.ShippingAddress.City.Trim(),
                    Country = model.RegisterFields.ShippingAddress.Country.Trim(),
                    Name = firstName + ' ' + lastName,
                    Phone = model.RegisterFields.ShippingAddress.Phone.Trim(),
                    State = model.RegisterFields.ShippingAddress.State.Trim(),
                    StreetAddress = model.RegisterFields.ShippingAddress.StreetAddress.Trim(),
                    StreetAddress2 =
                        model.RegisterFields.ShippingAddress.StreetAddress2 == null
                            ? model.RegisterFields.ShippingAddress.StreetAddress2
                            : model.RegisterFields.ShippingAddress.StreetAddress2.Trim(),
                    Zip = model.RegisterFields.ShippingAddress.Zip.Trim()
                }
            };

            var webUser = _membershipService.CreateWebUser(_globals.Tenant,
                firstName
                , lastName
                , model.RegisterFields.Password
                , email
                , USTimeZone.Central
                , UserType.Customer
                , myInstitution.idInstitution
                , addresses
                ,
                model.RegisterFields.Title == null
                    ? model.RegisterFields.Title
                    : model.RegisterFields.Title.Trim()
                , null
                , DomainConstants.Active
                );

            if (!_stateService.HasValue(Constants.CurrentUser))
                _stateService.SetValue(Constants.CurrentUser, webUser);

            var userAccount = _membershipService.CreateUser(
                _globals.Tenant,
                firstName,
                lastName,
                string.Empty,
                //  Pass empty string for username because MembershipReboot assigns the email to the username field where emailIsUsername is true
                model.RegisterFields.Password,
                email);

            _membershipService.AddAccountTypeNotVerifiedClaim(userAccount, ClaimValues.ManualRegistration);

            Debug.Assert(_stateService.HasValue(DomainConstants.VerificationKey),
                "There's no reason session should not have a value for the VerificationKey at this point ");

            var verificationKey = _stateService.GetValue<string>(DomainConstants.VerificationKey);
            _stateService.ClearValue(DomainConstants.VerificationKey);

            userAccount = _membershipService.VerifyEmailFromKey(
                verificationKey,
                model.RegisterFields.Password
                );

            _membershipService.LogInUser(_globals.Tenant, model.RegisterFields.Email,
                model.RegisterFields.Password, true); // log the user in.            

        }

        public void ResetPassword(string tenant, string email)
        {
            _membershipService.ResetPassword(tenant, email);
        }

        public bool SignUserIn(SignInModel model, out string userMustVerify)
        {
            if (_membershipService.LogInUser(_globals.Tenant, model.Email, model.Password, model.RememberMe, out userMustVerify, model.SigninAfterCheckout))
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

        public void UpdateNameTitle(string firstName, string lastName, string email, string title)
        {
            _membershipService.UpdateNameTitle(firstName, lastName, email, title);
        }

        public void UpdateUserDetails(ManageModel model)
        {
            var updateFields = model.RegisterFields;

            var billingAddressFields = updateFields.BillingAddress;
            var shippingAddressFields = updateFields.ShippingAddress;

            var billingAddress = new Address
            {
                Name = model.RegisterFields.FirstName + ' ' + model.RegisterFields.LastName,
                StreetAddress = billingAddressFields.StreetAddress.Trim(),
                StreetAddress2 = billingAddressFields.StreetAddress2 == null ? billingAddressFields.StreetAddress2 : billingAddressFields.StreetAddress2.Trim(),
                State = billingAddressFields.State.Trim(),
                City = billingAddressFields.City.Trim(),
                Country = billingAddressFields.Country.Trim(),
                Zip = billingAddressFields.Zip.Trim(),
                Phone = billingAddressFields.Phone.Trim(),
                AddressType = Enum.GetName(typeof(AddressType), billingAddressFields.TypeOfAddress)
            };

            var shippingAddress = new Address
            {
                Name = shippingAddressFields.Name.Trim(),
                StreetAddress = shippingAddressFields.StreetAddress.Trim(),
                StreetAddress2 = shippingAddressFields.StreetAddress2 == null ? shippingAddressFields.StreetAddress2 : shippingAddressFields.StreetAddress2.Trim(),
                State = shippingAddressFields.State.Trim(),
                City = shippingAddressFields.City.Trim(),
                Country = shippingAddressFields.Country.Trim(),
                Zip = shippingAddressFields.Zip.Trim(),
                Phone = shippingAddressFields.Phone.Trim(),
                AddressType = Enum.GetName(typeof(AddressType), shippingAddressFields.TypeOfAddress)
            };

            _membershipService.UpdateUserDetails(_globals.Tenant,
                updateFields.FirstName.Trim(),
                updateFields.LastName.Trim(),
                //updateFields.Password,
                updateFields.Email.Trim(),
                updateFields.Institution,
                billingAddress,
                shippingAddress,
                updateFields.Title == null ? "na" : updateFields.Title.Trim()
                );
        }

        public EditBillingAddressModel BuildBillingAddressModel()
        {
            var model = new EditBillingAddressModel();

            var user = GetWebUserFromIPrincipal();
            var addresses = user.Addresses.ToArray();
            var billingAddress = addresses.First(a => a.AddressType == WebUiConstants.BillingAddress);

            model.BillingAddress = new AddressModel
            {
                Name = string.Concat(user.FirstName, ' ', user.LastName),
                City = billingAddress.City,
                Country = billingAddress.Country,
                StreetAddress = billingAddress.StreetAddress,
                StreetAddress2 = billingAddress.StreetAddress2,
                State = billingAddress.State,
                Zip = billingAddress.Zip,
                Phone = billingAddress.Phone,
                TypeOfAddress = AddressType.Billing

            };

            return model;
        }

        public EditShippingAddressModel BuildShippingAddressModel()
        {
            var model = new EditShippingAddressModel();
            
            var user = GetWebUserFromIPrincipal();
            var addresses = user.Addresses.ToArray();
            var shippingAddress = addresses.First(a => a.AddressType == WebUiConstants.ShippingAddress);


            model.ShippingAddress = new AddressModel
            {
                City = shippingAddress.City,
                Country = shippingAddress.Country,
                StreetAddress = shippingAddress.StreetAddress,
                StreetAddress2 = shippingAddress.StreetAddress2,
                State = shippingAddress.State,
                Zip = shippingAddress.Zip,
                Phone = shippingAddress.Phone,
                Name = shippingAddress.Name,
                TypeOfAddress = AddressType.Shipping
            };

            return model;
        }

        public void BuildCityStateTimeZoneData(Dictionary<string, string> cityStateTimeZoneData, string zipAddress)
        {
            var zipCentricFields = zipAddress.Split(',');

            Debug.Assert(zipCentricFields.Length == 3, "There must be 3 fields in zipCentricFields, otherwise we have bad data.");

            if (zipCentricFields.Length < 3)
                zipCentricFields = AppHelper.AddNonvalidToArray(zipCentricFields);

            var myCity = new string(AppHelper.CharsToTitleCase(zipCentricFields[0]).ToArray());

            cityStateTimeZoneData.Add("success", "true");
            cityStateTimeZoneData.Add("City", myCity);
            cityStateTimeZoneData.Add("State", zipCentricFields[1]);
            cityStateTimeZoneData.Add("TimeZone", ((int)Enum.Parse(typeof(USTimeZone), zipCentricFields[2])).ToString(CultureInfo.InvariantCulture));
        }

        public bool ChangePasswordFromResetKey(string key, string password)
        {
            return _membershipService.ChangePasswordFromResetKey(key, password);
        }

        public UserAccount CreateUserAccountFromCart(RegisterViewModel model)
        {
            var email = model.RegisterFields.Email.Trim();
            var firstName = model.RegisterFields.FirstName.Trim();
            var lastName = model.RegisterFields.LastName.Trim();

            ////first check if email exists
            //var checkIfUsed = _membershipService.GetUserByEmail(email);
            //if (checkIfUsed != null)
            //{
            //    _logger.Error("dupe email attempt: " + email);
            //}
            //  model.Passwor should be null or empty. Generate own temp password here
            model.RegisterFields.Password = PasswordGenerator.GenerateRandomString(8);
            if (_stateService.HasValue(DomainConstants.TempPassword))
                _stateService.ClearValue(DomainConstants.TempPassword);

            _stateService.SetValue(DomainConstants.TempPassword, model.RegisterFields.Password);
                // will use in TtsTokenizer
#if DEBUG
            _logger.Info("Password {0} created for user {1}", model.RegisterFields.Password, model.RegisterFields.Email);
#endif

            var userAccount = _membershipService.CreateUser(
                _globals.Tenant,
                firstName,
                lastName,
                string.Empty,
                //  Pass empty string for username because MembershipReboot assigns the email to the username field where emailIsUsername is true
                model.RegisterFields.Password,
                email);

            _membershipService.AddAccountTypeNotVerifiedClaim(userAccount, ClaimValues.CartRegistration);

            Debug.Assert(_stateService.HasValue(DomainConstants.VerificationKey),
                "There's no reason session should not have a value for the VerificationKey at this point ");

            var verificationKey = _stateService.GetValue<string>(DomainConstants.VerificationKey);
            _stateService.ClearValue(DomainConstants.VerificationKey);

            userAccount = _membershipService.VerifyEmailFromKey(
                verificationKey,
                model.RegisterFields.Password
                );

            return userAccount;

        }

        public WebUser CreateWebUserFromCart(RegisterViewModel model)
        {
            var email = model.RegisterFields.Email.Trim();
            var firstName = model.RegisterFields.FirstName.Trim();
            var lastName = model.RegisterFields.LastName.Trim();

            var myInstitution = _membershipService.ProcessInstitutionForUser(
                model.RegisterFields.Institution.Trim(),
                email,
                model.RegisterFields.BillingAddress.City.Trim(),
                model.RegisterFields.BillingAddress.State.Trim(),
                "N",
                "New",
                model.RegisterFields.BillingAddress.Zip.Trim());

            IList<Address> addresses = new List<Address>
            {
                new Address
                {
                    AddressType =
                        Enum.GetName(typeof (AddressType), model.RegisterFields.BillingAddress.TypeOfAddress),
                    City = model.RegisterFields.BillingAddress.City.Trim(),
                    Country = model.RegisterFields.BillingAddress.Country.Trim(),
                    Name = firstName + ' ' + lastName,
                    Phone = model.RegisterFields.BillingAddress.Phone.Trim(),
                    State = model.RegisterFields.BillingAddress.State.Trim(),
                    StreetAddress = model.RegisterFields.BillingAddress.StreetAddress.Trim(),
                    StreetAddress2 =
                        model.RegisterFields.BillingAddress.StreetAddress2 == null
                            ? model.RegisterFields.BillingAddress.StreetAddress2
                            : model.RegisterFields.BillingAddress.StreetAddress2.Trim(),
                    Zip = model.RegisterFields.BillingAddress.Zip.Trim()
                },
                new Address
                {
                    AddressType =
                        Enum.GetName(typeof (AddressType), model.RegisterFields.ShippingAddress.TypeOfAddress),
                    City = model.RegisterFields.ShippingAddress.City.Trim(),
                    Country = model.RegisterFields.ShippingAddress.Country.Trim(),
                    Name = firstName + ' ' + lastName,
                    Phone = model.RegisterFields.ShippingAddress.Phone.Trim(),
                    State = model.RegisterFields.ShippingAddress.State.Trim(),
                    StreetAddress = model.RegisterFields.ShippingAddress.StreetAddress.Trim(),
                    StreetAddress2 =
                        model.RegisterFields.ShippingAddress.StreetAddress2 == null
                            ? model.RegisterFields.ShippingAddress.StreetAddress2
                            : model.RegisterFields.ShippingAddress.StreetAddress2.Trim(),
                    Zip = model.RegisterFields.ShippingAddress.Zip.Trim()
                }
            };

            var webUser = _membershipService.CreateWebUser(_globals.Tenant
                , firstName
                , lastName
                , string.Empty
                , email
                , USTimeZone.Central
                , UserType.Customer
                , myInstitution.idInstitution
                , addresses
                ,
                model.RegisterFields.Title == null
                    ? model.RegisterFields.Title
                    : model.RegisterFields.Title.Trim()
                , null
                , DomainConstants.Active
                );

            if (!_stateService.HasValue(Constants.CurrentUser))
                _stateService.SetValue(Constants.CurrentUser, webUser);

            return webUser;
        }

        public Institution GetInstitutionFromEmail(string email)
        {
            var domain = email.Split('@')[1];
            return _membershipService.GetInstitutionByDomain(domain);
        }

        public WebUser GetWebUserByEmail(string email)
        {
            return _membershipService.GetUserByEmail(email);
        }

        public WebUser GetWebUserById(int id)
        {
            return _membershipService.GetWebUserById(id);
        }

        public WebUser GetWebUserFromIPrincipal()
        {
            var identity = ClaimsPrincipal.Current;

            var userAccount = _membershipService.GetUserAccountByUserId(ClaimsExtensions.GetUserID(identity));

            var user = _membershipService.GetUserByEmail(userAccount.Email);
            return user;
        }

        public string GetZipAddress(int zip)
        {
            return AppHelper.GetCityStateFromZip(zip);
        }

        public LoginModel BuildLoginModel(string returnUrl)
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

        public CreateUserConfirmedViewModel ConfirmUser(string email, string password)
        {
            var changeEmailFromKeyInputModel = new CreateUserConfirmedViewModel
            {
                Email = email,
                OldPassword = password,
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