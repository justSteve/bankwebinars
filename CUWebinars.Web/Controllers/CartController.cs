using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CUWebinars.Business.AccountService;
using CUWebinars.Business.Models;
using CUWebinars.Business.Repository;
using CUWebinars.Business.Services;
using CUWebinars.Web.Helpers;
using CUWebinars.Web.Services;
using CUWebinars.Web.ViewModel;
using Ninject.Extensions.Logging;

namespace CUWebinars.Web.Controllers
{
    public class CartController : Controller
    {
        private TTSWebinarsContext db = new TTSWebinarsContext();
        private IMailService _mail;
        IStateService stateService = new StateService();

        //Steve added MembershipService dependancy to allow for 'currentUser' in Details.
        public IMembershipService membershipService;
        private readonly IWebinarRepository _webinarRepository;
        private readonly IOrderManagementService _orderManagementService;
        public ILogger Logger { get; set; }

        public CartController(MembershipService membershipService, IMailService mail, IWebinarRepository webinarRepository, ILogger logger, IOrderManagementService orderManagementService)
        {
            this.membershipService = membershipService;
            _mail = mail;
            _webinarRepository = webinarRepository;
            Logger = logger;
            _orderManagementService = orderManagementService;
        }

        //


        //private CheckoutWorkflowHelper2 _checkoutWorkflow;

        //public CartController()
        //{
        //    FormsAuth = new FormsAuthenticationService();
        //}

        //public IFormsAuthentication FormsAuth
        //{
        //    get;
        //    private set;
        //}

        //protected override void OnActionExecuting(ActionExecutingContext filterContext)
        //{
        //    _checkoutWorkflow = new CheckoutWorkflowHelper2(ControllerContext);

        //    base.OnActionExecuting(filterContext);
        //}

        public WebinarDetailsViewModel CheckOutViewModel(int? orderRowID)
        {
            if (orderRowID != null && orderRowID > 0)
            {
                var thisModel = new WebinarDetailsViewModel();
                int rowID = Convert.ToInt32(orderRowID);
                //thisModel.Order.OrderRow = OrderFacade.Instance.LoadOrderRow(rowID);
                //{
                //    if (thisrow != null)
                //    {
                //        thisModel.User = thisrow.Order.User;
                //        thisModel.Webinar = thisrow.Webinar;
                //        thisModel.Affiliate = thisrow.Order.Affiliate;
                //        thisModel.ConnectionInfo = OrderFacade.Instance.BuildConnectionInfo(thisrow);
                //    }
                //}
                return thisModel;
            }
            return null;
        }


        [AcceptVerbs(HttpVerbs.Post)]
        //[Authorize(Roles = AppRoles.CustomerAffiliateAdmin)]
        public ActionResult ConfirmOrder(int ID)
        {
            var model = CheckOutViewModel(ID);

            var order = model.CheckoutOptionsViewModel.Order;

            if (Request["referred"] != null && WebUtility.HtmlDecode(Request["referred"]) != "How did you hear about this webinar?")
            {
                order.Origin = Request["referred"] + Environment.NewLine + order.Origin;
                //ViewData["referred"] = Request["referred"];
            }
            //OrderFacade.Instance.Submit(order);
            Session["IsOrderPaid"] = true;

            Logger.Info("User Submits order: " + order.idOrder);
            Session.Add("LastOrderId", Session["CurrentOrderId"]);
            Session.Remove("CurrentOrderId");
            Session.Remove("IsOrderPaid");

            //new MailController().OrderConfirmationEmail(row).Deliver();

            return Json(new
            {
                success = "success",
                orderRowID = order.OrderRows.SingleOrDefault().idOrderRow,
                msg = "OrderFacade.Instance.BuildConnectionInfo(order.Rows.SingleOrDefault())"
            }, JsonRequestBehavior.AllowGet);
        }



