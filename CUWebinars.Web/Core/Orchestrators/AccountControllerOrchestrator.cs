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
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using ClaimsExtensions = CUWebinars.Web.Helpers.ClaimsExtensions;
using ClaimTypes = CUWebinars.Business.Constants.ClaimTypes;

namespace CUWebinars.Web.Core.Orchestrators
{
    public class AccountControllerOrchestrator : IAccountControllerOrchestrator
    {
        private readonly GlobalConfig _globals = GlobalConfig.GlobalConfigSingleton;
        private readonly HttpRequestBase _request;
        private readonly ILogger _logger;
        private readonly IMembershipService _membershipService;
        private readonly IStateService _stateService;
        private readonly IAppHelper _appHelper;
        private readonly IOrderManagementService _orderManagementService;
        private bool _disposed;

        public 
            AccountControllerOrchestrator(ILogger logger,
            IMembershipService membershipService,
            IOrderManagementService orderManagementService,
            IStateService stateService,
            HttpRequestBase request,
            IAppHelper appHelper)

        {
            _request = request;
            _logger = logger;
            _membershipService = membershipService;
            _orderManagementService = orderManagementService;
            _stateService = stateService;
            _appHelper = appHelper;
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

            if (!_stateService.HasValue(WebUiConstants.CurrentUser))
                _stateService.SetValue(WebUiConstants.CurrentUser, webUser);

            var userAccount = _membershipService.CreateUser(
                _globals.Tenant,
                firstName,
                lastName,
                string.Empty,
                //  Pass empty string for username because MembershipReboot assigns the email to the username field where emailIsUsername is true
                model.RegisterFields.Password,
                email);

            // The following Claim is added so the system knows that the User is yet to pro-actively verify its account.
            // Note: this is a CUW/BW construct of Verified, as distinct from the MR idea of Verified (which is dealt with below)
            _membershipService.AddAccountTypeNotVerifiedClaim(userAccount, ClaimValues.ManualRegistration);

            Debug.Assert(_stateService.HasValue(DomainConstants.VerificationKey), "There's no reason session should not have a value for the VerificationKey at this point ");

            var verificationKey = _stateService.GetValue<string>(DomainConstants.VerificationKey);
            _stateService.ClearValue(DomainConstants.VerificationKey);

            _membershipService.VerifyEmailFromKey(
                verificationKey,
                model.RegisterFields.Password
                ); // verify the user to unlock functionality like PasswordReset

            _membershipService.LogInUser(
                _globals.Tenant, 
                model.RegisterFields.Email,
                model.RegisterFields.Password, 
                true
                ); // log the user in.            

        }

        public void ResetPassword(string tenant, string email)
        {
            _stateService.SetValue(DomainConstants.ResetPasswordRequested, true);
            _membershipService.ResetPassword(tenant, email);
        }

        public bool SignUserIn(SignInModel model, out string userMustVerify)
        {
            if (_membershipService.LogInUser(_globals.Tenant, model.Email, model.Password, model.RememberMe, out userMustVerify))
            {
                if (!ReferenceEquals(_request.ApplicationPath, null) && !ReferenceEquals(_request.Url, null))
                {
                    var retUrl = model.ReturnUrl.Replace(
                            string.Format(@"{0}://{1}{2}/",
                            _request.Url.Scheme,
                            _request.Url.Authority,
                            _request.ApplicationPath.TrimEnd('/')),
                            string.Empty
                            );

                    _logger.Info("Account.SignIn Post Success. Session={0}, Redirecting to: {1}", _appHelper.GetUserAuditInfo(), retUrl);
                }

                return true;
            }

            return false;
        }

        public void UpdateBillingEmailOfOrder(int idOrder, string email)
        {
            _orderManagementService.UpdateOrderWithUserEmail(idOrder, email);
        }

