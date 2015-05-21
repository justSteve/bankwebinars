using BrockAllen.MembershipReboot;
using CUWebinars.Business.AccountService;
using CUWebinars.Business.Constants;
using CUWebinars.Business.Models;
using CUWebinars.Business.Services;
using CUWebinars.Web.Helpers;
using CUWebinars.Web.Infrastructure;
using CUWebinars.Web.Mapping.Mappers;
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
        private readonly IUniversalMapper _universalMapper;
        private readonly IOrderManagementService _orderManagementService;
        private bool _disposed;

        public
            AccountControllerOrchestrator(ILogger logger,
            IMembershipService membershipService,
            IOrderManagementService orderManagementService,
            IStateService stateService,
            HttpRequestBase request,
            IAppHelper appHelper,
            IUniversalMapper universalMapper)
        {
            _request = request;
            _logger = logger;
            _membershipService = membershipService;
            _orderManagementService = orderManagementService;
            _stateService = stateService;
            _appHelper = appHelper;
            _universalMapper = universalMapper;
        }

        public bool LogUserIn(SignInModel signInModel)
        {
            _logger.Info("LogUserIn {0}. Session={1} ", signInModel.Email, _appHelper.GetUserAuditInfo());

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

                    _logger.Info("Account.SignIn {0}. Redirecting to: {2},  Session={1} ", model.Email, _appHelper.GetUserAuditInfo(), retUrl);
                }

                return true;
            }
            _logger.Info("Account.SignIn FAILED on {0}. Session={1} ", model.Email, _appHelper.GetUserAuditInfo());
            return false;
        }

        public void UpdateBillingEmailOfOrder(int idOrder, string email)
        {
            _orderManagementService.UpdateOrderWithUserEmail(idOrder, email);
        }

        public bool UserConfirmed(CreateUserConfirmedViewModel model)
        {

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

                _logger.Info("Account.Confirmed {1}. Session={0}", _appHelper.GetUserAuditInfo(), model.Email);

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
                DateBilled = discountModel.DateBilled,
                DateValidFrom = discountModel.DateValidFrom,
                //idUser = idUser,
                DateValidTo = discountModel.DateValidTo,
                DiscountCode = discountModel.DiscountCode,
                DiscountType = discountModel.TypeOfDiscount,
                FlatOff = discountModel.FlatOff,
                Notes = discountModel.Notes,
                PercentOff = discountModel.PercentOff,
                RenewalTerm = discountModel.RenewalTerm,
                Status = discountModel.Status,
                UsesCount = discountModel.UsesCount,
                UsesRemain = discountModel.UsesRemain,
                //WebUserDiscountXref = 
                //idDiscount = 
            };

            _membershipService.UpdateDiscountDetails(discount);
        }

        public void EditUser(EditUserViewModel model)
        {
            var updateFields = model.EditFields;

            var billingAddressFields = updateFields.BillingAddress;
            var shippingAddressFields = updateFields.ShippingAddress;

            var billingAddress = new Address
            {
                Name = model.EditFields.FirstName + ' ' + model.EditFields.LastName,
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
                updateFields.Email.Trim(),
                updateFields.Institution,
                billingAddress,
                shippingAddress,
                updateFields.Title == null ? "na" : updateFields.Title.Trim()
                );

        }

        public void AddShippingAddressVerifiedClaim(int userId)
        {
            var userAccount = _membershipService.GetUserAccountByWebUserId(_globals.Tenant, userId);
            _membershipService.AddClaim(userAccount, ClaimTypes.AddressVerified, "true");
        }

        public MyWebinarsDTO BuildMyWebinarsDTO(DiscountModel discountModel, ClaimsIdentity claimsIdentityOfAuthenticatedUser)
        {
            var currentUser = GetWebUserFromIPrincipal();

            var model = new MyWebinarsDTO
            {
                WebUser = currentUser,
                OrderHasAdditionalLocationsViewModel = new OrderHasAdditionalLocationsViewModel()
                {

                }
            };

            if (claimsIdentityOfAuthenticatedUser.HasClaim(ClaimTypes.DisplayPostEventMaterials))
            {
                model.MyClaims =
                    claimsIdentityOfAuthenticatedUser.Claims.Where(c => c.Type == ClaimTypes.DisplayPostEventMaterials)
                        .Select(c => c.Value);
            }

            if (discountModel.TypeOfDiscount == DiscountType.ComplianceSeries)
            {
                model.Subscription = discountModel;
            }

            if (discountModel.TypeOfDiscount == DiscountType.Package)
            {
                model.Package = discountModel;
            }

            model.Scheduled = _orderManagementService.SelectOrdersWithScheduledWebinars(currentUser.idUser);
            model.Recorded = _orderManagementService.SelectOrdersWithRecordedWebinars(currentUser.idUser);
            model.Archived = _orderManagementService.SelectOrdersWithArchivedWebinars(currentUser.idUser);

            foreach (var orderRow in model.Scheduled.Select(order => order.OrderRows
                .Single(or => or.RowStatus == OrderRowStatus.Active))
                .Where(row => row.Webinar.Status == WebinarStatus.Active))
            {
                _orderManagementService.GetJoinUrl(orderRow);
            }

            return model;
        }



        public MyWebinarsDTO BuildOnDemandDTO(DiscountModel discountModel, ClaimsIdentity claimsIdentityOfAuthenticatedUser)
        {
            var currentUser = GetWebUserFromIPrincipal();

            var model = new MyWebinarsDTO
            {

                WebUser = currentUser,
                OrderHasAdditionalLocationsViewModel = new OrderHasAdditionalLocationsViewModel()
                {

                }
            };

            if (claimsIdentityOfAuthenticatedUser.HasClaim(ClaimTypes.DisplayPostEventMaterials))
            {
                model.MyClaims =
                    claimsIdentityOfAuthenticatedUser.Claims.Where(c => c.Type == ClaimTypes.DisplayPostEventMaterials)
                        .Select(c => c.Value);
            }

            if (discountModel.TypeOfDiscount == DiscountType.ComplianceSeries)
            {
                model.Subscription = discountModel;
            }

            if (discountModel.TypeOfDiscount == DiscountType.Package)
            {
                model.Package = discountModel;
            }

            model.Scheduled = _orderManagementService.SelectOrdersWithScheduledWebinars(currentUser.idUser);
            model.Recorded = _orderManagementService.SelectOrdersWithRecordedWebinars(currentUser.idUser);
            model.Archived = _orderManagementService.SelectOrdersWithArchivedWebinars(currentUser.idUser);

            foreach (var orderRow in model.Scheduled.Select(order => order.OrderRows
                .Single(or => or.RowStatus == OrderRowStatus.Active))
                .Where(row => row.Webinar.Status == WebinarStatus.Active))
            {
                _orderManagementService.GetJoinUrl(orderRow);
            }

            return model;
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

        public void AddFullNameClaim(RegisterViewModel model)
        {
            var userAccount = _membershipService.GetUserAccountByEmail(_globals.Tenant, model.RegisterFields.Email);

            _membershipService.AddClaim(userAccount,
                ClaimTypes.FullName,
                string.Concat(model.RegisterFields.FirstName.Trim(), ' ', model.RegisterFields.LastName.Trim())
                );
        }

        public bool AddPasswordForCartCreatedUser(CreateUserConfirmedViewModel model)
        {
            UserAccount userAccount;
            int retries = 0;

            // Try and find the user for up to 20s. 
            // If it still does not exist, chuck an exception.
            _logger.Error(string.Format("Beginning GetUserAccountByEmail with timeout set to {0} seconds.", _globals.RetryCount / 2));

            do
            {
                if (retries >= _globals.RetryCount - 1)
                    _logger.Error(string.Format("GetUserAccountByEmail times out after {0} seconds.", retries / 2));

                userAccount = _membershipService.GetUserAccountByEmail(_globals.Tenant, model.Email);

                if (ReferenceEquals(userAccount, null))
                {
                    // sleeping for half second, so 2 retries will result in 1s of sleeping.
                    Thread.Sleep(500);

                    // Every 5 seconds, log the fact that the flow has been stuck here for the then current duration.
                    if (retries % 10 == 0 && retries > 0)
                    {
                        _logger.Info(string.Format("UserAccount returning null after {0} seconds.", retries / 2));
                    }
                }
                else
                {
                    break;
                }


            } while (retries++ < _globals.RetryCount);

            //  if still null here, retries have exceeded RetryCount and operation aborted
            if (ReferenceEquals(userAccount, null))
                return false;

            if (!userAccount.HasClaim(ClaimTypes.FullName))
            {
                _membershipService.AddClaim(userAccount, ClaimTypes.FullName, _orderManagementService.GetWebUserFullname(model.Email));
            }

            _membershipService.VerifyUserByEmail(_globals.Tenant, model.Email);

            _stateService.SetValue(DomainConstants.CartCreatedUserPasswordCreate, true);


            retries = 0; // re-use and re-set retries.

            do
            {
                try
                {
                    _membershipService.ResetPassword(_globals.Tenant, model.Email);
                    break; // reached if no exception is thrown
                }
                catch (Exception exception)
                {
                    _logger.ErrorException("ResetPassword: ", exception);
                    Elmah.ErrorSignal.FromCurrentContext().Raise(exception);
                }
                Thread.Sleep(500);
            } while (retries++ < _globals.RetryCount);

            var verificationKey = _stateService.GetValue<string>(DomainConstants.VerificationKey);

            _logger.Info("verificationKey is {0}", verificationKey);

            _stateService.ClearValue(DomainConstants.CartCreatedUserPasswordCreate);

            retries = 0; // re-use and re-set retries.

            do
            {
                try
                {
                    _membershipService.ChangePasswordFromResetKey(verificationKey, model.NewPassword);
                    break; // reached if no exception is thrown
                }
                catch (Exception exception)
                {
                    _logger.ErrorException("ChangePasswordFromResetKey: ", exception);
                    Elmah.ErrorSignal.FromCurrentContext().Raise(exception);
                }

                Thread.Sleep(500);

            } while (retries++ < _globals.RetryCount);

            _stateService.ClearValue(DomainConstants.VerificationKey);

            _logger.Info("Account.Confirmed POST. Session={0}", _appHelper.GetUserAuditInfo());

            return true;
        }

        public AddQuizEditModel BuildAddQuizEditModel()
        {
            throw new NotImplementedException();
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

        public DiscountModel BuildDiscountModel()
        {
            var discountModel = new DiscountModel();
            var currentUser = GetWebUserFromIPrincipal();
            var userDiscount = _orderManagementService.GetDiscountByUser(currentUser);

            _universalMapper.Map(userDiscount, discountModel);

            if (discountModel.TypeOfDiscount == DiscountType.ComplianceSeries)
            {
                discountModel.DateValidFrom = userDiscount.DateValidFrom;
                discountModel.DateValidTo = userDiscount.DateValidTo;

                // ???
            }

            return discountModel;
        }

        public ManageModel BuildManageModel(ManageMessageId? message)
        {
            var manageModel = new ManageModel
            {
                StatusMessage = string.Empty
            };

            var user = GetWebUserFromIPrincipal();

            var addresses = user.Addresses.ToArray();
            var billingAddress = addresses.First(a => a.AddressType == WebUiConstants.BillingAddress);
            var shippingAddress = addresses.First(a => a.AddressType == WebUiConstants.ShippingAddress);

            manageModel.RegisterFields = new RegisterModel
            {
                BillingAddress = new AddressModel
                {

                    Name = user.FirstName + ' ' + user.LastName,
                    City = billingAddress.City,
                    Country = billingAddress.Country,
                    StreetAddress = billingAddress.StreetAddress,
                    StreetAddress2 = billingAddress.StreetAddress2,
                    State = billingAddress.State,
                    Zip = billingAddress.Zip,
                    Phone = billingAddress.Phone,
                    TypeOfAddress = AddressType.Billing
                },
                ShippingAddress = new AddressModel
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
                },
                FirstName = user.FirstName,
                LastName = user.LastName,
                Institution = user.Institution.InstitutionName,
                Email = user.email,
                Title = user.Title,
                AccountDetailsTitle = WebUiConstants.ManageUser
            };

            return manageModel;
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

        public string CreateUserAccountFromCart(string email)
        {
            // This boolean is a toggle which lives in the AppSettings of the config file.
            if (_globals.UseAzureWebjobs)
            {
                var storageCredentials = new StorageCredentials(_globals.StorageAccountName, _globals.StorageAccessKey);
                var cloudStorageAccount = new CloudStorageAccount(storageCredentials, false);

                CloudQueueClient _queueClient = cloudStorageAccount.CreateCloudQueueClient();

                CloudQueue cloudQueue = _queueClient.GetQueueReference(_globals.CreateUserQueueName);

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
                    BaseUrl =  string.Format(@"{0}://{1}{2}/", _request.Url.Scheme, _request.Url.Authority, _request.ApplicationPath.TrimEnd('/')),
                    Email = email,
                    FirstName= string.Empty,
                    LastName = string.Empty,
                    Password = password
                };

#if DEBUG
                _logger.Info("Password {0} created for user {1}", password, email);
#endif

                var payload = JsonConvert.SerializeObject(registerFieldsDto);
                var cloudQueueMessage = new CloudQueueMessage(payload);
                cloudQueue.EncodeMessage = true;
                cloudQueue.AddMessage(cloudQueueMessage);

                return password;
            }
            else
            {
                var password = PasswordGenerator.GenerateRandomString(8);

                // Take note of the fact that this registration occurred as part of the Checkout process.
                // This will result in the RegisterUser email being suppressed.
                _stateService.SetValue(DomainConstants.UserCreatedDuringCartCheckout, true);

                var userAccount = _membershipService.CreateUserFromCart(
                    _globals.Tenant,
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

            if (!emailIsAvailable)
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

        public Order GetOrderById(int idOrder)
        {
            return _orderManagementService.GetOrderById(idOrder);
        }

        public WebUser GetWebUserByEmail(string email)
        {
            return _membershipService.GetUserByEmail(email);
        }

        public int? GetWebUserIdByEmail(string email)
        {
            return _membershipService.GetWebUserIdByEmail(email);
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
                OldPassword = PasswordGenerator.GenerateRandomString(5),
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
                else
                {
                    break;
                }

            } while (retries++ < _globals.RetryCount);

            //if still null at this point, we have exceeded the retry limit and assume that something has gone wrong.
            if (ReferenceEquals(userAccount, null)) throw new Exception("User does not exist in system: " + email);

            if (!userAccount.HasClaim(ClaimTypes.FullName))
            {
                var webUser = _orderManagementService.GetWebUser(email);
                _membershipService.AddClaim(userAccount, ClaimTypes.FullName, string.Concat(webUser.FirstName, ' ', webUser.LastName));
            }

            //bool hasAlreadyVerifiedAccount = !userAccount.HasClaim(ClaimTypes.HasNotVerified);


            if (userAccount.HasClaim(ClaimTypes.HasNotVerified, ClaimValues.CartRegistration))
            {
                return changeEmailFromKeyInputModel;
            }

            changeEmailFromKeyInputModel.ScreenMessage = "There has been an error at the server.";

            return changeEmailFromKeyInputModel;
        }

        public CreateUserConfirmedViewModel GetCreateUserConfirmedViewModel(string email, int idOrder, bool viaBillMePostRequest = false)
        {
            var order = _orderManagementService.GetOrderById(idOrder);

            var changeEmailFromKeyInputModel = new CreateUserConfirmedViewModel
            {
                Email = email,
                OldPassword = PasswordGenerator.GenerateRandomString(5),
                NewPassword = string.Empty,
                ConfirmPassword = string.Empty,
                Order = order,
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

            if (ReferenceEquals(userAccount, null)) throw new Exception("User does not exist in system: " + email);

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

                _membershipService.Dispose();
                _orderManagementService.Dispose();

                _disposed = true;
            }
        }
    }
}