        [AcceptVerbs(HttpVerbs.Post)]
        //[Authorize(Roles = AppRoles.CustomerAffiliateAdmin)]
        public ActionResult CancelOrder(int id)
        {
            //var order = _checkoutWorkflow.CurrentOrder;

            //Logger.Info("User Cancels order: " + order.ID);

            //OrderFacade.Instance.DeleteOrderRow(order, id);

            //_checkoutWorkflow.RemoveOrder(order.ID);
            //_checkoutWorkflow.;


            Session.Remove("LastOrderId");
            Session.Remove("CurrentOrderId");
            Session.Remove("CurrentOrderIds");
            Session.Remove("IsOrderPaid");

            return Json(new
            {
                success = "success"

            }, JsonRequestBehavior.AllowGet);
            //return RedirectToAction("Step2");
        }


        public ActionResult CheckoutOptions(int ID)
        {
            var model = CheckOutViewModel(ID);
            return PartialView("Partials/CheckoutOptions", model);
        }

        public ActionResult CheckoutContact(int ID)
        {
            var model = CheckOutViewModel(ID);
            return PartialView("Partials/CheckoutContact", model);
        }
        public ActionResult CheckoutConfirm(int ID)
        {
            var model = CheckOutViewModel(ID);
            return PartialView("Partials/CheckoutConfirm", model);
        }
        //public ActionResult CheckoutDisplayRowPrice(int ID)
        //{
        //    var model = CheckOutViewModel(ID);
        //    return PartialView("_DisplayRowPrice", row);
        //}

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Signup2(WebinarDetailsViewModel formModel
                , int mode
                , string stageOfCheckout
                , int? sixMonthPaidConnectionsCount
                , int? twelveMonthPaidConnectionsCount

            )
        {
            var isPreReg = stageOfCheckout;
            ViewData["CheckoutInProcess"] = "true";
            var IsUserLogged = false;
            var model = formModel;

            var currentOrder = stateService.GetValue<Order>("CurrentOrder");
            // Will we be putting this in Session at some point?

            //  i think we need a way to persist 'non-completed orders'
            // in interest of minimizing - perhaps just storing the orderID and then 
            // hydrating it as needed.
            var currentAffiliate = stateService.GetValue<Affiliate>("CurrentAffiliate");

            model.CheckoutOptionsViewModel.Order = currentOrder;
            model.Affiliate = currentAffiliate;
            model.Webinar = _orderManagementService.GetWebinar(formModel.Webinar.idWebinar);

            try
            {
                model.WebUser = _orderManagementService.GetWebUser(formModel.WebUser.idUser);
                // hardcode in the currentUser - steve@juststeve.com
                //model.WebUser = _orderManagementService.GetWebUser(26357);
            }
            catch
            {

            }

            if (model.Webinar.Title.Contains("Compliance Perspectives")) // TODO: Clarify the reason for this.
            {
                model.Webinar = _orderManagementService.GetWebinar(883);
            }

            var options = _orderManagementService.GetOptionsByWebinarIdFromOptionsRepository(model.Webinar.idWebinar, true);
            model.Options = options;

            //var option = options.SingleOrDefault(o => o.Type == "additional_location");
            var option = new AdditionalLocations();

            AdditionalLocations additionalLocations = null;

            var emailAddresses =
                model.
                CheckoutOptionsViewModel.
                DisplayOptionsViewModel.
                AdditionalLocationsViewModel.
                AddAdditionalLocationsViewModel.Emails;

            //if (emailAddresses.Any())
            //{
            //    additionalLocations = _orderManagementService.CreateOrderRowOption(
            //        option,
            //        option.OptionExplain,
            //        Convert.ToDecimal(option.PriceToAdd),
            //        emailAddresses.ToArray()
            //        );

            //    if (model.Webinar.idWebinar == 842 && model.Webinar.Status == WebinarStatus.Scheduled)
            //    {
            //        ////CreateCPSubscription()
            //        //int freeConnectionsCount;
            //        //if (addEmails == "")
            //        //{
            //        //    freeConnectionsCount = 0;
            //        //}
            //        //else
            //        //{
            //        //    freeConnectionsCount = addEmails.Split(',').Count();
            //        //}
            //        ////int freeConnectionsCount = 0;
            //        //AdditionalLocations.AdditionalLocationsCount = freeConnectionsCount;
            //        //if (AdditionalLocations.AdditionalLocationsCount > 3)
            //        //{
            //        //    AdditionalLocations.AdditionalLocationsCount = 3;
            //        //}


            //        //if ((RegistrationType)mode == RegistrationType.Twelve_Month_Subscription)
            //        //{
            //        //    AdditionalLocations.AdditionalLocationsCount += twelveMonthPaidConnectionsCount.HasValue ? twelveMonthPaidConnectionsCount.Value : 0;
            //        //}
            //        //else
            //        //{
            //        //    AdditionalLocations.AdditionalLocationsCount += sixMonthPaidConnectionsCount.HasValue ? sixMonthPaidConnectionsCount.Value : 0;
            //        //}
            //    }

            //}

            var orderRow = _orderManagementService.CreateOrderRow(model.Webinar, additionalLocations, string.Empty, mode);
            // populate the ViewBag with the RegistrationType (aka RegType)
            //ViewBag.RegistrationType = options.SingleOrDefault(o => o.idRegType == orderRow.RegistrationType);


            if (currentOrder == null)
            {
                currentOrder = _orderManagementService.CreateNewOrder(
                    model.Affiliate,
                    model.WebUser,
                    model.Webinar,
                    orderRow
                    //,model.Options
                    );
                //StateService.SetValue("CurrentOrder", string.Empty);
            }

            currentOrder.Origin = "<p>InitialPage: " + stateService.GetValue<String>("FirstPage") + "</p><p>" +
                       " InitialReferrer: " + stateService.GetValue<String>("InitialQueryString") + "</p><p>" +
                       " InitialCookies: " + stateService.GetValue<String>("FirstCookies") + "</p><p>" +
                       " SessionID: " + stateService.GetValue<String>("SessionID") + "</p>";


            //var additionalLocations = orderRow.AdditionalLocations.OfType<AdditionalLocationsOrderRowOption>();
            //var additionalLocationsOption = additionalLocations as AdditionalLocationsOrderRowOption[] ?? additionalLocations.ToArray();

            //int count = additionalLocationsOption.Count();

            //if (count > 0)
            //{
            //    int additionalLocationsCount;

            //    if (int.TryParse(Request["AdditionalLocationsCount" + orderRow.idOrderRow], out additionalLocationsCount))
            //    {
            //        additionalLocationsOption.Single().AdditionalLocationsCount = additionalLocationsCount;
            //    }
            //    else if (Request["AdditionalLocationsCount" + orderRow.idOrderRow] == "")
            //    {
            //        additionalLocationsOption.Single().AdditionalLocationsCount = 0;
            //    }
            //}

            if (Request.IsAuthenticated)
            {
                IsUserLogged = true;
                _orderManagementService.AssignUserToOrder(currentOrder);
                currentOrder.AuditInfo = AppHelper.GetUserAuditInfo();
                currentOrder.Origin= _orderManagementService.GetOrderInitiator();

            }
            else
            {
                IsUserLogged = false;
                //_orderManagementService.AssignUserToOrder(currentOrder);
                currentOrder.AuditInfo = AppHelper.GetUserAuditInfo();
                currentOrder.Origin= _orderManagementService.GetOrderInitiator();
                Logger.Error("ERROR: CartController | Signup - currentUser is null" + currentOrder.idOrder);
                //TODO: assign appropriate ModelError and error logging/handling
            }


            currentOrder = _orderManagementService.SaveOrderChanges(currentOrder);

            if (model.Webinar.idWebinar == 883 && model.Webinar.Status == WebinarStatus.Scheduled)
            {
                try
                {
                    _orderManagementService.CreateCPSubscription(orderRow);
                }
                catch (Exception)
                {
                    throw;
                }
            }

            try
            {
                if (stageOfCheckout == "preReg")
                {
                    return PartialView("_DisplayRowPrice", model.CheckoutOptionsViewModel.Order.OrderRows.Single());
                }
                else
                {
                    //_orderManagementService.Save(currentOrder);

                    //_orderManagementService.AddOrderRow(currentOrder, orderRow);

                    Session["CurrentOrderId"] = currentOrder.idOrder;
                    //_checkoutWorkflow.AddOrder(currentOrder);
                    //model.Order.OrderRows.Add(orderRow);
                    model.CheckoutOptionsViewModel.Order = currentOrder;

                    //db.SaveChanges();
                    ViewData["WhichStep"] = IsUserLogged ? "Step2" : "Step1";
                    return Json(new
                                    {
                                        success = "success",
                                        whichStep = "Step1",
                                        orderRowID = model.CheckoutOptionsViewModel.Order.OrderRows.Single().idOrderRow
                                    }, JsonRequestBehavior.AllowGet);
                    //return View("~/Views/Webinar/Details2.cshtml", model);
                }
            }
            catch
            {
                throw;
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Step1_Register()
        {
            ////ViewData["CheckoutInProcess"] = "false";
            ////ViewData["WhichStep"] = "Step0";
            //if (Request.IsAuthenticated)
            //{
            //    string forAuditInfo = "Submitted via EndUserCheckout ";
            //    var currentUser = _mem
            //    //var currentUser = UserFacade.Instance.GetCurrentUser();

            //    //if (UserFacade.Instance.IsCurrentUserControlled())
            //    //{
            //    //    forAuditInfo += " by Affiliate or Admin " + UserFacade.Instance.GetMasterUserID();
            //    //    currentUser = UserFacade.Instance.GetMasterUser();
            //    //}

            //    if (_checkoutWorkflow.CurrentOrder != null)
            //    {
            //        OrderFacade.Instance.AssignUserToOrder(_checkoutWorkflow.CurrentOrder, currentUser);
            //        _checkoutWorkflow.CurrentOrder.AuditInfo = forAuditInfo + " " + AppHelper.GetUserAuditInfo();
            //        _checkoutWorkflow.CurrentOrder.Origin= currentUser.UserType;
            //        //_checkoutWorkflow.CurrentOrder.InitiatedBy2 = currentUser;
            //        OrderFacade.Instance.Save(_checkoutWorkflow.CurrentOrder);
            //    }

            //    _checkoutWorkflow.SetProceedToNextStep(true);
            //}
            //else
            //{
            //    Logger.Instance.LogMessage("non-authenticated user at Checkout|Step1_Register: " + AppHelper.GetUserAuditInfo());
            //}


            //ViewData["WhichStep"] = _checkoutWorkflow.IsUserLogged() ? "Step2" : "Step1";
            return Json(new
            {
                success = "success",
                userInfo = "_checkoutWorkflow.CurrentOrder.User.FullName",// + "<br>" + _checkoutWorkflow.CurrentOrder.User.Institution.Name + "<br>" + _checkoutWorkflow.CurrentOrder.User.Email,
                whichStep = "",//_checkoutWorkflow.IsUserLogged() ? "Step2" : "Step1",
                orderRowID = 2//_checkoutWorkflow.CurrentOrder.Rows.SingleOrDefault().ID
            }, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult CheckIfAddLocShouldHide(int optionID)
        {
            var shouldShow = "";
            var firstOrDefault = db.RegTypes.Where(o => o.idRegType == optionID)
                .Select(o => o.ShowLiveNotifications).FirstOrDefault();
            if (firstOrDefault != null)
            {
                shouldShow = firstOrDefault.ToString();
                //if (OptionsFacade.Instance.Load(optionID).ShowLiveNotifications == "No") shouldShow = "false";
                if (shouldShow == "Yes")
                {
                    Logger.Info(shouldShow);
                }

            }
            return Json(new
                 {
                     shouldShow
                 }, JsonRequestBehavior.AllowGet);
        }

        //[Authorize(Roles = AppRoles.Admin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SetOrderStatus(int orderRowID, OrderStatus status)
        {
            OrderRow row = _orderManagementService.LoadOrderRow(orderRowID);
            row.Order.OrderStatus = status;
            _orderManagementService.SaveOrderChanges(row.Order);

            return Json(row.Order.OrderStatus.ToString());
        }
    }
}