        public bool UserConfirmed(CreateUserConfirmedViewModel model)
        {
            //  TODO: __Is this directive still operative? add try catch and reverse as should be atomic.
            if (_membershipService.VerifyUserByEmail(_globals.Tenant, model.Email))
            {
                _stateService.SetValue(DomainConstants.UserCreatedViaNewOrder, true);
                _membershipService.ResetPassword(_globals.Tenant, model.Email);

                var verificationKey = _stateService.GetValue<string>(DomainConstants.VerificationKey);
                
                _stateService.ClearValue(DomainConstants.UserCreatedViaNewOrder);

                try
                {
                    _membershipService.ChangePasswordFromResetKey(verificationKey, model.NewPassword);
                }
                catch (Exception exception)
                {
                    _logger.ErrorException("ChangePasswordFromResetKey: ", exception);
                    Elmah.ErrorSignal.FromCurrentContext().Raise(exception);
                }

                _stateService.ClearValue(DomainConstants.VerificationKey);

                _membershipService.LogInUser(_globals.Tenant, model.Email, model.NewPassword, true);

                _logger.Info("Account.Confirmed POST. Session={0}", _appHelper.GetUserAuditInfo());
                
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

        public void UpdateShippingAddressDetails(AddressModel shippingAddressModel, int idUser)
        {
            var shippingAddress = new Address
            {
                AddressType = shippingAddressModel.TypeOfAddress.ToString(),
                City = shippingAddressModel.City,
                Country = shippingAddressModel.Country,
                idUser = idUser,
                Name = shippingAddressModel.Name,
                Phone = shippingAddressModel.Phone,
                State = shippingAddressModel.State,
                StreetAddress = shippingAddressModel.StreetAddress,
                StreetAddress2 = shippingAddressModel.StreetAddress2,
                Zip = shippingAddressModel.Zip
            };

            _membershipService.UpdateShippingAddressDetails(shippingAddress);
        }

        public void UpdateDiscountDetails(DiscountModel discountModel, int idUser)
        {
            var discount = new Discount
            {
                Cost = discountModel.Cost,
                DateBilled= discountModel.DateBilled,
                 DateValidFrom= discountModel.DateValidFrom,
                //idUser = idUser,
                DateValidTo= discountModel.DateValidTo,
                DiscountCode = discountModel.DiscountCode,
                DiscountType= discountModel.TypeOfDiscount,
                FlatOff= discountModel.FlatOff,
                Notes= discountModel.Notes,
                PercentOff= discountModel.PercentOff,
                RenewalTerm= discountModel.RenewalTerm,
                Status= discountModel.Status,
                UsesCount= discountModel.UsesCount,
                UsesRemain= discountModel.UsesRemain,
                //WebUserDiscountXref = 
                //idDiscount = 
            };

            //_membershipService.UpdateDiscountDetails(discount);
        }

        public void EditContactInfo(EditContactInfoModel model)
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

        public void AddPasswordForCartCreatedUser(CreateUserConfirmedViewModel model)
        {
            UserAccount userAccount;
            int retries = 0;

            // Try and find the user for up to 20s. If it still does not exist, chuck an exception.
            do
            {
                userAccount = _membershipService.GetUserAccountByEmail(_globals.Tenant, model.Email);

                if (ReferenceEquals(userAccount, null))
                {
                    // sleeping for half second, so 2 retries will result in 1s of sleeping.
                    Thread.Sleep(500);

                    // Every 20 seconds, log the fact that the flow has been stuck here for the then current duration.
                    if (retries%40 == 0 && retries > 0)
                    {
                        _logger.Info(string.Format("UserAccount returning null after {0} seconds.", retries/2));
                    }
                }
                else
                {
                    break;
                }


            } while (retries++ < _globals.RetryCount);

            _membershipService.VerifyUserByEmail(_globals.Tenant, model.Email);

            _stateService.SetValue(DomainConstants.CartCreatedUserPasswordCreate, true);
            _membershipService.ResetPassword(_globals.Tenant, model.Email);

            var verificationKey = _stateService.GetValue<string>(DomainConstants.VerificationKey);

            _stateService.ClearValue(DomainConstants.CartCreatedUserPasswordCreate);

            try
            {
                _membershipService.ChangePasswordFromResetKey(verificationKey, model.NewPassword);
            }
            catch (Exception exception)
            {
                _logger.ErrorException("ChangePasswordFromResetKey: ", exception);
                Elmah.ErrorSignal.FromCurrentContext().Raise(exception);
            }

            _stateService.ClearValue(DomainConstants.VerificationKey);

            //_membershipService.LogInUser(_globals.Tenant, model.Email, model.NewPassword, true);

            _logger.Info("Account.Confirmed POST. Session={0}", _appHelper.GetUserAuditInfo());
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

        public string CreateUserAccountFromCart(RegisterViewModel model)
        {
            // This boolean is a toggle which live in the AppSettings of the config file.
            if (_globals.UseAzureWebjobs)
            {
                var storageCredentials = new StorageCredentials(_globals.StorageAccountName, _globals.StorageAccessKey);
                var cloudStorageAccount = new CloudStorageAccount(storageCredentials, false);

                CloudQueueClient _queueClient = cloudStorageAccount.CreateCloudQueueClient();

                CloudQueue cloudQueue = _queueClient.GetQueueReference(_globals.CreateUserQueueName);

                var email = model.RegisterFields.Email.Trim();
                var firstName = model.RegisterFields.FirstName.Trim();
                var lastName = model.RegisterFields.LastName.Trim();

                //first check if email exists
                var userExists = _membershipService.GetUserAccountByEmail(_globals.Tenant, email);
                if (userExists != null)
                {
                    _logger.Error("dupe email attempt: " + email);
                    throw new Exception("User already exists.");
                }

                var password = PasswordGenerator.GenerateRandomString(8);

                var registerFieldsDto = new RegisterFieldsDTO
                {
                    Email = email,
                    FirstName = firstName,
                    LastName = lastName,
                    Password = password
                };

#if DEBUG
                _logger.Info("Password {0} created for user {1}", model.RegisterFields.Password,
                    model.RegisterFields.Email);
#endif

                var payload = JsonConvert.SerializeObject(registerFieldsDto);
                var cloudQueueMessage = new CloudQueueMessage(payload);
                cloudQueue.EncodeMessage = true;
                cloudQueue.AddMessage(cloudQueueMessage);

                return password;
            }
            else
            {
                var email = model.RegisterFields.Email.Trim();
                var firstName = model.RegisterFields.FirstName.Trim();
                var lastName = model.RegisterFields.LastName.Trim();

                var password = PasswordGenerator.GenerateRandomString(8);

                // take note of the fact that this registration occurred as part of the Checkout process.
                _stateService.SetValue(DomainConstants.UserCreatedDuringCartCheckout, true);

                var userAccount = _membershipService.CreateUser(
                    _globals.Tenant,
                    firstName,
                    lastName,
                    string.Empty,
                    //  Pass empty string for username because MembershipReboot assigns the email to the username field where emailIsUsername is true
                    password, // use lastName as password
                    email);

                // The following Claim is added so the system knows that the User is yet to pro-actively verify its account.
                // Note: this is a CUW/BW construct of Verified, as distinct from the MR idea of Verified (which is dealt with below)
                _membershipService.AddAccountTypeNotVerifiedClaim(userAccount, ClaimValues.CartRegistration);

                var verificationKey = _stateService.GetValue<string>(DomainConstants.VerificationKey);
                _stateService.ClearValue(DomainConstants.VerificationKey);
                _stateService.ClearValue(DomainConstants.UserCreatedDuringCartCheckout);

                // verify the user to unlock functionality like PasswordReset
                _membershipService.VerifyEmailFromKey(
                    verificationKey,
                    password
                    );

                return password;
            }

        }

        public WebUser CreateWebUserFromCart(RegisterViewModel model)
        {
            var email = model.RegisterFields.Email.Trim();
            var firstName = model.RegisterFields.FirstName.Trim();
            var lastName = model.RegisterFields.LastName.Trim();

            bool emailIsAvailable = _membershipService.GetUserByEmail(email) == null;

            if(!emailIsAvailable)
                throw new ValidationException(string.Format("The email address {0} is already in use by an existing user.", email));

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

            if (!_stateService.HasValue(WebUiConstants.CurrentUser))
                _stateService.SetValue(WebUiConstants.CurrentUser, webUser);

            return webUser;
        }

        public Institution GetInstitutionFromEmail(string email)
        {
            var domain = email.Split('@')[1];
            return _membershipService.GetInstitutionByDomain(domain);
        }

        public IEnumerable<Institution> GetInstitutionsByName(string name)
        {
            return _membershipService.GetInstitutionsByName(name);
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
            return _appHelper.GetCityStateFromZip(zip);
        }

        public LoginModel BuildLoginModel(string returnUrl)
        {
            var urlHelper = new UrlHelper(_request.RequestContext); 

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
            if (string.IsNullOrEmpty(returnUrl) && _request.UrlReferrer != null)
            {
                returnUrl = HttpUtility.UrlDecode(_request.UrlReferrer.PathAndQuery);
                _logger.Info("Login|returnURL was: " + returnUrl + " Session=" + _appHelper.GetUserAuditInfo());
            }

            if (urlHelper.IsLocalUrl(returnUrl) && !string.IsNullOrEmpty(returnUrl))
            {
                if (returnUrl.Contains("PasswordResetConfirm") || returnUrl.Equals("/Account/Signin") || returnUrl.Equals("/Account/Login"))
                    returnUrl = "/Account/MyWebinars";

                loginModel.ReturnUrl = returnUrl;
                loginModel.SignIn.ReturnUrl = returnUrl;
            }

            loginModel.ActiveTab = "login";
            
            return loginModel;
        }
        
        public CreateUserConfirmedViewModel PrepareViewForCartUserAddingPassword(string email, bool viaBillMePostRequest = false)
        {
            UserAccount userAccount;
            int retries = 0;

            var changeEmailFromKeyInputModel = new CreateUserConfirmedViewModel
            {
                Email = email,
                OldPassword = PasswordGenerator.RandomStringFast(5),
                NewPassword = string.Empty,
                ConfirmPassword = string.Empty,
                ScreenMessage = string.Empty,
                UserIsLoggedIn = _request.IsAuthenticated
            };

            if (viaBillMePostRequest)
                changeEmailFromKeyInputModel.ScreenMessage = "Thank you for your order.";

             // Try and find the user for up to 10s. If it still does not exist, chuck an exception.
            do
            {
                userAccount = _membershipService.GetUserAccountByEmail(_globals.Tenant, email);

                if (ReferenceEquals(userAccount, null))
                {
                    // sleeping for half second, so 2 retries will result in 1s of sleeping.
                    Thread.Sleep(500);

                    // Every 20 seconds, log the fact that the flow has been stuck here for the then current duration.
                    if (retries%40 == 0 && retries > 0)
                    {
                        _logger.Info(string.Format("UserAccount returning null after {0} seconds.", retries/2));
                    }
                }

            } while (retries++ < _globals.RetryCount);

             //if still null at this point, we have exceeded the retry limit and assume that something has gone wrong.
            if (ReferenceEquals(userAccount, null)) throw new Exception("User does not exist in system");

            bool hasAlreadyVerifiedAccount = !userAccount.HasClaim(ClaimTypes.HasNotVerified);

            if (hasAlreadyVerifiedAccount)
            {
                var loginLinkBuilder = new TagBuilder("a");
                loginLinkBuilder.MergeAttributes(new Dictionary<string, string>{{"href", @"/Account/Login"}});
                loginLinkBuilder.SetInnerText("Login Page");

                var para1TagBuilder = new TagBuilder("div");
                para1TagBuilder.InnerHtml = "You've already created your initial password.";

                var para2TagBuilder = new TagBuilder("div");
                para2TagBuilder.InnerHtml =
                    string.Format("Please use the Password Reset feature on the {0} to reset your password.", loginLinkBuilder.ToString(TagRenderMode.Normal));

                changeEmailFromKeyInputModel.ScreenMessage =
                    string.Concat(
                        para1TagBuilder.ToString(TagRenderMode.Normal),
                        para2TagBuilder.ToString(TagRenderMode.Normal)
                        );

                return changeEmailFromKeyInputModel;
            }

            if (userAccount.HasClaim(ClaimTypes.HasNotVerified, ClaimValues.CartRegistration))
            {
                return changeEmailFromKeyInputModel;
            }

            changeEmailFromKeyInputModel.ScreenMessage = "There has been an error at the server.";

            return changeEmailFromKeyInputModel;
        }

        public CreateUserConfirmedViewModel GetCreateUserConfirmedViewModel(string email, bool viaBillMePostRequest = false)
        {
            //UserAccount userAccount;
            //int retries = 0;

            var changeEmailFromKeyInputModel = new CreateUserConfirmedViewModel
            {
                Email = email,
                OldPassword = PasswordGenerator.RandomStringFast(5),
                NewPassword = string.Empty,
                ConfirmPassword = string.Empty,
                ScreenMessage = string.Empty,
                UserIsLoggedIn = _request.IsAuthenticated
            };

            if (viaBillMePostRequest)
                changeEmailFromKeyInputModel.ScreenMessage = "Thank you for your order.";


            return changeEmailFromKeyInputModel;
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
                UserIsLoggedIn = _request.IsAuthenticated
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