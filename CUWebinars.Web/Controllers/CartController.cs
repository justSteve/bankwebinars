using System.Collections.Generic;
using System.Net;
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
using System.Web.Mvc;

namespace CUWebinars.Web.Controllers
{
    public class CartController : Controller
    {
        private TTSWebinarsContext db = new TTSWebinarsContext();
        readonly IStateService _stateService;

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
            _stateService = stateService;
        }

        public PartialViewResult GetAdditionalLocationByOrderId(int webUserId, int webinarId)
        {
            //  TODO: Implement

            var order = _orderManagementService.GetOrdersByUserId(webUserId);

            var addAdditionalLocationViewModel = new AdditionalLocationAddViewModel
            {
                // AdditionalLocations = order.OrderRows.First().AdditionalLocation.ToList()
                AdditionalLocation = new List<AdditionalLocation>()
            };

            return PartialView("~/Views/Webinar/Partials/_AdditionalLocationsModal.cshtml", addAdditionalLocationViewModel);
        }


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


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ConfirmOrder(int id)
        {
            var model = BuildCheckOutViewModel(id);

            var order = model.Order;

            if (Request["referred"] != null && WebUtility.HtmlDecode(Request["referred"]) != "How did you hear about this webinar?")
            {
                order.Origin = Request["referred"] + Environment.NewLine + order.Origin;
                //ViewData["referred"] = Request["referred"];
            }

            return Json(new
            {
                success = "success",
                orderRowID = order.OrderRows.SingleOrDefault().idOrderRow,
                msg = "OrderFacade.Instance.BuildConnectionInfo(order.Rows.SingleOrDefault())"
            }, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
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

        public PartialViewResult CheckoutContactDetails()
        {
            var model = new RegisterViewModel
            {
                RegisterFields = new RegisterModel
                {
                    AccountDetailsTitle = string.Empty,
                    BillingAddress = new AddressModel
                    {

                    }
                }
            };

            return PartialView("~/Views/cart/Partials/CheckoutContact.cshtml", model);
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Signup2(WebinarDetailsViewModel formModel, string stageOfCheckout)
        {
            formModel.CheckoutInProcess = true;
            var model = formModel;

            //var currentOrder = _stateService.GetValue<Order>("CurrentOrder");
            // let's see if we can avoid the need for Session Var



            var currentAffiliate = _stateService.GetValue<Affiliate>("CurrentAffiliate");

            model.Order = new Order();
            model.Affiliate = currentAffiliate;
            model.Webinar = _webinarManagementService.GetWebinar(formModel.Webinar.idWebinar);

            _orderManagementService.AttachAffiliate(model.Affiliate);

            try
            {
                model.WebUser = _orderManagementService.GetWebUser(formModel.WebUser.idUser);
            }
            catch
            {
                _logger.InfoException("no valid WebUser found.", new Exception());
            }

            //var options = _orderManagementService.GetOptionsByWebinarIdFromOptionsRepository(model.Webinar.idWebinar, true);
            var options = _orderManagementService.GetOptionsByWebinarId(model.Webinar.idWebinar, true);
            model.Options = options;

            //IList<AdditionalLocation> addLoc = new

            // replace AdditionalLocations handling from scratch

            var orderRow = _orderManagementService.CreateOrderRow(model.Webinar, null, Convert.ToInt32(Request.Form["RegistrationType"]));

            var currentOrder = _orderManagementService.CreateNewOrder(
                    model.Affiliate,
                    model.WebUser,
                    model.Webinar,
                    orderRow
                //,model.Options
                    );
            //StateService.SetValue("CurrentOrder", string.Empty);


            currentOrder.Origin = "<p>InitialPage: " + _stateService.GetValue<String>("FirstPage") + "</p><p>" +
                       " InitialReferrer: " + _stateService.GetValue<String>("InitialQueryString") + "</p><p>" +
                       " InitialCookies: " + _stateService.GetValue<String>("FirstCookies") + "</p><p>" +
                       " SessionID: " + _stateService.GetValue<String>("SessionID") + "</p>";


            if (Request.IsAuthenticated)
            {
                model.UserIsLoggedIn = true;
            }
            else
            {
                model.UserIsLoggedIn = false;
                //_orderManagementService.AssignUserToOrder(currentOrder);
                _logger.Error("ERROR: CartController | Signup - currentUser is null" + currentOrder.idOrder);
                //TODO: assign appropriate ModelError and error logging/handling
            }

            currentOrder.AuditInfo = AppHelper.GetUserAuditInfo();
            currentOrder.Origin = _orderManagementService.GetOrderInitiator();
            
            currentOrder = _orderManagementService.SaveOrderChanges(currentOrder, string.Empty, string.Empty);

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