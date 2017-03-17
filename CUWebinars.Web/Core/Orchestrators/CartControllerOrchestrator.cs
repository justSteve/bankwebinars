using System.Collections;
using System.Diagnostics;
using System.Security.Claims;
using CUWebinars.Business.AccountService;
using CUWebinars.Business.Constants;
using CUWebinars.Business.Core;
using CUWebinars.Business.Core.Helpers;
using CUWebinars.Business.Models;
using CUWebinars.Business.Models.Mapping;
using CUWebinars.Business.Notification;
using CUWebinars.Business.Notification.Formatters;
using CUWebinars.Business.Notification.ViewModel;
using CUWebinars.Business.Services;
using CUWebinars.Web.Helpers;
using CUWebinars.Web.Models;
using CUWebinars.Web.Services;
using CUWebinars.Web.ViewModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.DynamicData;
using CUWebinars.Business.Repository;
using CUWebinars.Web.Mapping.Mappers;
using CUWebinars.Web.Models.DataTablesModels;
using GemBox.Document;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using ClaimTypes = CUWebinars.Business.Constants.ClaimTypes;

namespace CUWebinars.Web.Core.Orchestrators
{
    public class CartControllerOrchestrator : ICartControllerOrchestrator
    {
        public HttpRequestBase Request { get; set; }

        private GlobalConfig _globals = GlobalConfig.GlobalConfigSingleton;

        private readonly IStateService _stateService;
        private readonly IMembershipService _membershipService;
        private readonly ILogger _logger;
        private readonly IAppHelper _appHelper;
        private readonly IFormatter _generalFormatter;
        private readonly IOrderManagementService _orderManagementService;
        private readonly IWebinarManagementService _webinarManagementService;
        private readonly IRegTypeRepository _regTypeRepository;
        private readonly IUniversalMapper _universalMapper;

        private bool _disposed;
        private GlobalConfig _globalConfig = GlobalConfig.GlobalConfigSingletonCreator.UniqueInstance;

        public CartControllerOrchestrator(IMembershipService membershipService,
            IOrderManagementService orderManagementService,
            IWebinarManagementService webinarManagementService,
            IStateService stateService,
            ILogger logger,
            HttpRequestBase request,
            IAppHelper appHelper,
            IFormatter generalFormatter,
                IRegTypeRepository regTypeRepository,

            IUniversalMapper universalMapper)
        {
            Request = request;
            _membershipService = membershipService;
            _logger = logger;
            _appHelper = appHelper;
            _generalFormatter = generalFormatter;
            _orderManagementService = orderManagementService;
            _webinarManagementService = webinarManagementService;
            _stateService = stateService;
            _regTypeRepository = regTypeRepository;

            _universalMapper = universalMapper;
        }

        public EditUserViewModel BuildEditUserViewModel()
        {
            var model = new EditUserViewModel
            {
                EditFields = new EditUserModel
                {
                    AccountDetailsTitle = WebUiConstants.Register,
                    BillingAddress = new AddressModel
                    {
                        TypeOfAddress = AddressType.Billing
                    },
                    ShippingAddress = new AddressModel
                    {
                        TypeOfAddress = AddressType.Shipping
                    }
                }
            };

            return model;
        }
        public RegisterViewModel BuildRegisterViewModel()
        {
            var model = new RegisterViewModel
            {
                RegisterFields = new RegisterModel
                {
                    AccountDetailsTitle = WebUiConstants.Register,
                    BillingAddress = new AddressModel
                    {
                        TypeOfAddress = AddressType.Billing
                    },
                    ShippingAddress = new AddressModel
                    {
                        TypeOfAddress = AddressType.Shipping
                    }
                }
            };

            return model;
        }

        public DisplayOptionsInDropDownViewModel BuildDisplayOptionsInDropDownViewModel(
            OrderRow orderRow,
            int? idOrderRow)
        {
            if (idOrderRow.HasValue && idOrderRow.Value > 0)
            {
                if (ReferenceEquals(orderRow, null))
                    orderRow = _orderManagementService.GetOrderRowById(idOrderRow.Value);

                try
                {
                    var displayOptionsInDropDownViewModel = new DisplayOptionsInDropDownViewModel
                    {
                        Options = _orderManagementService.GetOptionsByWebinarId(orderRow.idWebinar, true),
                        OrderRowId = idOrderRow.Value,
                        OrderRowRegistrationType = orderRow.RegistrationType
                    };

                    return displayOptionsInDropDownViewModel;
                }
                catch (Exception exception)
                {
                    _logger.ErrorException("BuildCheckOutViewModel", exception);
                    Elmah.ErrorSignal.FromCurrentContext().Raise(exception);
                    throw;
                }

            }

            return null;
        }

