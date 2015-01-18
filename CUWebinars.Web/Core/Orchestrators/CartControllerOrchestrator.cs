using CUWebinars.Business.AccountService;
using CUWebinars.Business.Constants;
using CUWebinars.Business.Core;
using CUWebinars.Business.Models;
using CUWebinars.Business.Services;
using CUWebinars.Web.Helpers;
using CUWebinars.Web.Models;
using CUWebinars.Web.Services;
using CUWebinars.Web.ViewModel;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

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
        private readonly IOrderManagementService _orderManagementService;
        private readonly IWebinarManagementService _webinarManagementService;
        private bool _disposed;

        public CartControllerOrchestrator(IMembershipService membershipService,
            IOrderManagementService orderManagementService,
            IWebinarManagementService webinarManagementService,
            IStateService stateService,
            ILogger logger,
            HttpRequestBase request,
            IAppHelper appHelper)
        {
            Request = request;
            _membershipService = membershipService;
            _logger = logger;
            _appHelper = appHelper;
            _orderManagementService = orderManagementService;
            _webinarManagementService = webinarManagementService;
            _stateService = stateService;
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

        public CheckoutConfirmViewModel BuildCheckoutConfirmViewModel(int? idOrderRow)
        {
            if (idOrderRow.HasValue && idOrderRow.Value > 0)
            {
                try
                {
                    _logger.Info("Building ");
                    var orderRow = _orderManagementService.GetOrderRowById(idOrderRow.Value);
                    var webUser = orderRow.Order.WebUser;
                    var userFullName = string.Concat(webUser.FirstName, " ", webUser.LastName);

                    var addresses = webUser.Addresses.ToArray();
                    var billingAddress = addresses.First(a => a.AddressType == WebUiConstants.BillingAddress);
                    var shippingAddress = addresses.FirstOrDefault(a => a.AddressType == WebUiConstants.ShippingAddress);

                    // This ViewModel is built here because it is re-used.
                    var orderHasAdditionalLocationsViewModel =
                        BuildOrderHasAdditionalLocationsViewModel(orderRow, idOrderRow);

                    var checkoutConfirmViewModel = new CheckoutConfirmViewModel
                    {
                        AdditionalLocationCaption = DomainHelpers.BuildAdditionalLocationsCaption(orderRow),
                        AdminComments = orderRow.Order.AdminComments,
                        AffiliateComments = orderRow.Order.AffiliateComments,
                        //CCUserDetails =
                        //    "None <a href=\"#AddCCModal\" role=\"button\" class=\"btn btn-mini\" data-toggle=\"modal\"> Add?</a> ", // CC user removed at request
                        DisplayOptionsInDropDownViewModel = BuildDisplayOptionsInDropDownViewModel(orderRow, idOrderRow),
                        DisplayRowPriceViewModel =
                            BuildDisplayRowPriceViewModel(orderRow, idOrderRow,
                                orderHasAdditionalLocationsViewModel.OptionsCost),
                        idUser = orderRow.Order.WebUser.idUser,
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
                        OrderExists = orderRow.Order != null,
                        OrderHasAdditionalLocationsViewModel = orderHasAdditionalLocationsViewModel,
                        OrderRowExists = true,
                        OrderRowHasId = true,
                        OrderStatus = orderRow.Order.OrderStatus,
                        Origin = orderRow.Order.Origin,
                        UserComments = orderRow.Order.UserComments,
                        UserFullname = userFullName,
                        UserDetails =
                            string.Concat(userFullName, " - ", orderRow.Order.Institution, "<br>", webUser.email),
                        UserType = webUser.UserType
                    };

                    if (checkoutConfirmViewModel.OrderRowExists)
                    {
                        if (orderRow.idOrderRow > 0)
                            checkoutConfirmViewModel.OrderRowHasId = true;

                        if (checkoutConfirmViewModel.OrderRowHasId)
                            checkoutConfirmViewModel.OptionLabel = orderRow.RegistrationType.OptionLabel;

                        if (orderRow.Order.OrderStatus == OrderStatus.InProcess)
                            checkoutConfirmViewModel.UserDetails +=
                                " - <a id='editUserDetails' role='button' class='btn btn-mini' target='new'> Edit?</a>";
                    }

                    if (Request["referred"] != null &&
                        WebUtility.HtmlDecode(Request["referred"]) != "How did you hear about this webinar?")
                    {
                        orderRow.Order.Origin = Request["referred"] + Environment.NewLine + orderRow.Order.Origin;
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
            return null;
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
                    _logger.Info("Building ");

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
                        AdditionalLocations = null, //TODO: come back to
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

            return null;
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

                    _logger.Info("Building price for " + orderRow.Order.idOrder);

                    if (!optionsCost.HasValue)
                    {
                        var dataOperations =
                            new Business.Core.DataOperations(GlobalConfig.GlobalConfigSingleton.DefaultConnectionString);
                        var additionalLocationsPricing = dataOperations.GetAdditionalLocationsPricing(orderRow.idWebinar);
                        optionsCost = additionalLocationsPricing.Single().Item2;
                    }

                    var displayRowPriceViewModel = new DisplayRowPriceViewModel
                    {
                        Discount = orderRow.Discount,
                        NumberOfAdditionalLocations = orderRow.AdditionalLocation.Count(),
                        OrderStatus = orderRow.Order.OrderStatus,
                        Price = orderRow.RegistrationType.Price,
                        PricesAndDiscounts =
                            _orderManagementService.CalculateOrderCost(orderRow.Order, optionsCost.Value),
                        RowPrice = orderRow.RowPrice,
                        RegistrationType = orderRow.RegistrationType
                    };

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
                    var additionalLocations = orderRow.AdditionalLocation.ToList();
                    var webUser = order.WebUser;
                    var webinar = orderRow.Webinar;

                    _logger.Info("Building BuildCheckOutViewModel for " + orderRow.Order.idOrder);

                    var webinarDetailsViewModel = new WebinarDetailsViewModel
                    {
                        Order = order,
                        Webinar = webinar,
                        WebUser = webUser
                    };

                    if (Request["referred"] != null &&
                        WebUtility.HtmlDecode(Request["referred"]) != "How did you hear about this webinar?")
                    {
                        //TODO: Determine the conditions that nessitate that this question be answered

                        order.AdminComments += "Referred by: " + Request["referred"] + Environment.NewLine;
                        //ViewData["referred"] = Request["referred"];
                    }
                    return webinarDetailsViewModel;
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

        public OrderHasAdditionalLocationsViewModel BuildOrderHasAdditionalLocationsViewModel(
            OrderRow orderRow,
            int? idOrderRow)
        {
            if (ReferenceEquals(orderRow, null))
                orderRow = _orderManagementService.GetOrderRowById(idOrderRow.Value);

            if (idOrderRow.HasValue && idOrderRow.Value > 0)
            {
                try
                {
                    _logger.Info("Building OrderHasAdditionalLocationsViewModel for: " + orderRow.Order.idOrder);

                    var addressesAndOptionsCost =
                        _orderManagementService.GetCostOfAdditionalLocations(orderRow.AdditionalLocation,
                            orderRow.idWebinar);

                    var orderHasAdditionalLocationsViewModel = new OrderHasAdditionalLocationsViewModel
                    {
                        AdditionalLocations = orderRow.AdditionalLocation,
                        Addresses = addressesAndOptionsCost.Item1,
                        OptionsCost = addressesAndOptionsCost.Item2
                    };

                    return orderHasAdditionalLocationsViewModel;
                }
                catch (Exception exception)
                {
                    _logger.ErrorException("BuildOrderHasAdditionalLocationsViewModel", exception);
                    Elmah.ErrorSignal.FromCurrentContext().Raise(exception);
                    throw;
                }
            }
            return null;
        }

        public void CancelOrder(int idOrder)
        {
            _orderManagementService.DeleteOrder(orderId: idOrder);
        }

        public Tuple<string, string> CheckIfAddLocShouldHide(int optionId)
        {
            var firstOrDefault = _orderManagementService.GetRegTypeOption(optionId)
                .Select(o => new {Show = o.ShowLiveNotifications, Ship = o.ShowShippedNotifications})
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

        public Order CreateOrder(CheckoutOptionsViewModel formModel)
        {
            _stateService.SetValue(DomainConstants.CheckoutInProcess, true);

            var webinar = _webinarManagementService.GetWebinar(formModel.idWebinar);
            var newOrderRow = CreateOrderRow(webinar,
                formModel.AdditionalLocations == null ? null : formModel.AdditionalLocations.ToList(),
                formModel.RegistrationTypeId);

            var currentAffiliate = _stateService.GetValue<Affiliate>("CurrentAffiliate");
            _orderManagementService.AttachAffiliate(currentAffiliate);

            // At this point, user may not be registered. So, when creating the Order, if user 
            // does not exist, a dummy user with an email of notauthenticated@cuwebinars.com will be created.
            var webUser = _orderManagementService.GetWebUser(formModel.idUser);

            return CreateNewOrder(currentAffiliate, webUser, webinar, newOrderRow);
        }

        public IEnumerable<WebUser> GetWebUsersByLastName(string lastName)
        {
            return _membershipService.GetWebUsersByLastName(lastName);
        }

        private Order CreateNewOrder(Affiliate affiliate, WebUser webUser, Webinar webinar, OrderRow orderRow)
        {
            if (ReferenceEquals(null, webUser))
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


            var newOrder = _orderManagementService.CreateNewOrder(affiliate, webUser, webinar, orderRow);
            newOrder.AuditInfo = _appHelper.GetUserAuditInfo();
            newOrder.Origin = DomainConstants.Cart;

            newOrder = _orderManagementService.SaveOrderChanges(newOrder, string.Empty, string.Empty);

            return newOrder;
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
            PricesAndDiscounts pricesAndDiscounts = default(PricesAndDiscounts);
            _orderManagementService.UpdateOrderChanges(order, ref pricesAndDiscounts);

            return pricesAndDiscounts;
        }

        public void FireOrderSubmittedNotification(Order order, bool? userCreatedInCart = null)
        {
            if (userCreatedInCart.HasValue)
                _orderManagementService.FireOrderSubmittedEvent(order, userCreatedInCart.Value, Request.Url);
            else
                _orderManagementService.FireOrderSubmittedEvent(order);
        }

        public void UpdateOrderWithUserId(int orderId, int userId)
        {
            _orderManagementService.UpdateOrderWithUserId(orderId, userId);
        }

        public Discount ApplyDiscountCode(string code, OrderRow row)
        {
           return _orderManagementService.ApplyDiscountCode(code, row);
        }

        public void RemoveAdditionalLocationsFromOrder(int idOrderRow)
        {
            _orderManagementService.RemoveAdditionalLocationsForOrder(idOrderRow);
        }

        private int GetDummyUserId()
        {
            int userId;
            if (int.TryParse(_globals.UnAuthenticatedUser, out userId))
                return userId;

            throw new FormatException("Value in AppSetting in Web.config must be a valid integer.");
        }

        private OrderRow CreateOrderRow(Webinar webinar, IList<AdditionalLocation> additionalLocations,
            int registrationType)
        {
            //var currentOrder = _stateService.GetValue<Order>("CurrentOrder");
            // let's see if we can avoid the need for Session Var

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