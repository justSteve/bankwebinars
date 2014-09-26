using CUWebinars.Business.AccountService;
using CUWebinars.Business.Models;
using CUWebinars.Business.Services;
using CUWebinars.Web.Helpers;
using CUWebinars.Web.Services;
using CUWebinars.Web.ViewModel;
using Ninject.Extensions.Logging;
using System;
using System.Linq;
using System.Web.Mvc;

namespace CUWebinars.Web.Controllers
{
    public class CartController : Controller
    {
        private TTSWebinarsContext db = new TTSWebinarsContext();
        readonly IStateService _stateService = new StateService();

        private readonly IMembershipService _membershipService;
        private readonly ILogger _logger;
        private readonly IOrderManagementService _orderManagementService;
        private readonly IWebinarManagementService _webinarManagementService;
        private bool _disposed;

        public CartController(IMembershipService membershipService,
            IOrderManagementService orderManagementService,
            IWebinarManagementService webinarManagementService,
            IStateService stateService,
            ILogger logger)
        {
            _membershipService = membershipService;
            _logger = logger;
            _orderManagementService = orderManagementService;
            _webinarManagementService = webinarManagementService;
            stateService = stateService;
        }

        ////            if (orderRowID != null && orderRowID > 0)
        //    {
        //        var thisModel = new WebinarDetailsViewModel();
        //        int rowID = Convert.ToInt32(orderRowID);
        //        thisModel.OrderRow = OrderFacade.Instance.LoadOrderRow(rowID);
        //        {
        //            if (thisModel.OrderRow != null)
        //            {
        //                thisModel.User = thisModel.OrderRow.Order.User;
        //                thisModel.Webinar = thisModel.OrderRow.Webinar;
        //                thisModel.Affiliate = thisModel.OrderRow.Order.Affiliate;
        //                thisModel.ConnectionInfo = OrderFacade.Instance.BuildConnectionInfo(thisModel.OrderRow);
        //            }
        //        }
        //        return thisModel;
        //    }
        //    return null;

        public WebinarDetailsViewModel BuildCheckOutViewModel(int? idOrderRow)
        {
            if (idOrderRow.HasValue && idOrderRow.Value > 0)
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

                return webinarDetailsViewModel;
            }
            return null;
        }


        //[AcceptVerbs(HttpVerbs.Post)]
        ////[Authorize(Roles = AppRoles.CustomerAffiliateAdmin)]
        //public ActionResult ConfirmOrder(int ID)
        //{
        //    var model = CheckOutViewModel(ID);

        //    var order = model.CheckoutOptionsViewModel.Order;

        //    if (Request["referred"] != null && WebUtility.HtmlDecode(Request["referred"]) != "How did you hear about this webinar?")
        //    {
        //        order.Origin = Request["referred"] + Environment.NewLine + order.Origin;
        //        //ViewData["referred"] = Request["referred"];
        //    }
        //    //OrderFacade.Instance.Submit(order);
        //    Session["IsOrderPaid"] = true;

        //    _logger.Info("User Submits order: " + order.idOrder);
        //    Session.Add("LastOrderId", Session["CurrentOrderId"]);
        //    Session.Remove("CurrentOrderId");
        //    Session.Remove("IsOrderPaid");

        //    //new MailController().OrderConfirmationEmail(row).Deliver();

        //    return Json(new
        //    {
        //        success = "success",
        //        orderRowID = order.OrderRows.SingleOrDefault().idOrderRow,
        //        msg = "OrderFacade.Instance.BuildConnectionInfo(order.Rows.SingleOrDefault())"
        //    }, JsonRequestBehavior.AllowGet);
        //}



        [AcceptVerbs(HttpVerbs.Post)]
        //[Authorize(Roles = AppRoles.CustomerAffiliateAdmin)]
        public ActionResult CancelOrder(int id)
        {
            //var order = _checkoutWorkflow.CurrentOrder;

            //_logger.Info("User Cancels order: " + order.ID);

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
            var model = BuildCheckOutViewModel(ID);
            return PartialView("Partials/CheckoutOptions", model);
        }

