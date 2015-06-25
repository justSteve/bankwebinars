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
using System.Linq;
using System.Net;
using System.Web;
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
        private bool _disposed;
        private GlobalConfig _globalConfig = GlobalConfig.GlobalConfigSingletonCreator.UniqueInstance;

        public CartControllerOrchestrator(IMembershipService membershipService,
            IOrderManagementService orderManagementService,
            IWebinarManagementService webinarManagementService,
            IStateService stateService,
            ILogger logger,
            HttpRequestBase request,
            IAppHelper appHelper,
            IFormatter generalFormatter)
        {
            Request = request;
            _membershipService = membershipService;
            _logger = logger;
            _appHelper = appHelper;
            _generalFormatter = generalFormatter;
            _orderManagementService = orderManagementService;
            _webinarManagementService = webinarManagementService;
            _stateService = stateService;
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

        public CheckoutConfirmViewModel BuildCheckoutConfirmViewModel(int? idOrderRow)
        {
            if (idOrderRow.HasValue && idOrderRow.Value > 0)
            {
                try
                {
                    _logger.Info("BuildCheckoutConfirmViewModel idOrderRow: " + idOrderRow.Value);
                    var orderRow = _orderManagementService.GetOrderRowById(idOrderRow.Value);
                    var order = orderRow.Order;
                    _logger.Info("BuildCheckoutConfirmViewModel idOrder: " + order.idOrder);
                    var webUser = orderRow.Order.WebUser;
                    var userFullName = string.Concat(webUser.FirstName, " ", webUser.LastName);

                    var addresses = webUser.Addresses.ToArray();
                    var billingAddress = addresses.First(a => a.AddressType == WebUiConstants.BillingAddress);
                    var shippingAddress = addresses.FirstOrDefault(a => a.AddressType == WebUiConstants.ShippingAddress);
                    if (!ReferenceEquals(null, order))
                    {

                        JObject existingJObject = null;

                        string comments = string.Empty;


                        if (!ReferenceEquals(null, order.AdminComments))
                        {
                            comments = order.AdminComments.Trim();
                        }

                        var newJson =
                            new JProperty(
                                string.Concat("LegacyCommentsFromCheckout-", TtsConfig.UtcNowAsCts.ToString(DomainConstants.DateTimeLongFormat)),
                                    new JObject(new JProperty("LegacyComments", order.AdminComments))
                                );

                        if (string.IsNullOrWhiteSpace(comments))
                        {
                            existingJObject = new JObject(newJson);
                        }
                        else
                        {
                            existingJObject = JObject.Parse(comments);
                            existingJObject.Add(newJson);
                        }

                        order.AdminComments = existingJObject.ToString(Formatting.None);
                    }
                    // This ViewModel is built here because it is re-used.
                    var orderHasAdditionalLocationsViewModel =
                        BuildOrderHasAdditionalLocationsViewModel(orderRow, idOrderRow);

                    var checkoutConfirmViewModel = new CheckoutConfirmViewModel
                    {
                        AdditionalLocationCaption = DomainHelpers.BuildAdditionalLocationsCaption(orderRow),
                        AdjustUserDetailsPanel = new AdjustUserDetailsEditModel
                        {
                            Email = webUser.email,
                            FirstName = webUser.FirstName,
                            idUser = webUser.idUser,
                            LastName = webUser.LastName,
                            Institution = orderRow.Order.Institution
                        },
                        AdminComments = order.AdminComments,
                        //AffiliateComments = orderRow.Order.AffiliateComments,
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
                        UserDetails = string.Concat("<span id='userFullnameLabel'>", userFullName,
                            "</span> - <span id='userInstitutionLabel'>", orderRow.Order.Institution, "</span><br>",
                            "<span id='userEmailLabel'>", webUser.email, "</span>"),

                        UserType = webUser.UserType
                    };

                    if (checkoutConfirmViewModel.OrderRowExists)
                    {
                        if (orderRow.idOrderRow > 0)
                            checkoutConfirmViewModel.OrderRowHasId = true;

                        if (checkoutConfirmViewModel.OrderRowHasId)
                            checkoutConfirmViewModel.OptionLabel = orderRow.RegistrationType.OptionLabel;
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

            var dataOp = new DataOperations(_globals.DefaultConnectionString);
            var addPrice = dataOp.GetAdditionalLocationsPricing(idWebinar).SingleOrDefault();

            decimal priceOfAdditionalLocation = 0M;

            if (!ReferenceEquals(addPrice, null))
            {
                priceOfAdditionalLocation = addPrice.Price; // Item2 of the Tuple is the price
            }


            var addAdditionalLocationViewModel = new AdditionalLocationOfferViewModel
            {
                AdditionalLocations = additionalLocations.ToList(),
                //AdditionalLocations = new List<AdditionalLocation>(),
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
                        var dataOperations =
                            new Business.Core.DataOperations(GlobalConfig.GlobalConfigSingleton.DefaultConnectionString);
                        var additionalLocationsPricing = dataOperations.GetAdditionalLocationsPricing(orderRow.idWebinar);
                        if (additionalLocationsPricing == null)
                        {
                            _logger.Warn("AdditionalLocation lacks price on: " + orderRow.idWebinar);
                            optionsCost = 0;
                        }
                        else
                        {
                            optionsCost = additionalLocationsPricing.Single().Price;
                        }
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
                    //var additionalLocations = orderRow.AdditionalLocation.ToList();
                    var webUser = order.WebUser;
                    var webinar = orderRow.Webinar;

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
                    _logger.Info("OrderHasAdditionalLocationsViewModel for: " + orderRow.Order.idOrder);

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
            _logger.Error("OrderHasAdditionalLocationsViewModel fell thru too far!");
            return null;
        }

        public void CancelOrder(int idOrder)
        {
            _logger.Info("CancelOrder hit {0}", idOrder);
            _orderManagementService.DeleteOrder(orderId: idOrder);
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

        public Order CreateOrder(CheckoutOptionsViewModel formModel)
        {
            _stateService.SetValue(DomainConstants.CheckoutInProcess, true);
            Claim beingImpersonatedClaim = null;

            if (Request.IsAuthenticated)
            {
                var user = Request.RequestContext.HttpContext.User as ClaimsPrincipal;
                beingImpersonatedClaim = user.Claims.SingleOrDefault(c => c.Type == ClaimTypes.BeingImpersonated);
            }
            
            var webinar = _webinarManagementService.GetWebinar(formModel.idWebinar);

            if(webinar == null)
            {
                _logger.Error(string.Format("Null value for Webinar {0}", formModel.idWebinar));
                throw new NullReferenceException(string.Format("Null value for Webinar with id {0}", formModel.idWebinar));
            }

            var newOrderRow = CreateOrderRow(webinar,
                formModel.AdditionalLocations == null ? null : formModel.AdditionalLocations.ToList(),
                formModel.RegistrationTypeId);

            var currentAffiliate = _stateService.GetValue<Affiliate>("CurrentAffiliate");
            _orderManagementService.AttachAffiliate(currentAffiliate);

            // At this point, user may not be registered. So, when creating the Order, if user 
            // does not exist, a dummy user with an email of notauthenticated@cuwebinars.com will be created.
            var webUser = _orderManagementService.GetWebUserWithAddressAndInstitution(formModel.idUser);

            return CreateNewOrder(currentAffiliate, webUser, webinar, newOrderRow, beingImpersonatedClaim);
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
            if (ReferenceEquals(null, webUser))
            {
                if(!_stateService.HasValue(WebUiConstants.SessionId))
                    _logger.Info("Session id is not in session.");
                if(!_stateService.HasValue(WebUiConstants.SessionId))
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

                newOrder.AdminComments = JsonHelpers.MergeJsonWithStoredField(newOrder.AdminComments,createdByImpersonatedUserMsg);
            }

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
            var notificationStorage = new NotificationStorage
            {
                idOrder = order.idOrder,
                SessionStartInfo = _appHelper.GetSessionStartInfo()
            };
            Stopwatch jsonWatch = Stopwatch.StartNew();

            order.NotificationStorage = JsonConvert.SerializeObject(notificationStorage);

            jsonWatch.Stop();
            Trace.TraceInformation("{0} took {1}s to run.", "JSON Serialization", jsonWatch.Elapsed.Seconds);

            if (userCreatedInCart.HasValue)
                _orderManagementService.FireOrderSubmittedEvent(order, userCreatedInCart.Value, url: Request.Url);
            else
                _orderManagementService.FireOrderSubmittedEvent(order, url: Request.Url);

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


            foreach (var newSubmittedAdditionalLocation in additionalLocations.Where(al => !existingAdditionalLocationsForOrderRow.Select(eal => eal.Email).Contains(al.Email)))
            {
                newSubmittedAdditionalLocation.idOrderRow = newOrderRowId;
                _orderManagementService.AddAdditionalLocation(newSubmittedAdditionalLocation);
            }

            foreach (var additionalLocationToDelete in
                existingAdditionalLocationsForOrderRow.Where(existingEmail => !additionalLocations.Select(al => al.Email).Contains(existingEmail.Email)))
            {
                _orderManagementService.RemoveAndDeleteAdditionalLocation(additionalLocationToDelete);
            }

            _orderManagementService.SaveChanges();
        }

        public Order LoadOrder(int id)
        {
            return _orderManagementService.GetOrderById(id);
        
        }

        public void SetOrderPaidByCC(int qOrder, string s, string formFields)
        {
            {
                Order order = LoadOrder(qOrder);

                //order.PaymentType = (PaymentType)2;


                //order.AdminComments = order.StoreComments + "\r\nCredit Card Order Approved: " + txApprovalCode.ToString();

                string buildMessage = "Step2_PayCC: " + order.idOrder + " QueryString: " + formFields;
                //Logger.Instance.LogMessage(buildMessage);

                order.OrderStatus = OrderStatus.Paid;
                _orderManagementService.SaveChanges();
            }
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
            return _orderManagementService.ApplyDiscountCode(code, row);
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