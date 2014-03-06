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
                //    if (thisModel.OrderRow != null)
                //    {
                //        thisModel.User = thisModel.OrderRow.Order.User;
                //        thisModel.Webinar = thisModel.OrderRow.Webinar;
                //        thisModel.Affiliate = thisModel.OrderRow.Order.Affiliate;
                //        thisModel.ConnectionInfo = OrderFacade.Instance.BuildConnectionInfo(thisModel.OrderRow);
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

            var order = model.Order;

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

            //new MailController().OrderConfirmationEmail(model.OrderRow).Deliver();

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
        //    return PartialView("_DisplayRowPrice", model.OrderRow);
        //}

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Signup2(WebinarDetailsViewModel formModel
                , int mode
                , string stage_of_checkout
                , string addEmails
                , int? sixMonthPaidConnectionsCount
                , int? twelveMonthPaidConnectionsCount

            )
        {

            var currentAffiliate = stateService.GetValue<Affiliate>("CurrentAffiliate");
            var isPreReg = stage_of_checkout;
            ViewData["CheckoutInProcess"] = "true";
            var IsUserLogged = false;

            var currentOrder = _orderManagementService.CreateNewOrder();

            currentOrder.Origin = "<p>InitialPage: " + HttpContext.Session["FirstPageOfSession"] + "</p><p>" +
                       " InitialReferrer: " + HttpContext.Session["FirstReferrerOfSession"] + "</p><p>" +
                       " InitialCookies: " + HttpContext.Session["FirstCookiesOfSession"] + "</p><p>" +
                       " SessionID: " + HttpContext.Session["SessionID"] + "</p>";

            currentOrder = _orderManagementService.AssignAffiliateToOrder(currentAffiliate, currentOrder);
            currentOrder = _orderManagementService.AssignWebUserToOrder(formModel.WebUser, currentOrder);
            
            var model = formModel;

            model.Webinar = db.Webinars.Find(formModel.Webinar.idWebinar);

            try
            {
                model.WebUser = db.WebUsers.Find(formModel.WebUser.idUser);
            }
            catch
            {

            }
            
            if (Request.IsAuthenticated)
            {
                IsUserLogged = true;
                //_orderManagementService.AssignUserToOrder(currentOrder, model.WebUser);
                currentOrder.AuditInfo = AppHelper.GetUserAuditInfo();
                currentOrder.InitiatedBy = _orderManagementService.GetOrderInitiator();
            }
            else
            {

                Logger.Error("ERROR: CartController | Signup - currentUser is null" + currentOrder.idOrder);
                //TODO: assign appropriate ModelError and error logging/handling
            }

            if (model.Webinar.Title.Contains("Compliance Perspectives"))
            {
                model.Webinar = db.Webinars.Find(883);
            }

            currentOrder = _orderManagementService.SaveChanges(currentOrder);
            
            var orderRow = new OrderRow
                                    {
                                        Webinar = model.Webinar,
                                        Order = currentOrder,
                                        AlternateEmail = String.Empty,
                                        RegistrationType = mode
                                    };
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

            //var additionalLocations = orderRow.Options.OfType<AdditionalLocationsOrderRowOption>();
            var additionalLocations = orderRow.OrderRowOptions.OfType<AdditionalLocationsOrderRowOption>();
            AdditionalLocationsOrderRowOption[] additionalLocationsOption =
                additionalLocations as AdditionalLocationsOrderRowOption[] ?? additionalLocations.ToArray();
            int count = additionalLocationsOption.Count();
            if (count > 0)
            {
                int additionalLocationsCount;
                if (int.TryParse(Request["AdditionalLocationsCount" + orderRow.idOrderRow], out additionalLocationsCount))
                {
                    additionalLocationsOption.Single().AdditionalLocationsCount = additionalLocationsCount;
                }
                else if (Request["AdditionalLocationsCount" + orderRow.idOrderRow] == "")
                {
                    additionalLocationsOption.Single().AdditionalLocationsCount = 0;
                }
            }


            var options = _orderManagementService.GetOptionsByWebinarId(model.Webinar.idWebinar);
            var option = options.SingleOrDefault(o => o.Type == "additional_location");

            addEmails = addEmails.TrimStart(',');
            var connectionsCount = addEmails.Split(',');
            if (connectionsCount.Any())
            {
                var orderRowOption = new OrderRowOption()
                {
                    additional_locations_count = connectionsCount.Count(),
                    Option = option,
                    OrderRow = orderRow,
                    OptionDescription = option.OptionExplain,
                    OptionPrice = Convert.ToDecimal(option.PriceToAdd),
                    additional_locations_emails = addEmails
                };

                if (model.Webinar.idWebinar == 842 && model.Webinar.Status == WebinarStatus.Scheduled)
                {
                    ////CreateCPSubscription()
                    //int freeConnectionsCount;
                    //if (addEmails == "")
                    //{
                    //    freeConnectionsCount = 0;
                    //}
                    //else
                    //{
                    //    freeConnectionsCount = addEmails.Split(',').Count();
                    //}
                    ////int freeConnectionsCount = 0;
                    //orderRowOption.AdditionalLocationsCount = freeConnectionsCount;
                    //if (orderRowOption.AdditionalLocationsCount > 3)
                    //{
                    //    orderRowOption.AdditionalLocationsCount = 3;
                    //}


                    //if ((RegistrationType)mode == RegistrationType.Twelve_Month_Subscription)
                    //{
                    //    orderRowOption.AdditionalLocationsCount += twelveMonthPaidConnectionsCount.HasValue ? twelveMonthPaidConnectionsCount.Value : 0;
                    //}
                    //else
                    //{
                    //    orderRowOption.AdditionalLocationsCount += sixMonthPaidConnectionsCount.HasValue ? sixMonthPaidConnectionsCount.Value : 0;
                    //}
                }

                orderRow.OrderRowOptions.Add(orderRowOption);
            }

            try
            {
                if (stage_of_checkout == "preReg")
                {
                    return PartialView("_DisplayRowPrice", model.Order.OrderRows.Single());
                }
                else
                {
                    //_orderManagementService.Save(currentOrder);

                    _orderManagementService.AddOrderRow(currentOrder, orderRow);

                    Session["CurrentOrderId"] = currentOrder.idOrder;
                    //_checkoutWorkflow.AddOrder(currentOrder);
                    model.Order.OrderRows.Add(orderRow);

                    db.SaveChanges();
                    ViewData["WhichStep"] = IsUserLogged ? "Step2" : "Step1";
                    return Json(new
                                    {
                                        success = "success",
                                        whichStep = "Step1",
                                        orderRowID = model.Order.OrderRows.Single().idOrderRow
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
            //        _checkoutWorkflow.CurrentOrder.InitiatedBy = currentUser.UserType;
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

            //var shouldShow = "true";
            var shouldShow = db.Options.Where(o => o.idOption == optionID).Select(o => o.ShowLiveNotifications);
            //if (OptionsFacade.Instance.Load(optionID).ShowLiveNotifications == "No") shouldShow = "false";
            return Json(new
                                {
                                    shouldShow
                                }, JsonRequestBehavior.AllowGet);
        }

        //[Authorize(Roles = AppRoles.Admin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SetOrderRowStatus(int orderRowID, OrderRowStatus status)
        {
            OrderRow row = _orderManagementService.LoadOrderRow(orderRowID);
            row.Status = status;
            _orderManagementService.Save(row.Order);

            return Json(row.Status.ToString());
        }
    }
}