        public CheckoutConfirmViewModel BuildCheckoutConfirmViewModel(int? idOrder)
        {

            if (idOrder.HasValue && idOrder.Value > 0)
            {
                var order = _orderManagementService.GetOrderById(idOrder.Value);
                if (!ReferenceEquals(null, order))
                {

                    try
                    {
                        var existingOrders = _orderManagementService.FindOrdersByUserId(order.idUser)
                            .Where(
                                o =>
                                    o.OrderRows.SingleOrDefault(r => r.RowStatus == OrderRowStatus.Active).idWebinar ==
                                    order.OrderRows.SingleOrDefault(r => r.RowStatus == OrderRowStatus.Active).idWebinar).ToList();

                        if (existingOrders.Any())
                        {
                            foreach (var o in existingOrders)
                            {
                                if (o.idOrder != idOrder)
                                    _logger.Warn("BuildCheckoutConfirmViewModel found an existing order: " + o.idOrder + " same userid as: " + idOrder);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.FatalException("BuildCheckoutConfirmViewModel: ", ex);
                    }
                    try
                    {
                        var existingOrdersByEmail = _orderManagementService.GetOrdersByEmail(order.BillingEmail, 19) //_orderRepository.FindOrdersByBillingEmail(webUser.email, 19)
                            .Where(o =>o.OrderRows.FirstOrDefault(r => r.RowStatus == OrderRowStatus.Active).idWebinar ==
                                    order.OrderRows.SingleOrDefault(r => r.RowStatus == OrderRowStatus.Active).idWebinar).ToList();

                        if (existingOrdersByEmail.Any())
                        {

                            foreach (var o in existingOrdersByEmail)
                            {
                                if (o.idOrder != idOrder)
                                    _logger.Warn("BuildCheckoutConfirmViewModel found an existing order By Email: " + o.idOrder + " same userid as: " + idOrder);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.FatalException("BuildCheckoutConfirmViewModel found existing by email: ", ex);
                    }

                    try
                    {
                        _logger.Info("BuildCheckoutConfirmViewModel found: " + order.idOrder);
                        var webUser = order.WebUser;
                        var userFullName = string.Concat(webUser.FirstName, " ", webUser.LastName);

                        var addresses = webUser.Addresses.ToArray();
                        var billingAddress = addresses.First(a => a.AddressType == WebUiConstants.BillingAddress);
                        var shippingAddress =
                            addresses.FirstOrDefault(a => a.AddressType == WebUiConstants.ShippingAddress);

                        OrderRow row = order.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active);

                        // This ViewModel is built here because it is re-used.
                        var additionalLocationsViewModel =
                            BuildAdditionalLocationsViewModel(row, idOrder);

                        Affiliate aff = _stateService.GetValue<Affiliate>(WebUiConstants.CurrentAffiliate);
                        order.Affiliate = aff;

                        bool desCheckout = false;

                        if (row.Webinar.SeriesInfo == "DES")
                            desCheckout = true;
                        var checkoutConfirmViewModel = new CheckoutConfirmViewModel
                        {
                            DESCheckout = desCheckout,
                            AdditionalLocationCaption = DomainHelpers.BuildAdditionalLocationsCaption(row),
                            AdjustUserDetailsPanel = new AdjustUserDetailsEditModel
                            {
                                Email = webUser.email,
                                FirstName = webUser.FirstName,
                                idUser = webUser.idUser,
                                LastName = webUser.LastName,
                                Institution = order.Institution,
                                Title = webUser.Title,
                                BillingAddress = new AddressModel()
                                {
                                    StreetAddress = order.BillingAddress,
                                    StreetAddress2 = order.BillingAddress2,
                                    City = order.BillingCity,
                                    Name = order.FirstName + ' ' + order.LastName,
                                    Phone = order.BillingPhone,
                                    State = order.BillingState,
                                    TypeOfAddress = AddressType.Billing,
                                    Zip = order.BillingZip
                                },
                                ShippingAddress = new AddressModel()
                                {

                                    TypeOfAddress = AddressType.Shipping,
                                    StreetAddress = order.ShippingAddress,
                                    StreetAddress2 = order.ShippingAddress2,
                                    City = order.ShippingCity,
                                    Name = order.FirstName + ' ' + order.LastName,
                                    Phone = order.ShippingPhone,
                                    State = order.ShippingState,
                                    Zip = order.ShippingZip
                                }
                            },
                            AdminComments = order.AdminComments,
                            Affiliate = aff,
                            //AffiliateComments = order.AffiliateComments,
                            //CCUserDetails =
                            //    "None <a href=\"#AddCCModal\" role=\"button\" class=\"btn btn-mini\" data-toggle=\"modal\"> Add?</a> ", // CC user removed at request
                            DiscountModel = BuildDiscountModel(webUser),
                            DisplayOptionsInDropDownViewModel =
                                BuildDisplayOptionsInDropDownViewModel(row, idOrder),
                            DisplayRowPriceViewModel =
                                BuildDisplayRowPriceViewModel(row, idOrder,
                                    additionalLocationsViewModel.OptionsCost),
                            idUser = order.WebUser.idUser,
                            ShippingDetailsModel = new ShippingDetailsModel()
                            {
                                UserId = webUser.idUser,
                                ShippingAddress = shippingAddress == null
                                    ? new AddressModel()
                                    : new AddressModel
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
                                    }
                            },
                            OrderExists = row.Order != null,
                            AdditionalLocationsViewModel = additionalLocationsViewModel,
                            OrderRowExists = true,
                            OrderRowHasId = true,
                            OrderStatus = order.OrderStatus,
                            Origin = order.Origin,
                            UserComments = order.UserComments,
                            UserFullname = userFullName,
                            UserDetails = string.Concat("<span id='userFullnameLabel'>", userFullName,
                                "</span> - <span id='userInstitutionLabel'>", order.Institution, "</span><br>",
                                "<span id='userEmailLabel'>", webUser.email, "</span>"),

                            UserType = webUser.UserType
                        };

                        if (checkoutConfirmViewModel.OrderRowExists)
                        {
                            if (row.idOrder > 0)
                                checkoutConfirmViewModel.OrderRowHasId = true;

                            if (checkoutConfirmViewModel.OrderRowHasId)
                                checkoutConfirmViewModel.OptionLabel = row.RegistrationType.OptionLabel;
                        }

                        if (Request["referred"] != null &&
                            WebUtility.HtmlDecode(Request["referred"]) != "How did you hear about this webinar?")
                        {
                            order.Origin = Request["referred"] + Environment.NewLine + order.Origin;
                            //ViewData["referred"] = Request["referred"];
                        }


                        return checkoutConfirmViewModel;    
                    }
                    catch (Exception exception)
                    {
                        _logger.ErrorException("BuildCheckOutViewModel", exception);
                        Elmah.ErrorSignal.FromCurrentContext().Raise(exception);
                        throw;
                    }
                }
                else
                {
                    _logger.Fatal("Null or 0 order found while confirming order", new Exception("null or zero order "));
                }

            }
            _logger.Error("BuildCheckoutConfirmViewModel fell thru to Return Null");
            return null;
        }

        public ContinueShoppingModel BuildContinueShoppingModel(int? idWebinar)
        {
            Webinar webinar = LoadWebinar(idWebinar.Value);

            ContinueShoppingModel model = new ContinueShoppingModel
            {
                idWebinar = idWebinar.Value,
                //SearchTerm = 
                SelectedRelated = new List<Webinar>(),      // _webinarManagementService.GetRelated(idWebinar),
                SelectedPresenter = webinar.Presenter.WebUser.FullName,
                SelectedTopics = webinar.WebinarTopicXrefs
            };
            return model;

        }



        public DiscountModel BuildDiscountModel(WebUser currentUser)
        {
            var discountModel = new DiscountModel();
            var userDiscount = _orderManagementService.GetDiscountByUser(currentUser);
            if (ReferenceEquals(userDiscount, null))
                return null;
            _universalMapper.Map(userDiscount, discountModel);
            discountModel.DateValidFrom = userDiscount.DateValidFrom;
            discountModel.DateValidTo = userDiscount.DateValidTo;
            discountModel.RenewalTerm = userDiscount.RenewalTerm;
            discountModel.Status = userDiscount.Status;
            discountModel.Notes = userDiscount.Notes;

            discountModel.CreditsRemain = _orderManagementService.CalculateCreditsRemain(userDiscount);
            discountModel.CreditsUsed = _orderManagementService.CalculateCreditsUsed(userDiscount);
            discountModel.Cost = userDiscount.Cost;
            discountModel.DateBilled = userDiscount.DateBilled;
            discountModel.FlatOff = userDiscount.FlatOff;
            discountModel.PercentOff = userDiscount.PercentOff;
            discountModel.Status = userDiscount.Status;
            discountModel.DiscountCode = userDiscount.DiscountCode;
            discountModel.TotalCount = userDiscount.TotalCount;
            if (discountModel.TypeOfDiscount == DiscountType.Subscription &&
                discountModel.CreditsRemain < (decimal).25) return null;

            return discountModel;
        }

        public CheckoutOptionsViewModel BuildCheckoutOptionsViewModel(
            WebinarDetailsViewModel webinarDetailsViewModel,
            int? idWebinar,
            int? idOrderRow,
            int? idOrder)
        {
            Order order = null;

            if (ReferenceEquals(webinarDetailsViewModel, null))
            {
                var webinar = _webinarManagementService.GetWebinarByIdIncludingAllWebinarsByPresenter(idWebinar.Value);
                order = _orderManagementService.GetOrderById(idOrder.Value);

                webinarDetailsViewModel = new WebinarDetailsViewModel()
                {
                    Webinar = webinar,
                    WebinarFiles = webinar.WebinarFiles.ToList(),
                };
            }

            if (idWebinar.HasValue && idWebinar.Value > 0)
            {
                try
                {
                    _logger.Info("BuildCheckoutOptionsViewModel " + idOrder.Value);

                    var checkoutOptionsViewModel = new CheckoutOptionsViewModel
                    {
                        DisplayOptionsViewModel = new DisplayOptionsViewModel
                        {
                            EventTitle = webinarDetailsViewModel.Webinar.Title,
                            idWebinar = webinarDetailsViewModel.Webinar.idWebinar,
                            Options = _orderManagementService.GetOptionsByWebinarId(idWebinar.Value, false),
                            WebinarDuration = webinarDetailsViewModel.Webinar.Duration,
                            WebinarStatus = webinarDetailsViewModel.Webinar.Status
                        },

                        ConnectionInfoPresent = webinarDetailsViewModel.Webinar.ConnectionInfo != null,
                        WebinarDuration = webinarDetailsViewModel.Webinar.Duration,
                        idWebinar = webinarDetailsViewModel.Webinar.idWebinar,
                        idUser = webinarDetailsViewModel.WebUser != null ? webinarDetailsViewModel.WebUser.idUser : 0,
                        OrderExists = order != null,
                        WebinarStatus = webinarDetailsViewModel.Webinar.Status
                    };

                    if (checkoutOptionsViewModel.OrderExists)
                    {
                        checkoutOptionsViewModel.OrderHasId = order.idOrder > 0;

                        if (order.OrderRows != null)
                        {
                            checkoutOptionsViewModel.DisplayOptionsViewModel.OrderRowExists = true;
                            checkoutOptionsViewModel.DisplayOptionsViewModel.DisplayRowPriceViewModel =
                                BuildDisplayRowPriceViewModel(order.OrderRows.First(), idOrderRow);
                        }
                        else
                        {
                            checkoutOptionsViewModel.DisplayOptionsViewModel.DisplayRowPriceViewModel =
                                BuildDisplayRowPriceViewModel(null, idOrderRow);
                        }
                    }

                    return checkoutOptionsViewModel;

                }
                catch (Exception exception)
                {
                    _logger.ErrorException("BuildCheckoutOptionsViewModel", exception);
                    Elmah.ErrorSignal.FromCurrentContext().Raise(exception);
                    throw;
                }

            }
            // in real-world, this should never hit, correct?
            _logger.Error("BuildCheckoutOptionsViewModel fell thru too far!");
            return null;
        }

        public AdditionalLocationOfferViewModel BuildAdditionalLocationOfferViewModel(int idUser, int idWebinar)
        {
            var order = _orderManagementService.GetOrdersByUserId(idUser).FirstOrDefault();
            OrderRow orderRow = null;
            IEnumerable<AdditionalLocation> additionalLocations = Enumerable.Empty<AdditionalLocation>();

            if (!ReferenceEquals(null, order))
            {
                orderRow = order.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active);
                additionalLocations = orderRow.AdditionalLocation;
            }

            decimal priceOfAdditionalLocation = _orderManagementService.GetAdditionalLocationsPricing(idWebinar);

            var addAdditionalLocationViewModel = new AdditionalLocationOfferViewModel
            {
                AdditionalLocations = additionalLocations.ToList(),

                OrderExists = !ReferenceEquals(order, null),
                Emails = order == null ? new List<string>() : additionalLocations.Select(al => al.Email).ToList(),
                Price = priceOfAdditionalLocation,
                WebUser = order == null ? null : order.WebUser
            };

            return addAdditionalLocationViewModel;
        }

        public DisplayRowPriceViewModel BuildDisplayRowPriceViewModel(OrderRow orderRow, int? idOrderRow,
            decimal? optionsCost = null)
        {
            if (idOrderRow.HasValue && idOrderRow.Value > 0)
            {
                try
                {
                    if (ReferenceEquals(orderRow, null))
                        orderRow = _orderManagementService.GetOrderRowById(idOrderRow.Value);

                    if (orderRow.RowStatus != OrderRowStatus.Active)
                        return null;

                    _logger.Info("BuildDisplayRowPriceViewModel price for " + orderRow.Order.idOrder);

                    if (!optionsCost.HasValue)
                    {
                        optionsCost = _orderManagementService.GetAdditionalLocationsPricing(orderRow.idWebinar);
                    }

                    string addressesForAdditionalLocations = "";
                    if (orderRow.AdditionalLocation.Count > 0)
                    {
                        foreach (var addy in orderRow.AdditionalLocation)
                        {
                            if (_appHelper.CheckIsEmailValid(addy.Email.Trim()))
                                addressesForAdditionalLocations += addy.Email.Trim() + "<br>";
                        }

                        addressesForAdditionalLocations.Remove(addressesForAdditionalLocations.IndexOf('<'));

                    }

                    var displayRowPriceViewModel = new DisplayRowPriceViewModel
                    {
                        //Discount = orderRow.Discount,
                        NumberOfAdditionalLocations = orderRow.AdditionalLocation.Count(),
                        AddressesForAdditionalLocations = addressesForAdditionalLocations,
                        //OrderStatus = orderRow.Order.OrderStatus,
                        //Price = Convert.ToDecimal(orderRow.RegistrationType.Price),
                        PricesAndDiscounts =
                            _orderManagementService.CalculateOrderCost(orderRow.Order, optionsCost.Value),
                        //RowPrice = orderRow.RowPrice,
                        RegistrationType = orderRow.RegistrationType
                    };

                    _logger.Info("Returning BuildDisplayRowPriceViewModel price for " + orderRow.Order.idOrder);

                    return displayRowPriceViewModel;
                }
                catch (Exception exception)
                {
                    _logger.ErrorException("BuildDisplayRowPriceViewModel", exception);
                    Elmah.ErrorSignal.FromCurrentContext().Raise(exception);
                    throw;
                }
            }
            return null;
        }

        public WebinarDetailsViewModel BuildCheckOutViewModel(int? idOrderRow)
        {
            if (idOrderRow.HasValue && idOrderRow.Value > 0)
            {
                try
                {
                    var orderRow = _orderManagementService.GetOrderRowById(idOrderRow.Value);
                    var order = orderRow.Order;

                    var webUser = order.WebUser;
                    var webinar = _webinarManagementService.GetWebinar(order.OrderRows.FirstOrDefault().idWebinar);


                    _logger.Info("BuildCheckOutViewModel for " + orderRow.Order.idOrder);

                    var webinarDetailsViewModel = new WebinarDetailsViewModel
                    {
                        Order = order,
                        Webinar = webinar,
                        WebUser = webUser

                    };

                    //if (Request["referred"] != null &&
                    //    WebUtility.HtmlDecode(Request["referred"]) != "How did you hear about this webinar?")
                    //{
                    //    //
                    //    order.AdminComments += "Referred by: " + Request["referred"] + Environment.NewLine;
                    //    //ViewData["referred"] = Request["referred"];
                    //}
                    return webinarDetailsViewModel;
                }
                catch (Exception exception)
                {
                    _logger.ErrorException("BuildCheckOutViewModel", exception);
                    Elmah.ErrorSignal.FromCurrentContext().Raise(exception);
                    throw;
                }
            }
            //
            _logger.Error("BuildCheckOutViewModel fell thru too far!");
            return null;
        }

        public AdditionalLocationsViewModel BuildAdditionalLocationsViewModel(
            OrderRow orderRow,
            int? idOrderRow)
        {
            if (ReferenceEquals(orderRow, null))
                orderRow = _orderManagementService.GetOrderRowById(idOrderRow.Value);

            if (idOrderRow.HasValue && idOrderRow.Value > 0)
            {
                try
                {
                    _logger.Info("AdditionalLocationsViewModel building for: " + orderRow.Order.idOrder);
                    string lstAddLoc = "";
                    AdditionalLocation badAddLoc = new AdditionalLocation();
                    if (orderRow.AdditionalLocation != null && orderRow.AdditionalLocation.Count > 0)
                    {
                        foreach (var additionalLocation in orderRow.AdditionalLocation)
                        {
                            if (_appHelper.CheckIsEmailValid(additionalLocation.Email))
                            {
                                lstAddLoc += additionalLocation.Email + ",";
                            }
                            else
                            {
                                badAddLoc =
                                    orderRow.AdditionalLocation.SingleOrDefault(a => a.Email == additionalLocation.Email);

                            }
                        }
                        orderRow.AdditionalLocation.Remove(badAddLoc);
                    }

                    var additionalLocationsViewModel = new AdditionalLocationsViewModel
                    {
                        AdditionalLocations = orderRow.AdditionalLocation,
                        Addresses = lstAddLoc.TrimEnd(','),
                        OptionsCost = orderRow.Webinar.AdditionalLocationPrice
                    };

                    return additionalLocationsViewModel;
                }
                catch (Exception exception)
                {
                    _logger.ErrorException("BuildAdditionalLocationsViewModel", exception);
                    Elmah.ErrorSignal.FromCurrentContext().Raise(exception);
                    throw;
                }
            }
            _logger.Error("AdditionalLocationsViewModel fell thru too far!");
            return null;
        }

        public void CancelOrder(int idOrder)
        {
            _logger.Info("CancelOrder hit {0}", idOrder);
            _orderManagementService.DeleteOrder(orderId: idOrder);
        }

        public void SetOrderStatus(int idOrder, string loggedInEmail, OrderStatus orderStatus)
        {
            Order order = GetOrderById(idOrder);
            if (order.BillingEmail.ToLower() == loggedInEmail.ToLower())
            {
                order.OrderStatus = orderStatus;
                _orderManagementService.SaveChanges(); // less processing than SaveOrderChanges
            } // else log this attempt? may want some visibility in case admins ever end up here
        }

        public Tuple<string, string> CheckIfAddLocShouldHide(int optionId)
        {
            var firstOrDefault = _orderManagementService.GetRegTypeOption(optionId)
                .Select(o => new { Show = o.ShowLiveNotifications, Ship = o.ShowShippedNotifications })
                .FirstOrDefault();

            if (firstOrDefault != null)
            {
                if (firstOrDefault.Show == "Yes")
                {
                    _logger.Info(firstOrDefault.Show);
                }

                return new Tuple<string, string>(firstOrDefault.Show, firstOrDefault.Ship);
            }

            return null;
        }

        public Order CreateOrderExpress(CheckoutOptionsViewModel formModel)
        {
            _stateService.SetValue(DomainConstants.CheckoutInProcess, true);

            var webinar = _webinarManagementService.GetWebinar(formModel.idWebinar);

            if (webinar == null)
            {
                _logger.Error(string.Format("Null value for Webinar {0}", formModel.idWebinar));
                throw new NullReferenceException(string.Format("Null value for Webinar with id {0}", formModel.idWebinar));
            }
            try
            {
                var newOrderRow = CreateOrderRow(webinar,
                               formModel.AdditionalLocations == null ? null : formModel.AdditionalLocations.ToList(),
                               formModel.RegistrationTypeId);


                var currentAffiliate = _stateService.GetValue<Affiliate>("CurrentAffiliate");
                _orderManagementService.AttachAffiliate(currentAffiliate);

                // At this point, user may not be registered. So, when creating the Order, if user 
                // does not exist, a dummy user with an email of notauthenticated@cuwebinars.com will be created.
                WebUser webUser = formModel.SelectedWebUser > 0 ?
                    _orderManagementService.GetWebUserWithAddressAndInstitution(formModel.SelectedWebUser) : // if logged in as admin or affiliate
                    _orderManagementService.GetWebUserWithAddressAndInstitution(formModel.idUser);

                return CreateNewOrder(currentAffiliate, webUser, webinar, newOrderRow, null);
            }
            catch (Exception ex)
            {
                _logger.FatalException("CreateOrder", ex);
                return null;

            }
        }

        public Order CreateOrder(CheckoutOptionsViewModel formModel)
        {

            _stateService.SetValue(DomainConstants.CheckoutInProcess, true);
            Claim beingImpersonatedClaim = null;
            Order existingOrder = new Order();

            if (Request.IsAuthenticated)
            {
                var user = Request.RequestContext.HttpContext.User as ClaimsPrincipal;
                if (user != null && user.Identity.Name != null)
                {
                    existingOrder = _orderManagementService
                         .GetOrdersByEmail(user.Identity.Name, 19)
                         .FirstOrDefault(o => o.OrderRows.SingleOrDefault(r => r.RowStatus == OrderRowStatus.Active).idWebinar == formModel.idWebinar);

                    if (existingOrder != null)
                    {
                        _logger.Warn("CreateOrder found existing order: " + existingOrder.idOrder);
                        _stateService.SetValue(DomainConstants.FoundExistingOrder, existingOrder);
                    }
                }
                beingImpersonatedClaim = user.Claims.SingleOrDefault(c => c.Type == ClaimTypes.BeingImpersonated);
            }

            var webinar = _webinarManagementService.GetWebinar(formModel.idWebinar);

            if (webinar == null)
            {
                _logger.Error(string.Format("Null value for Webinar {0}", formModel.idWebinar));
                throw new NullReferenceException(string.Format("Null value for Webinar with id {0}", formModel.idWebinar));
            }

            try
            {
                var newOrderRow = CreateOrderRow(webinar,
                               formModel.AdditionalLocations == null ? null : formModel.AdditionalLocations.ToList(),
                               formModel.RegistrationTypeId);


                var currentAffiliate = _stateService.GetValue<Affiliate>("CurrentAffiliate");
                _orderManagementService.AttachAffiliate(currentAffiliate);

                // At this point, user may not be registered. So, when creating the Order, if user 
                // does not exist, a dummy user with an email of notauthenticated@cuwebinars.com will be created.
                WebUser webUser = formModel.SelectedWebUser > 0 ?
                    _orderManagementService.GetWebUserWithAddressAndInstitution(formModel.SelectedWebUser) : // if logged in as admin or affiliate
                    _orderManagementService.GetWebUserWithAddressAndInstitution(formModel.idUser);

                var order = CreateNewOrder(currentAffiliate, webUser, webinar, newOrderRow, beingImpersonatedClaim);

                if (formModel.CCEmail != null)
                {
                    var newJson = new JProperty(string.Concat(JsonPropertyKeys.CarbonCopy), formModel.CCEmail);
                    order.UserComments = JsonHelpers.MergeJsonWithStoredField(order.UserComments, newJson);
                    _orderManagementService.SaveChanges();
                }

                return order;
            }
            catch (Exception ex)
            {
                _logger.FatalException("CreateOrder", ex);
                return null;

            }
        }

        public Order CreateOrderByAffiliate(CheckoutOptionsViewModel formModel, Affiliate affiliate)
        {



            _stateService.SetValue(DomainConstants.CheckoutInProcess, true);
            Claim beingImpersonatedClaim = null;

            //if (Request.IsAuthenticated)
            //{
            var user = Request.RequestContext.HttpContext.User as ClaimsPrincipal;
            if (user != null)
                beingImpersonatedClaim = user.Claims.SingleOrDefault(c => c.Type == ClaimTypes.BeingImpersonated);

            //}

            var webinar = _webinarManagementService.GetWebinar(formModel.idWebinar);

            if (webinar == null)
            {
                _logger.Error(string.Format("Null value for Webinar {0}", formModel.idWebinar));
                throw new NullReferenceException(string.Format("Null value for Webinar with id {0}", formModel.idWebinar));
            }
            try
            {
                var newOrderRow = CreateOrderRow(webinar,
                               formModel.AdditionalLocations == null ? null
                                   : formModel.AdditionalLocations.ToList(),
                               formModel.RegistrationTypeId);

                var currentAffiliate = _stateService.GetValue<Affiliate>("CurrentAffiliate");
                //_orderManagementService.AttachAffiliate(currentAffiliate);

                var tempAff = _orderManagementService.GetAffiliateById(19);

                // At this point, user may not be registered. So, when creating the Order, if user 
                // does not exist, a dummy user with an email of notauthenticated@cuwebinars.com will be created.
                WebUser webUser = formModel.SelectedWebUser > 0 ?
                    _orderManagementService.GetWebUserWithAddressAndInstitution(formModel.SelectedWebUser) : // if logged in as admin or affiliate
                    _orderManagementService.GetWebUserWithAddressAndInstitution(formModel.idUser);

                var order = CreateNewOrder(tempAff, webUser, webinar, newOrderRow, beingImpersonatedClaim);

                order = _orderManagementService.AssignAffiliateToOrder(currentAffiliate.idUserAff, order);

                JProperty adminMsg = new JProperty(JsonPropertyKeys.AffiliateCheckout, JsonConvert.SerializeObject("Order created by: " + order.Affiliate.DisplayTitle + " - " + user.Identity.Name, Formatting.None,
                        new JsonSerializerSettings()
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        }));

                order.AdminComments = JsonHelpers.MergeJsonWithStoredField(order.AdminComments, adminMsg);
                order.AffiliateComments = JsonHelpers.MergeJsonWithStoredField(order.AffiliateComments, adminMsg);
                order.UserComments = JsonHelpers.MergeJsonWithStoredField(order.UserComments, adminMsg);

                _orderManagementService.SaveChanges();

                return order;
            }
            catch (Exception ex)
            {
                _logger.FatalException("CreateOrder", ex);
                return null;

            }
        }

