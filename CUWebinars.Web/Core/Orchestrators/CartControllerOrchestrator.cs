using System.Collections.Generic;
using System.Text;
using CUWebinars.Business.AccountService;
using CUWebinars.Business.Models;
using CUWebinars.Business.Services;
using CUWebinars.Web.Helpers;
using CUWebinars.Web.Models;
using CUWebinars.Web.Services;
using CUWebinars.Web.ViewModel;
using Ninject.Extensions.Logging;
using System;
using System.Linq;
using System.Net;
using System.Web;

namespace CUWebinars.Web.Core.Orchestrators
{
    public class CartControllerOrchestrator : ICartControllerOrchestrator
    {
        public HttpRequest Request { get; set; }
        private readonly IStateService _stateService;

        private readonly IMembershipService _membershipService;
        private readonly ILogger _logger;
        private readonly IOrderManagementService _orderManagementService;
        private readonly IWebinarManagementService _webinarManagementService;
        private bool _disposed;

        public CartControllerOrchestrator(IMembershipService membershipService,
            IOrderManagementService orderManagementService,
            IWebinarManagementService webinarManagementService,
            IStateService stateService,
            ILogger logger,
            HttpRequest request)
        {
            Request = request;
            _membershipService = membershipService;
            _logger = logger;
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
            if (ReferenceEquals(orderRow, null))
                orderRow = _orderManagementService.GetOrderRowById(idOrderRow.Value);

            if (idOrderRow.HasValue && idOrderRow.Value > 0)
            {
                try
                {
                    var displayOptionsInDropDownViewModel = new DisplayOptionsInDropDownViewModel
                    {
                        //Options = null, // todo: this could be sent to the server from the client. Already know the options. No need hit database.
                        Options = new List<RegType>(),
                        OrderRowId = idOrderRow.Value,
                        OrderRowRegistrationType = orderRow.RegistrationType
                    };

                    return displayOptionsInDropDownViewModel;
                }
                catch (Exception exception)
                {
                    _logger.ErrorException("BuildCheckOutViewModel", exception);
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

                    var checkoutConfirmViewModel = new CheckoutConfirmViewModel
                    {
                        AdditionalLocationCaption = string.Empty,
                        AdminComments = orderRow.Order.AdminComments,
                        AffiliateComments = orderRow.Order.AffiliateComments,
                        CCUserDetails =
                            "None <a href=\"#AddCCModal\" role=\"button\" class=\"btn btn-mini\" data-toggle=\"modal\"> Add?</a> ",
                        DisplayOptionsInDropDownViewModel = BuildDisplayOptionsInDropDownViewModel(orderRow, idOrderRow),
                        DisplayRowPriceViewModel = BuildDisplayRowPriceViewModel(orderRow, idOrderRow),
                        idUser = orderRow.Order.WebUser.idUser,
                        OrderExists = orderRow.Order != null,
                        OrderHasAdditionalLocationsViewModel =
                            BuildOrderHasAdditionalLocationsViewModel(orderRow, idOrderRow),
                        OrderRowExists = true,
                        OrderRowHasId = true,
                        OrderStatus = orderRow.Order.OrderStatus,
                        Origin = orderRow.Order.Origin,
                        UserComments = orderRow.Order.UserComments,
                        UserDetails =
                            webUser.FirstName + " " + webUser.LastName + " - " + orderRow.Order.Institution + "<br>" +
                            webUser.email,
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
                                " - <a role=\"button\" class=\"btn btn-mini\" target=\"new\" href='/account/manage/" +
                                webUser.idUser + "' > Edit?</a>";
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
                    Affiliate = _stateService.GetValue<Affiliate>("CurrentAffiliate"),
                    Webinar = webinar,
                    WebinarFiles = webinar.WebinarFiles.ToList(),
                    Options = _orderManagementService.GetOptionsByWebinarId(idWebinar.Value, false),
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
                                Options = webinarDetailsViewModel.Options,
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
                        throw;
                    }

                }
            
            return null;
        }

        public DisplayRowPriceViewModel BuildDisplayRowPriceViewModel(OrderRow orderRow, int? idOrderRow)
        {
            if (ReferenceEquals(orderRow,null))
                orderRow = _orderManagementService.GetOrderRowById(idOrderRow.Value);

            if (orderRow.RowStatus != OrderRowStatus.Active)
                return null;

            if (idOrderRow.HasValue && idOrderRow.Value > 0)
            {
                try
                {
                    _logger.Info("Building ");

                    var displayRowPriceViewModel = new DisplayRowPriceViewModel
                    {
                        Discount = orderRow.Discount,
                        NumberOfAdditionalLocations = orderRow.AdditionalLocation.Count(),
                        OrderStatus = orderRow.Order.OrderStatus,
                        Price = orderRow.RegistrationType.Price,
                        RowPrice = orderRow.RowPrice,
                        RegistrationType = orderRow.RegistrationType
                    };

                    return displayRowPriceViewModel;
                }
                catch (Exception exception)
                {
                    _logger.ErrorException("BuildDisplayRowPriceViewModel", exception);
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
                    _logger.Info("Building ");
                    var orderRow = _orderManagementService.GetOrderRowById(idOrderRow.Value);
                    var order = orderRow.Order;
                    var additionalLocations = orderRow.AdditionalLocation.ToList();
                    var webUser = order.WebUser;
                    var webinar = orderRow.Webinar;

                    var webinarDetailsViewModel = new WebinarDetailsViewModel
                    {
                        Affiliate = order.Affiliate,
                        Order = order,
                        Webinar = webinar,
                        WebUser = webUser
                    };

                    if (Request["referred"] != null &&
                        WebUtility.HtmlDecode(Request["referred"]) != "How did you hear about this webinar?")
                    {
                        order.Origin = Request["referred"] + Environment.NewLine + order.Origin;
                        //ViewData["referred"] = Request["referred"];
                    }

                    return webinarDetailsViewModel;
                }
                catch (Exception exception)
                {
                    _logger.ErrorException("BuildCheckOutViewModel", exception);
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
                    _logger.Info("Building ");

                    var addressesAndOptionsCost = GetAddressesAndOptionsCost(orderRow.AdditionalLocation);

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
                    throw;
                }
            }
            return null;
        }

        private Tuple<string, int> GetAddressesAndOptionsCost(IEnumerable<AdditionalLocation> additionalLocations)
        {
            StringBuilder addresses = new StringBuilder();
            int optionsCost = 0;
            var i = 0;

            var additionalLocationsEnumerated = additionalLocations as AdditionalLocation[] ?? additionalLocations.ToArray(); // ensures only enumerated once

            foreach (var additionalLocation in additionalLocationsEnumerated)
            {
                i++;
                if (i == additionalLocationsEnumerated.Count())
                {
                    addresses.Append(additionalLocation.Email);
                }
                if (i < additionalLocationsEnumerated.Count())
                {
                    if (i == additionalLocationsEnumerated.Count() - 1)
                    {
                        addresses.Append(additionalLocation.Email + " and ");
                    }
                    else
                    {
                        addresses.Append(additionalLocation.Email + ", ");
                    }
                }

                optionsCost = optionsCost + Convert.ToInt32(additionalLocation.Price);
            }

            return new Tuple<string, int>(addresses.ToString(), optionsCost);
        }

        public string CheckIfAddLocShouldHide(int optionId)
        {
            var firstOrDefault =  _orderManagementService.GetRegTypesForOption(optionId)
                .Select(o => o.ShowLiveNotifications)
                .FirstOrDefault();

            if (firstOrDefault != null)
            {
                if (firstOrDefault == "Yes")
                {
                    _logger.Info(firstOrDefault);
                }

                return firstOrDefault;
            }

            return null;
        }

        public Order CreateOrder(CheckoutOptionsViewModel formModel, string stageOfCheckout)
        {
            var webinar = _webinarManagementService.GetWebinar(formModel.idWebinar);
            var newOrderRow = CreateOrderRow(webinar, formModel.RegistrationTypeId);

            var currentAffiliate = _stateService.GetValue<Affiliate>("CurrentAffiliate");
            _orderManagementService.AttachAffiliate(currentAffiliate);

            var webUser = _orderManagementService.GetWebUser(formModel.idUser);

            return CreateNewOrder(currentAffiliate, webUser, webinar, newOrderRow);
        }

        public OrderRow LoadOrderRow(int id, OrderStatus status)
        {
            OrderRow row = _orderManagementService.LoadOrderRow(id);
            row.Order.OrderStatus = status;
            _orderManagementService.SaveOrderChanges(row.Order, null, null);

            return row;
        }

        private Order CreateNewOrder(Affiliate affiliate, WebUser webUser, Webinar webinar, OrderRow orderRow)
        {

            var newOrder = _orderManagementService.CreateNewOrder(affiliate, webUser, webinar, orderRow);
            newOrder.AuditInfo = AppHelper.GetUserAuditInfo();
            newOrder.Origin = _orderManagementService.GetOrderInitiator();

            newOrder = _orderManagementService.SaveOrderChanges(newOrder, string.Empty, string.Empty);

            return newOrder;
        }

        private OrderRow CreateOrderRow(Webinar webinar, int registrationType)
        {
            //var currentOrder = _stateService.GetValue<Order>("CurrentOrder");
            // let's see if we can avoid the need for Session Var

            var orderRow = _orderManagementService.CreateOrderRow(webinar, null, registrationType);

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