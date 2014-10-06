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
        readonly IStateService _stateService;

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

        public Order CreateOrder(WebinarDetailsViewModel formModel, string stageOfCheckout, string registrationType)
        {
            var newOrderRow = CreateOrderRow(formModel, stageOfCheckout, registrationType);

            return CreateNewOrder(formModel.Affiliate, formModel.WebUser, formModel.Webinar, newOrderRow);
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

        private OrderRow CreateOrderRow(WebinarDetailsViewModel formModel, string stageOfCheckout, string registrationType)
        {
            formModel.CheckoutInProcess = true;
            //var currentOrder = _stateService.GetValue<Order>("CurrentOrder");
            // let's see if we can avoid the need for Session Var


            var currentAffiliate = _stateService.GetValue<Affiliate>("CurrentAffiliate");

            formModel.Order = new Order();
            formModel.Affiliate = currentAffiliate;
            formModel.Webinar = _webinarManagementService.GetWebinar(formModel.Webinar.idWebinar);

            _orderManagementService.AttachAffiliate(formModel.Affiliate);

            try
            {
                formModel.WebUser = _orderManagementService.GetWebUser(formModel.WebUser.idUser);
            }
            catch
            {
                _logger.InfoException("no valid WebUser found.", new Exception());
            }

            //var options = _orderManagementService.GetOptionsByWebinarIdFromOptionsRepository(formModel.Webinar.idWebinar, true);
            var options = _orderManagementService.GetOptionsByWebinarId(formModel.Webinar.idWebinar, true);
            formModel.Options = options;

            //IList<AdditionalLocation> addLoc = new

            // replace AdditionalLocations handling from scratch

            var orderRow = _orderManagementService.CreateOrderRow(formModel.Webinar, null, int.Parse(registrationType));

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