        public ExpressCheckoutPostBackModel BuildExpressPostback(ExpressCheckoutPostBackModel form)
        {
            var user = _membershipService.GetUserByEmail(form.email5);

            if (!ReferenceEquals(null, user))
            {
                form.UserIsConfirmed = "yes";
            }

            try
            {
                var expressOrder = _orderManagementService.GetOrderById(Convert.ToInt32(form.orderid));

                if (!ReferenceEquals(expressOrder, null))
                {
                    var orderRow = expressOrder.OrderRows.Single(r => r.RowStatus == OrderRowStatus.Active);
                    var displayRowPriceViewModel = _orderManagementService.CalculateOrderCost(expressOrder, 0);

                    form.OrderIsConfirmed = "yes";
                    form.Order = expressOrder;
                    form.DisplayRowPriceViewModel = new DisplayRowPriceViewModel
                    {
                        PricesAndDiscounts = displayRowPriceViewModel,
                        //Discount = orderRow.Discount,
                        //NumberOfAdditionalLocations = additionalLocationsCount,
                        //OrderStatus = expressOrder.OrderStatus,
                        //Price = Convert.ToDecimal(orderRow.RegistrationType.Price),

                        RegistrationType = orderRow.RegistrationType,
                        //RowPrice = orderRow.RowPrice
                    };

                }

                //_orderManagementService.FireOrderSubmittedEvent(expressOrder, userCreatedByCheckout);

                //expressOrder.idOrderLegacy = _orderManagementService.SynchExpressCheckoutOrder(expressOrder);
                //_orderManagementService.SaveChanges();
                return form;

            }
            catch (Exception ex)
            {
                _logger.FatalException("ExpressPostback tossed:", ex);
            }
            return null;
        }