        public ActionResult CheckoutContact(int ID)
        {
            var model = BuildCheckOutViewModel(ID);
            return PartialView("Partials/CheckoutContact", model);
        }
        public ActionResult CheckoutConfirm(int ID)
        {
            var model = BuildCheckOutViewModel(ID);
            return PartialView("Partials/CheckoutConfirm", model);
        }
        public ActionResult CheckoutDisplayRowPrice(int ID)
        {
            var model = BuildCheckOutViewModel(ID);
            return PartialView("Partials/_DisplayRowPrice", model.Order.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Signup2(WebinarDetailsViewModel formModel
           , string stageOfCheckout)
        {

            var isPreReg = stageOfCheckout;
            ViewData["CheckoutInProcess"] = "true";
            var IsUserLogged = false;
            var model = formModel;

            var currentOrder = _stateService.GetValue<Order>("CurrentOrder");
            // Will we be putting this in Session at some point?

            //  i think we need a way to persist 'non-completed orders'
            // in interest of minimizing - perhaps just storing the orderID and then 
            // hydrating it as needed.
            var currentAffiliate = _stateService.GetValue<Affiliate>("CurrentAffiliate");

            model.Order = currentOrder;
            model.Affiliate = currentAffiliate;
            model.Webinar = _webinarManagementService.GetWebinar(formModel.Webinar.idWebinar);

            try
            {
                model.WebUser = _orderManagementService.GetWebUser(formModel.WebUser.idUser);
            }
            catch
            {
                _logger.InfoException("no valid WebUser found.", new Exception());
            }

            if (model.Webinar.Title.Contains("Compliance Perspectives"))
            // TODO: Clarify the reason for this.
            // Compliance Perspectives represents a variation on the standard
            // options/types of Registrations because it is a recurring, subscription-based
            // set of options. It has special handling as regards additional locations
            // pricing and notifications.
            {
                model.Webinar = _webinarManagementService.GetWebinar(883);
            }

            //var options = _orderManagementService.GetOptionsByWebinarIdFromOptionsRepository(model.Webinar.idWebinar, true);
            var options = _orderManagementService.GetOptionsByWebinarId(model.Webinar.idWebinar, true);
            model.Options = options;

            //IList<AdditionalLocation> addLoc = new

            // replace AdditionalLocations handling from scratch

            var orderRow = _orderManagementService.CreateOrderRow(model.Webinar, null, Convert.ToInt32(Request.Form["RegistrationType"]));
            

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

            currentOrder.Origin = "<p>InitialPage: " + _stateService.GetValue<String>("FirstPage") + "</p><p>" +
                       " InitialReferrer: " + _stateService.GetValue<String>("InitialQueryString") + "</p><p>" +
                       " InitialCookies: " + _stateService.GetValue<String>("FirstCookies") + "</p><p>" +
                       " SessionID: " + _stateService.GetValue<String>("SessionID") + "</p>";


            if (Request.IsAuthenticated)
            {
                IsUserLogged = true;
                
                currentOrder.AuditInfo = AppHelper.GetUserAuditInfo();
                currentOrder.Origin = _orderManagementService.GetOrderInitiator();
                currentOrder.Affiliate = currentAffiliate;

            }
            else
            {
                IsUserLogged = false;
                //_orderManagementService.AssignUserToOrder(currentOrder);
                currentOrder.Affiliate = currentAffiliate;
                currentOrder.AuditInfo = AppHelper.GetUserAuditInfo();
                currentOrder.Origin = _orderManagementService.GetOrderInitiator();
                _logger.Error("ERROR: CartController | Signup - currentUser is null" + currentOrder.idOrder);
                //TODO: assign appropriate ModelError and error logging/handling
            }


            currentOrder = _orderManagementService.SaveOrderChanges(currentOrder, "", "");

            //if (model.Webinar.idWebinar == 883 && model.Webinar.Status == WebinarStatus.Scheduled)
            //{
            //    try
            //    {
            //        _orderManagementService.CreateCPSubscription(orderRow);
            //    }
            //    catch (Exception)
            //    {
            //        throw;
            //    }
            //}

            try
            {
                if (stageOfCheckout == "preReg")
                {
                    return PartialView("Partials/_DisplayRowPrice", model.Order.OrderRows.Single());
                }
                else
                {
                    
                    model.Order = currentOrder;

                    //db.SaveChanges();
                    return Json(new
                                    {
                                        success = "success",
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
            //        _checkoutWorkflow.CurrentOrder.Origin= currentUser.UserType;
            //        //_checkoutWorkflow.CurrentOrder.InitiatedBy2 = currentUser;
            //        OrderFacade.Instance.Save(_checkoutWorkflow.CurrentOrder);
            //    }

            //    _checkoutWorkflow.SetProceedToNextStep(true);
            //}
            //else
            //{
            //    _logger.Instance.LogMessage("non-authenticated user at Checkout|Step1_Register: " + AppHelper.GetUserAuditInfo());
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
                    _logger.Info(shouldShow);
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
            _orderManagementService.SaveOrderChanges(row.Order, null, null);

            return Json(row.Order.OrderStatus.ToString());
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