        public RegType FindRegType4ExpressPostback2(string idRegType, int q18QWebinarid18)
        {
            return _regTypeRepository.FindRegType4ExpressPostback2(idRegType, q18QWebinarid18);
        }

        public string GetDiscountCaption(Discount discount, OrderRow row, int? undo, int? previewOnly)
        {
            return _orderManagementService.CalculateDiscountRedemption(discount, row).Notes;

        }



        public decimal CalculateCreditsRemaining(Discount myDiscount)
        {
            return _orderManagementService.CalculateCreditsRemain(myDiscount);
        }


        public OrderRow GetOrderRowLoaded(int idOrderRow)
        {
            return _orderManagementService.GetOrderRowById(idOrderRow);
        }

        public IEnumerable<WebUser> GetWebUsersByLastNameForAffiliate(string lastName, int idAffiliate)
        {
            return _membershipService.GetWebUsersByLastNameForAffiliate(lastName, idAffiliate);
        }

        private Order CreateNewOrder(Affiliate affiliate, WebUser webUser, Webinar webinar, OrderRow orderRow, Claim createdByImpersonatedClaim = null)
        {
            try
            {
                if (ReferenceEquals(null, webUser))
                {
                    if (!_stateService.HasValue(WebUiConstants.SessionId))
                        _logger.Info("Session id is not in session.");
                    if (!_stateService.HasValue(WebUiConstants.SessionId))
                        _logger.Info("AValidInstitution is not in session.");

                    webUser = _membershipService.CreateWebUser(
                        _globals.Tenant,
                        "Not",
                        "Authenticated",
                        string.Empty,
                        string.Concat(_stateService.GetValue<string>(WebUiConstants.SessionId), "@notauthenticated.com"),
                        USTimeZone.Central,
                        UserType.Customer,
                        _stateService.GetValue<int>("AValidInstitution"),
                        null,
                        "Mr",
                        null,
                        null);
                }

                _logger.Info("WebUser id is {0}", webUser.idUser);


                var newOrder = _orderManagementService.CreateNewOrder(affiliate, webUser, webinar, orderRow);
                newOrder.AuditInfo = _appHelper.GetUserAuditInfo();
                newOrder.Origin = DomainConstants.Cart;

                if (!ReferenceEquals(createdByImpersonatedClaim, null))
                {
                    JProperty createdByImpersonatedUserMsg = new JProperty(
                        JsonPropertyKeys.OrderCreatedByImpersonatedUserKey,
                        createdByImpersonatedClaim.Value
                        );

                    newOrder.AdminComments = JsonHelpers.MergeJsonWithStoredField(newOrder.AdminComments, createdByImpersonatedUserMsg);
                }

                newOrder = _orderManagementService.SaveOrderChanges(newOrder, string.Empty, string.Empty);

                return newOrder;
            }
            catch (Exception ex)
            {
                _logger.FatalException("CreateNewOrder", ex);
                throw;
            }

        }

        public RegType GetRegTypeById(int idRegType)
        {
            return _orderManagementService.GetRegTypeOption(idRegType).SingleOrDefault();
        }


        public OrderRow LoadOrderRow(int id, OrderStatus status)
        {
            OrderRow row = _orderManagementService.LoadOrderRow(id);
            row.Order.OrderStatus = status;
            _orderManagementService.SaveOrderChanges(row.Order, null, null);

            return row;
        }

        public PricesAndDiscounts UpdateOrderPricing(Order order)
        {

            PricesAndDiscounts pricesAndDiscounts = _orderManagementService.CalculateOrderCost(order,
                 _orderManagementService.GetAdditionalLocationsPricing(order.OrderRows.SingleOrDefault(r => r.RowStatus == OrderRowStatus.Active).idWebinar));
            _orderManagementService.UpdateOrderChanges(order, ref pricesAndDiscounts);

            return pricesAndDiscounts;
        }

        public PricesAndDiscounts UpdateOrderPricingReadOnly(Order order)
        {
            PricesAndDiscounts pricesAndDiscounts = default(PricesAndDiscounts);
            pricesAndDiscounts = _orderManagementService.CalculateOrderCost(order,
                  _orderManagementService.GetAdditionalLocationsPricing(order.OrderRows.SingleOrDefault(r => r.RowStatus == OrderRowStatus.Active).idWebinar));
            //_orderManagementService.UpdateOrderChanges(order, ref pricesAndDiscounts);

            return pricesAndDiscounts;
        }

        public void FireOrderSubmittedNotification(Order order, bool? userCreatedInCart = null)
        {
            var notificationStorage = new NotificationStorage
            {
                idOrder = order.idOrder,
                SessionStartInfo = _appHelper.GetSessionStartInfo()
            };

            order.NotificationStorage = JsonConvert.SerializeObject(notificationStorage);



            if (userCreatedInCart.HasValue)
            {
                _orderManagementService.FireOrderSubmittedEvent(order, userCreatedInCart.Value, url: Request.Url);
            }
            else
            {
                _orderManagementService.FireOrderSubmittedEvent(order, url: Request.Url);
            }
        }

        public void FireOrderSubmittedMultiNotification(string toEmail, string subject, string notificationCopy)
        {
            _orderManagementService.FireOrderSubmittedMultiEvent(toEmail, subject, notificationCopy);
        }

        public void FireOrderSubmitted2Notification(string toEmail, string subject, string notificationCopy)
        {
            _orderManagementService.FireOrderSubmitted2Event(toEmail, subject, notificationCopy);
        }

        public void FireOrderSubmittedNotificationDES(string toEmail, string subject, string notificationCopy)
        {
            _orderManagementService.FireOrderSubmitted2Event(toEmail, subject, notificationCopy);
        }

        public void UpdateOrderWithUserId(int orderId, int userId)
        {
            _orderManagementService.UpdateOrderWithUserId(orderId, userId);
        }

        public string GetDiscountAmountAsPercentageOrDollarAmount(Discount myDiscount)
        {
            var amountToDiscount = myDiscount.FlatOff.ToString();

            if (myDiscount.PercentOff > 0)
            {
                amountToDiscount = myDiscount.PercentOff + "%";
            }

            return amountToDiscount;
        }

        public void UpdateAdditionalLocationsForOrderRow(IEnumerable<AdditionalLocation> additionalLocations, int newOrderRowId)
        {
            var existingAdditionalLocationsForOrderRow =
                _orderManagementService.GetAdditionalLocationsForOrderRow(newOrderRowId);

            if (additionalLocations == null)
            {
                foreach (var additionalLocationToDelete in
                    existingAdditionalLocationsForOrderRow)
                {

                    //_orderManagementService.RestoreToDiscount(newOrderRowId);
                    _orderManagementService.RemoveAndDeleteAdditionalLocation(additionalLocationToDelete);
                }
            }
            else
            {
                foreach (
                    var newSubmittedAdditionalLocation in
                        additionalLocations.Where(
                            al => !existingAdditionalLocationsForOrderRow.Select(eal => eal.Email).Contains(al.Email)))
                {
                    newSubmittedAdditionalLocation.idOrderRow = newOrderRowId;
                    //_orderManagementService.RemoveFromDiscount(newOrderRowId);
                    _orderManagementService.AddAdditionalLocation(newSubmittedAdditionalLocation);
                }

                foreach (var additionalLocationToDelete in
                    existingAdditionalLocationsForOrderRow.Where(
                        existingEmail => !additionalLocations.Select(al => al.Email).Contains(existingEmail.Email)))
                {

                    //_orderManagementService.RestoreToDiscount(newOrderRowId);
                    _orderManagementService.RemoveAndDeleteAdditionalLocation(additionalLocationToDelete);
                }
            }
            _orderManagementService.SaveChanges();
        }

        public Order LoadOrder(int id)
        {
            return _orderManagementService.GetOrderById(id);

        }

        public Webinar LoadWebinar(int idWebinar)
        {
            return _webinarManagementService.GetWebinar(idWebinar);
        }

        public Boolean UserHasPriorOrders(WebUser webUser)
        {
            var orders = _orderManagementService.GetOrdersByUserId(webUser.idUser);

            if (orders == null)
            {
                return false;
            }
            return true;
        }

        public Order GetOrderById(int? idOrder)
        {
            return _orderManagementService.GetOrderById(idOrder.Value);
        }

        public Discount GetDiscountById(int value)
        {
            return _orderManagementService.GetDiscountById(value);
        }

        public bool AssignWebUserToOrder(Order _order)
        {
            var succeeded = false;

            var order = _orderManagementService.AssignWebUserToOrder(_order.WebUser, _order);
            if (order != null)
            {
                succeeded = true;
            }

            return succeeded;

        }

        public WebUser GetWebUserByEmail(string email)
        {

            return _orderManagementService.GetWebUser(email);
        }

        public Affiliate GetAffiliateById(int affiliateId)
        {
            return _orderManagementService.GetAffiliateById(affiliateId);
        }

        public int CheckIfEmailAlreadyRegisteredForWebinar(int idWebinar, string email)
        {
            return _orderManagementService.CheckIfEmailAlreadyRegisteredForWebinar(idWebinar, email);
        }

        public bool UserHasMultipleEvents(int? id)
        {
            var orderRow = GetOrderRowLoaded(id.Value);
            var order = orderRow.Order;
            if (ReferenceEquals(order, null))
            {
                return false;
            }
            return _orderManagementService.UserHasMultipleEvents(order.idUser);
        }

        public string BuildOrderSubmitted2DESNotification(Order order)
        {

            OrderRow row = order.OrderRows.SingleOrDefault(r => r.RowStatus == OrderRowStatus.Active);

            Discount desSub = new Discount
            {
                DiscountType = DiscountType.DirectorSeries,
                DateValidTo = order.OrderDate.AddYears(1),
                DateValidFrom = order.OrderDate,
                Cost = row.RowPrice,
                DateBilled = DateTime.UtcNow,
                DateVerified = DateTime.Now,
                DiscountCode = "DES" + order.idOrder,
                FlatOff = 0,
                Notes = "V3 entry",
                PercentOff = 0,
                RenewalTerm = 1,
                Status = "Active",
                TotalCount = 1,
                idAffiliate = order.idAffiliate,
                idDiscount = order.idOrder

            };
            row.Discount = desSub;

            _orderManagementService.SaveOrderChanges(order, null, null);

            Webinar webinar = row.Webinar;
            DocumentModel document =
                DocumentModel.Load(
                    System.Web.HttpContext.Current.Server.MapPath(
                        @"~/App_Data/mergeTemplates/OrderSubmittedDES.docx"));


            string theOrderSummary = "";
            string regDesc = "";


            var dsMergeFields = new
            {
                AttendType = row.RegistrationType.OptionLabelShort,
                RegDesc = regDesc,
                TenantSignature = "The " + _globalConfig.Tenant + " Staff",
                OrderID = row.idOrder,
                BillingEmail = order.BillingEmail,
                TechSupportLink = "<a href='" + _globalConfig.TenantURL + "/oh/" + order.idOrder + "'>" + _globalConfig.TenantURL + "/oh/" + order.idOrder + "</a>",
                OndemandLink = "<a href='" + _globalConfig.TenantURL + "/o/" + order.idOrder + "-" + row.OnDemandCode + "'>" + _globalConfig.TenantURL + "/o/" + order.idOrder + "-" + row.OnDemandCode + "</a>",
                LinkToMyWebinars = "<a href='" + _globalConfig.TenantURL + "/MyWebinars?idOrder=" + order.idOrder + "'>" + _globalConfig.TenantURL + "/MyWebinars?idOrder=" + order.idOrder + "</a>",
                TenantName = _globalConfig.Tenant,
                WebinarTitle = webinar.Title,
                FirstName = order.FirstName

            };

            document.MailMerge.Execute(dsMergeFields);


            bool noError = true;
            try
            {
                if (noError)
                {
                    _logger.Info("BuildOrderSubmitted2DES begins: " + order.idOrder);

                    //// SAVE LOCALLY if needed for easier testing
                    //document.Save(System.Web.HttpContext.Current.Server.MapPath(@"~/App_Data/mergeTemplates/" + order.idOrder + ".pdf"), SaveOptions.PdfDefault);

                    var storageCredentials = new StorageCredentials(_globalConfig.StorageAccountName,
                        _globalConfig.StorageAccessKey);

                    var cloudStorageAccount = new CloudStorageAccount(storageCredentials, false);
                    CloudBlobClient blobClient = cloudStorageAccount.CreateCloudBlobClient();

                    // Retrieve reference to a previously created container.
                    CloudBlobContainer container = blobClient.GetContainerReference("desregistrations");
                    container.CreateIfNotExists();

                    CloudBlockBlob blob = container.GetBlockBlobReference(webinar.idWebinar + "/" + order.idOrder + ".pdf");

                    using (MemoryStream output = new MemoryStream())
                    {
                        document.Save(output, SaveOptions.PdfDefault);
                        output.Position = 0; // reset to beginning so Upload operation can work correctly
                        blob.UploadFromStream(output);
                    }

                    //blob = container.GetBlockBlobReference(webinar.idWebinar + "/" + order.idOrder + ".gif");
                    //using (MemoryStream output = new MemoryStream())
                    //{
                    //    document.Save(output, new ImageSaveOptions() { Format = ImageSaveFormat.Gif});
                    //    output.Position = 0; // reset to beginning so Upload operation can work correctly
                    //    blob.UploadFromStream(output);
                    //}

                    blob = container.GetBlockBlobReference(webinar.idWebinar + "/" + order.idOrder + ".htm");
                    using (MemoryStream output = new MemoryStream())
                    {
                        document.Save(output, new HtmlSaveOptions() { EmbedImages = true });
                        output.Position = 0; // reset to beginning so Upload operation can work correctly
                        blob.UploadFromStream(output);
                    }

                    byte[] fileContents;
                    using (MemoryStream output = new MemoryStream())
                    {
                        document.Save(output, SaveOptions.HtmlDefault);
                        output.Position = 0; // reset to beginning so Upload operation can work correctly

                        fileContents = output.ToArray();
                    }
                    _orderManagementService.FireMandrillNotificationEvent(ConfigurationManager.AppSettings["TestEmailAddress"], "Welcome To The Director Series", System.Text.Encoding.UTF8.GetString(fileContents));
                    return System.Text.Encoding.UTF8.GetString(fileContents);
                }
                else
                {
                    https://ci4.googleusercontent.com/proxy/AOF0zatzFSovHlWus8P1dHxNNFo0tLbt-mot0d9e-Of2y7-y9OixCjE7b48XZyxMDreHdAqWirQiZ5bnZNro7z99YsBEbeXmAtMPk4wXt_4cag5u=s0-d-e1-ft#http://devholmen15:3538/Content/images/vrLocal/left_shadow.jpg
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.ErrorException("BuildOrderSubmitted2DES for: " + order.idOrder, ex);
                return "Error: " + ex.Message;
            }

        }

        public string BuildOrderSubmitted2Notification(Order order)
        {
            OrderRow row = order.OrderRows.SingleOrDefault(r => r.RowStatus == OrderRowStatus.Active);

            Webinar webinar = row.Webinar;
            DocumentModel document =
                DocumentModel.Load(
                    System.Web.HttpContext.Current.Server.MapPath(
                        @"~/App_Data/mergeTemplates/OrderSubmitted_PreEvent.docx"));

            NotificationMessageFields fields = _appHelper.BuildNotiFields(order);
            var orderCC = _orderManagementService.OrderHasCc(order);
            if (orderCC != null)
            {
            }
            fields.AddLocsCost = " Please note that the per seat cost of an Additional Location is " +
                     _orderManagementService.GetAdditionalLocationsPricing(row.idWebinar).ToString("C0") + ".";

            document.MailMerge.Execute(fields);


            bool noError = true;
            try
            {
                if (noError)
                {
                    _logger.Info("BuildOrderSubmitted2NotificationMessage begins: " + order.idOrder);

                    var storageCredentials = new StorageCredentials(_globalConfig.StorageAccountName,
                        _globalConfig.StorageAccessKey);

                    var cloudStorageAccount = new CloudStorageAccount(storageCredentials, false);
                    CloudBlobClient blobClient = cloudStorageAccount.CreateCloudBlobClient();

                    // Retrieve reference to a previously created container.
                    CloudBlobContainer container = blobClient.GetContainerReference("order-submitted");
                    container.CreateIfNotExists();

                    CloudBlockBlob blob = container.GetBlockBlobReference(webinar.idWebinar + "/" + order.idOrder + ".pdf");

                    using (MemoryStream output = new MemoryStream())
                    {
                        document.Save(output, SaveOptions.PdfDefault);
                        output.Position = 0; // reset to beginning so Upload operation can work correctly
                        blob.UploadFromStream(output);


                    }


                    blob = container.GetBlockBlobReference(webinar.idWebinar + "/" + order.idOrder + ".htm");
                    using (MemoryStream output = new MemoryStream())
                    {
                        document.Save(output, new HtmlSaveOptions() { EmbedImages = true });
                        output.Position = 0; // reset to beginning so Upload operation can work correctly
                        blob.UploadFromStream(output);
                    }

                    byte[] fileContents;
                    string myString = "";
                    using (MemoryStream output = new MemoryStream())
                    {
                        document.Save(output, SaveOptions.HtmlDefault);
                        output.Position = 0; // reset to beginning so Upload operation can work correctly


                        output.Position = 0;
                        var sr = new StreamReader(output);
                        myString = sr.ReadToEnd();
                        fileContents = output.ToArray();
                    }

                    //return System.Text.Encoding.UTF8.GetString(fileContents);
                    return (myString);
                }
            }
            catch (Exception ex)
            {
                _logger.ErrorException("BuildOrderSubmitted2Noti for: " + order.idOrder, ex);
                return "Error: " + ex.Message;
            }

            _logger.Error("BuildOrderSubmitted2FloatedTooFar: " + order.idOrder);
            return "BuildOrderSubmitted2FloatedTooFar: " + order.idOrder;
        }

        public string InvoicedOrderIsUpdated(Order order)
        {
            return _orderManagementService.InvoicedOrderIsUpdated(order);
        }

        public void FireMandrillNotificationEvent(string emails, string subjectLine, string orderConfirmString)
        {
            _orderManagementService.FireMandrillNotificationEvent(ConfigurationManager.AppSettings["TestEmailAddress"], subjectLine, orderConfirmString);

        }

        public void SaveOrder(Order order)
        {
            _orderManagementService.SaveOrderChanges(order, null, null);
        }


        public List<Order> GetOrdersByUser(string loggedInEmail)
        {
            return _orderManagementService.GetOrdersByEmail(loggedInEmail, 19).ToList();
        }

        public ExpressCheckoutModel ExpressCheckout(Order order, WebUser user)
        {
            UpdateOrderWithUserId(order.idOrder, user.idUser);
            var row = order.OrderRows.Single(r => r.RowStatus == OrderRowStatus.Active);

            var addLocs = "";

            if (row.AdditionalLocation != null && row.AdditionalLocation.Any())
            {
                foreach (var loc in row.AdditionalLocation)
                {
                    addLocs = loc.Email + Environment.NewLine;
                }
            }
            string justNumbers = new String(order.BillingPhone.Where(Char.IsDigit).ToArray());
            var area = justNumbers.Substring(0, 3);
            var phone = justNumbers.Substring(3, 3) + "-" + justNumbers.Substring(6, 4);
            var formID = "60456422151952";
            if (_globalConfig.Tenant == "BankWebinars")
            {
                formID = "52205870745961"; //production
                //formID = "60463854157965"; //dev
            }

            var model = new ExpressCheckoutModel
            {
                formID = formID,
                q18_q_webinarid18 = row.Webinar.idWebinar,
                q15_affiliateid15 = order.idAffiliate,
                q12_webinarTitle = row.Webinar.Title,
                q11_orderid = order.idOrder,
                q9_title = user.Title,
                q14_address14 = new Q14Address14
                {
                    addr_line1 = order.BillingAddress,
                    addr_line2 = order.BillingAddress2,
                    state = order.BillingState,
                    city = order.BillingCity,
                    postal = order.BillingZip,
                    country = "United States"
                },
                q4_name = new Q4Name { first = order.FirstName, last = order.LastName },
                q8_institution = order.Institution,
                q6_phoneNumber6 = new Q6PhoneNumber6 { area = area, phone = phone },
                q5_email5 = order.BillingEmail,
                q19_additionalLocations19 = addLocs
            };

            if (order.WebUser.idSubscriptionDiscount.HasValue)
            {
                var discount =
                    GetDiscountById(order.WebUser.idSubscriptionDiscount.Value);
                model.q20_discountCode20 = discount.DiscountCode;
            }

            return model;
        }

        public RegType GetRegTypeByLabel(string livePlusFive, int? idWebinar)
        {
            return _orderManagementService.GetRegTypeByLabel(livePlusFive, idWebinar: idWebinar.Value);
        }

        public void AddClaimForPostEventMaterials(string email, OrderRow row)
        {
            if (email == null) throw new ArgumentNullException(@"email");
            if (row == null) throw new ArgumentNullException(@"row");
            var editingUser = "";
            if (Request.IsAuthenticated)
            {
                var user = Request.RequestContext.HttpContext.User;
                if (!ReferenceEquals(user, null)) editingUser = user.Identity.Name;
            }
            try
            {
                if (!OnDemandCodeIsUnique(row.OnDemandCode))
                {
                    var fromVal = row.OnDemandCode;
                    while (!OnDemandCodeIsUnique(row.OnDemandCode))
                    {
                        row.OnDemandCode = RandomHelpers.GetUniqueCode(5);
                    }
                    _logger.Warn("OnDemand Order Changed " + row.idOrder + " from: " + fromVal + " to: " + row.OnDemandCode + " by: " + editingUser);
                }

                _orderManagementService.SaveChanges();
                _membershipService.AddClaimForPostEventMaterials(email, row,
                    _orderManagementService.CalculatePostEventMaterialsAccessExpiry(row), _globalConfig.Tenant);
            }
            catch (Exception ex)
            {
                _logger.FatalException("AddClaimForPostEventMaterials: ", ex);
            }

        }

        private bool OnDemandCodeIsUnique(string onDemandCode)
        {
            return _orderManagementService.OnDemandCodeIsUnique(onDemandCode);
        }

        public IList<Order> GetOrderByUserIdAndWebinar(int selectedWebUser, int idWebinar)
        {
            return
                _orderManagementService.GetOrdersByUserId(selectedWebUser)
                    .Where(o => o.OrderRows.SingleOrDefault(r => r.RowStatus == OrderRowStatus.Active)
                        .Webinar.idWebinar == idWebinar).ToList();
        }

        public string InsertOnDemandClaim(int orderId)
        {
            var order = GetOrderById(orderId);

            try
            {
                AddClaimForPostEventMaterials(order.BillingEmail, order.OrderRows.Single(r => r.RowStatus == OrderRowStatus.Active));
            }
            catch (Exception ex)
            {
                _logger.FatalException("InsertOnDemandClaim", ex);
            }

            var result = _globalConfig.TenantURL + "/o/" + orderId + "-" + order.OrderRows.Single(r => r.RowStatus == OrderRowStatus.Active).OnDemandCode;
            return result;
        }


        public INotificationMessage GenerateMessagePreview(Order order)
        {
            var orderSubmittedViewModel = new OrderSubmittedViewModel
            {
                AddPasswordUrl = string.Empty,
                ConfirmChangeEmailUrl = string.Empty,
                Order = order,
                OrderGenesis = OrderGenesis.CreatedViaCartByExistingUser,
                UserCreatedInCart = false,
                UserCreatedOnImport = false
            };

            return _generalFormatter.Format(orderSubmittedViewModel, "OrderSubmitted");
        }

        public void AdjustUserDetails(AdjustUserDetailsEditModel adjustUserDetailsEditModel)
        {
            var webUser = _membershipService.GetWebUserById(adjustUserDetailsEditModel.idUser);

            webUser.FirstName = adjustUserDetailsEditModel.FirstName;
            webUser.LastName = adjustUserDetailsEditModel.LastName;
            webUser.email = adjustUserDetailsEditModel.Email;

            _membershipService.UpdateUserDetails(webUser);
        }

        public Discount ApplyDiscountCode(string code, OrderRow row)
        {
            var discount = _orderManagementService.ApplyDiscountCode(code, row);


            if (!ReferenceEquals(discount, null))
            {
                _orderManagementService.CalculateOrderCost(row.Order, _orderManagementService.GetAdditionalLocationsPricing(row.idWebinar));
                _orderManagementService.SaveChanges();
            }
            return discount;


        }
        public Discount CheckDiscountCode(int idDiscount)
        {
            return _orderManagementService.GetDiscountById(idDiscount);
        }

        public void RemoveAdditionalLocationsFromOrder(int idOrderRow)
        {
            _orderManagementService.RemoveAdditionalLocationsForOrder(idOrderRow);
        }

        private OrderRow CreateOrderRow(Webinar webinar, IList<AdditionalLocation> additionalLocations,
            int registrationType)
        {
            var orderRow = _orderManagementService.CreateOrderRow(webinar, additionalLocations, registrationType);

            return orderRow;
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
                _webinarManagementService.Dispose();

                _disposed = true;
            }
        }
    }
}