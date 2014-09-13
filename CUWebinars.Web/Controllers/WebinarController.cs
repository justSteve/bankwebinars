using System.Configuration;
using System.IO;
using System.Web.Hosting;
using CUWebinars.Business.AccountService;
using CUWebinars.Business.Core.Exceptions;
using CUWebinars.Business.Models;
using CUWebinars.Business.Services;
using CUWebinars.Web.Core;
using CUWebinars.Web.Core.Browsers.Webinars;
using CUWebinars.Web.Helpers;
using CUWebinars.Web.Services;
using CUWebinars.Web.ViewModel;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;


namespace CUWebinars.Web.Controllers
{
    public class WebinarController : Controller
    {

        IStateService stateService = new StateService();

        //Steve added MembershipService dependancy to allow for 'currentUser' in Details.
        private readonly IMembershipService _membershipService;
        private readonly IOrderManagementService _orderManagementService;
        private readonly IWebinarManagementService _webinarManagementService;
        private readonly ILogger _logger;
        private bool _disposed;

        public WebinarController(
            IMembershipService membershipService,
            IOrderManagementService orderManagementService,
            IWebinarManagementService webinarManagementService,
            ILogger logger
            )
        {
            _membershipService = membershipService;
            _orderManagementService = orderManagementService;
            _webinarManagementService = webinarManagementService;
            _logger = logger;
        }
        //
        // GET: /Webinar/
        public ActionResult CompliancePerspectives()
        {

            ViewBag.TopicCaption = " ";
            ViewBag.Title = "All Listed Events for CUWebinars";

            var current = _webinarManagementService.GetCompliancePerspectives();
            ViewBag.Title = "All Upcoming Events for CUWebinars";

            return View(current);
        }

        public ActionResult Index()
        {
            var webinars = _webinarManagementService.GetAllActive();
            ViewBag.TopicCaption = " ";
            ViewBag.Title = "All Listed Events for CUWebinars";

            webinars = _webinarManagementService.GetUpcomingWebinars().OrderBy(w => w.Date);
            ViewBag.Title = "All Upcoming Events for CUWebinars";

            return View(webinars);
        }

        public ActionResult ListByTopic(int ID)
        {
            // var webinars = WebinarFacade.Instance.SelectAllActiveWebinarsByTopic(ID);

            Session["TopicID"] = ID;
            ViewBag.SearchTerm = "TopicID=" + Session["TopicID"].ToString();
            ViewBag.TopicCaption = "";

            switch (Session["TopicID"].ToString())
            {
                case "16":
                    ViewBag.TopicCaption = "IRA ";
                    ViewBag.Title = ConfigurationManager.AppSettings["Tenant"] + " related to IRAs ";
                    break;
                case "15":
                    ViewBag.TopicCaption = "Compliance ";
                    ViewBag.Title = ConfigurationManager.AppSettings["Tenant"] + " related to Compliance ";
                    break;
                case "17":
                    ViewBag.TopicCaption = "Customer Service ";
                    ViewBag.Title = ConfigurationManager.AppSettings["Tenant"] + " related to Customer Service ";
                    break;
                case "18":
                    ViewBag.TopicCaption = "Security ";
                    ViewBag.Title = ConfigurationManager.AppSettings["Tenant"] + " related to Security ";
                    break;
                case "19":
                    ViewBag.TopicCaption = "Operations ";
                    ViewBag.Title = ConfigurationManager.AppSettings["Tenant"] + " related to Operations ";
                    break;
                case "20":
                    ViewBag.TopicCaption = "Auditing ";
                    ViewBag.Title = ConfigurationManager.AppSettings["Tenant"] + " related to Auditing ";
                    break;
                case "21":
                    ViewBag.TopicCaption = "Sales ";
                    ViewBag.Title = ConfigurationManager.AppSettings["Tenant"] + " related to Sales ";
                    break;
                case "22":
                    ViewBag.TopicCaption = "Lending ";
                    ViewBag.Title = ConfigurationManager.AppSettings["Tenant"] + " related to Lending ";
                    break;
                case "23":
                    ViewBag.TopicCaption = "Human Resources ";
                    ViewBag.Title = ConfigurationManager.AppSettings["Tenant"] + " related to Human Resources ";
                    break;
                case "25":
                    ViewBag.TopicCaption = "Computer Skills ";
                    ViewBag.Title = ConfigurationManager.AppSettings["Tenant"] + " related to Computer Skills ";
                    break;
                case "26":
                    ViewBag.TopicCaption = "Risk Management ";
                    ViewBag.Title = ConfigurationManager.AppSettings["Tenant"] + " related to Risk Management ";
                    break;
                case "27":
                    ViewBag.TopicCaption = "Teller ";
                    ViewBag.Title = ConfigurationManager.AppSettings["Tenant"] + " related to Teller ";
                    break;
            }

            var webinars = _webinarManagementService.GetByTopic(ID);
            //var dtos = new WebinarDTOAssembler().Entities2DTOs(webinars);
            //return View(dtos);
            return View(webinars);
        }

        //public PartialViewResult GetAdditionalLocationByOrderId(int webUserId, int webinarId)
        //{
        //    //  TODO: Implement

        //    var order = _orderManagementService.GetOrdersByUserId();

        //    var addAdditionalLocationViewModel = new WebinarDetailsViewModel
        //    {
        //        AdditionalLocations = order.OrderRows.First().AdditionalLocation.ToList()
        //        AdditionalLocations = new AdditionalLocation[] { new AdditionalLocation { Email = "dave@dave.com" }, new AdditionalLocation { Email = "monty@python.com" } }
        //    };

        //    return PartialView("~/Views/Webinar/Partials/_AdditionalLocationsModal.cshtml", addAdditionalLocationViewModel);
        //}

        public ActionResult AllActive(string eventsToShow)
        {
            var webinars = _webinarManagementService.GetAllActive();
            ViewBag.TopicCaption = " ";
            ViewBag.Title = "All Listed Events for CUWebinars";
            if (eventsToShow == "upcoming")
            {
                webinars = _webinarManagementService.GetUpcomingWebinars().OrderBy(w => w.Date);
                ViewBag.Title = "All Upcoming Events for CUWebinars";
            }
            if (eventsToShow == "recorded")
            {
                webinars = _webinarManagementService.GetRecordedWebinars().OrderBy(w => w.Date);
                ViewBag.Title = "All Recorded Events for CUWebinars";
            }

            return View(webinars);

        }

        public ActionResult SearchWebinars()
        {
            if (Request["searchFor"] != null)
                Session["searchTerm"] = Request["searchFor"].ToString();

            return View("~/Views/Webinar/SearchWebinars.cshtml");
        }

        public PartialViewResult SendConnectionInfo()
        {
            var model = new AdhocNotificationViewModel
            {
                Webinars = EventInvokerHelpers.GetUpcomingWebinarsAsSelectListItems(_webinarManagementService)
            };

            return PartialView(@"Partials/_SendConnectionInfo", model);
        }

        [System.Web.Mvc.HttpPost]
        public JsonResult SendConnectionInfo(int webinarId)
        {
            try
            {
                var orders = _orderManagementService.GetOrdersForLiveNotifications(webinarId);

                _orderManagementService.FireSendConnectionInfoNotificationEvent(orders);
                return Json(new { Result = WebUiConstants.Success });
            }
            catch (Exception exception)
            {
                _logger.Error(string.Format("SendConnectionInfo Action: {0}", exception.Message), exception);
            }
            return Json(new { Result = WebUiConstants.Fail });
        }

        public PartialViewResult SendRecordingPosted()
        {
            var model = new AdhocNotificationViewModel
            {
                Webinars = EventInvokerHelpers.GetRecordedWebinarsAsSelectListItems(_webinarManagementService)
            };

            return PartialView(@"Partials/_SendRecordingPosted", model);
        }

        [System.Web.Mvc.HttpPost]
        public JsonResult SendRecordingPosted(int webinarId, string fileName)
        {
            try
            {
                //  Do something here with the file. Very exists in Azure. Also build connectionstring and save in OrderRow ?

                var orders = _orderManagementService.GetOrdersForRecordedNotifications(webinarId);

                if (orders.Any())
                {
                    _orderManagementService.FireSendRecordingIsPostedEvent(orders);

                    return Json(new { Result = WebUiConstants.Success });
                }

                return Json(new { Result = WebUiConstants.NoOrdersForWebinar });
            }
            catch (Exception exception)
            {
                _logger.Error(string.Format("SendRecordingPosted Action: {0}", exception.Message), exception);
            }
            return Json(new { Result = WebUiConstants.Fail });
        }


        //        [AcceptVerbs(HttpVerbs.Post)]
        //        public ActionResult Signup(int id
        //                                    , int mode
        //                                    , int? connectionsCount
        //                                    , int? sixMonthPaidConnectionsCount
        //                                    , int? twelveMonthPaidConnectionsCount
        //                                    , string currentUserEmail
        //            )
        //        {

        //            //        IDictionary<int, int> AdditionalLocation = Request.Params.AllKeys
        //            //.Where(x => x.StartsWith("ChooseOptions.AdditionalLocationCount"))
        //            //.Where(x => Request.Params[x] != null && Request.Params[x].ToString().Length > 0)
        //            //.Select(x => new { key = x.Replace(@"ChooseOptions.AdditionalLocationCount", ""), value = Request.Params[x] })
        //            //.ToDictionary(x => int.Parse(x.key), x => int.Parse(x.value));
        //            IDictionary<string, string> addEmails = Request.Params.AllKeys
        //                .Where(x => x.StartsWith("Email"))
        //                .Where(x => Request.Params[x] != null && Request.Params[x].ToString().Length > 0)
        //                .Select(x => new { key = x, value = Request.Params[x] })
        //                .ToDictionary(x => (x.key), x => (x.value));
        //            //IDictionary<int, int> sixMonthPaidAdditionalLocationCount = Request.Params.AllKeys
        //            //    .Where(x => x.StartsWith("ChooseOptions.SixMonthPaidAdditionalLocationCount"))
        //            //    .Where(x => Request.Params[x] != null && Request.Params[x].ToString().Length > 0)
        //            //    .Select(x => new { key = x.Replace(@"ChooseOptions.SixMonthPaidAdditionalLocationCount", ""), value = Request.Params[x] })
        //            //    .ToDictionary(x => int.Parse(x.key), x => int.Parse(x.value));
        //            //IDictionary<int, int> twelveMonthPaidAdditionalLocationCount = Request.Params.AllKeys
        //            //    .Where(x => x.StartsWith("ChooseOptions.TwelveMonthPaidAdditionalLocationCount"))
        //            //    .Where(x => Request.Params[x] != null && Request.Params[x].ToString().Length > 0)
        //            //    .Select(x => new { key = x.Replace(@"ChooseOptions.TwelveMonthPaidAdditionalLocationCount", ""), value = Request.Params[x] })
        //            //    .ToDictionary(x => int.Parse(x.key), x => int.Parse(x.value));

        ////            var checkoutSessionHelper = new CheckoutWorkflowHelper(ControllerContext);

        //            var currentOrder = new Order();
        //            var affiliate = stateService.GetValue<Affiliate>("CurrentAffiliate");
        //            currentOrder.Origin = "<p>InitialPage: " + HttpContext.Session["FirstPageOfSession"] + "</p><p>" +
        //                                   " InitialReferrer: " + HttpContext.Session["FirstReferrerOfSession"] + "</p><p>" +
        //                                    " InitialCookies: " + HttpContext.Session["FirstCookiesOfSession"] + "</p>";
        //            WebUser user;

        //            try
        //            {
        //               user = membershipService.GetUserByEmail(currentUserEmail);
        //            }
        //            catch (Exception)
        //            {
        //                throw;
        //            }

        //            _orderManagementService.AssignUserToOrder(currentOrder, user);
        //                currentOrder.AuditInfo = AppHelper.GetUserAuditInfo();
        //                currentOrder.Origin= _orderManagementService.GetOrderInitiator();
        //                //currentOrder.InitiatedBy2 = UserFacade.Instance.GetOrderInitiatorUser();

        //            var webinar = db.Webinars.Find(id);
        //            if (webinar.Title.Contains("Compliance Perspectives"))
        //            {
        //                webinar = db.Webinars.Find(883);
        //            }
        //            var orderRow = new OrderRow
        //            {
        //                Webinar = webinar,
        //                Order = currentOrder,
        //                AlternateEmail = String.Empty,
        //                RegistrationType = mode
        //            };
        //            //if (webinar.idWebinar == 842 && webinar.Status == WebinarStatus.Scheduled)
        //            //{
        //            //    try
        //            //    {
        //            //        OrderFacade.Instance.CreateCPSubscription(orderRow);

        //            //    }
        //            //    catch (Exception)
        //            //    {
        //            //        throw;
        //            //    }
        //            //}
        //            var AdditionalLocationAdditionalLocation = orderRow.AdditionalLocation.OfType<AdditionalLocationOrderRowOption>();
        //            if (AdditionalLocationAdditionalLocation.Count() > 0)
        //            {
        //                int AdditionalLocationCount;
        //                if (int.TryParse(Request["AdditionalLocationCount" + orderRow.idOrderRow], out AdditionalLocationCount))
        //                {
        //                    AdditionalLocationAdditionalLocation.Single().AdditionalLocationCount = AdditionalLocationCount;
        //                }
        //                else if (Request["AdditionalLocationCount" + orderRow.idOrderRow] == "")
        //                {
        //                    AdditionalLocationAdditionalLocation.Single().AdditionalLocationCount = 0;
        //                }
        //            }

        //            var options = _orderManagementService.GetOptionsByWebinarId(webinar.idWebinar);

        //            //var RegType = options.OfType<AdditionalLocationOption>().SingleOrDefault();
        //            var RegType = options.SingleOrDefault(o => o.Type == "additional_location");

        //            if (RegType != null)
        //            {
        //                var AdditionalLocation = new AdditionalLocationOrderRowOption
        //                {
        //                    AdditionalLocationCount = connectionsCount.HasValue ? connectionsCount.Value : 0,
        //                    RegType = RegType,
        //                    OrderRow = orderRow,
        //                    OptionDescription = RegType.OptionExplain,
        //                    RegTypePrice = Convert.ToDecimal(RegType.Price),
        //                    Emails = addEmails.Select(e => e.Value).ToList()
        //                };

        //                if (webinar.idWebinar == 842 && webinar.Status == WebinarStatus.Scheduled)
        //                {
        //                    ////CreateCPSubscription()
        //                    //int freeConnectionsCount;
        //                    //if (addEmails == "")
        //                    //{
        //                    //    freeConnectionsCount = 0;
        //                    //}
        //                    //else
        //                    //{
        //                    //    freeConnectionsCount = addEmails.Split(',').Count();
        //                    //}
        //                    ////int freeConnectionsCount = 0;
        //                    //AdditionalLocation.AdditionalLocationCount = freeConnectionsCount;
        //                    //if (AdditionalLocation.AdditionalLocationCount > 3)
        //                    //{
        //                    //    AdditionalLocation.AdditionalLocationCount = 3;
        //                    //}


        //                    //if ((RegistrationType)mode == RegistrationType.Twelve_Month_Subscription)
        //                    //{
        //                    //    AdditionalLocation.AdditionalLocationCount += twelveMonthPaidConnectionsCount.HasValue ? twelveMonthPaidConnectionsCount.Value : 0;
        //                    //}
        //                    //else
        //                    //{
        //                    //    AdditionalLocation.AdditionalLocationCount += sixMonthPaidConnectionsCount.HasValue ? sixMonthPaidConnectionsCount.Value : 0;
        //                    //}
        //                }

        //                orderRow.AdditionalLocation.Add(AdditionalLocation);
        //                //orderRow.Options.Add(AdditionalLocation);
        //            }

        //            //try
        //            //{
        //            //    OrderFacade.Instance.Save(currentOrder);
        //            //    OrderFacade.Instance.AddOrderRow(currentOrder, orderRow);

        //            //    Session["CurrentOrderId"] = currentOrder.ID;
        //            //    _checkoutWorkflow.AddOrder(currentOrder);

        //            //    //return RedirectToAction("Index", "Cart");
        //            //    

        //            //}
        //            //catch (RulesException ex)
        //            //{
        //            //    TempData["email"] = currentOrder.Email;
        //            //    Logger.Instance.LogException(ex);
        //            //    return RedirectToAction("ContactUsReDiscount", "Home");
        //            //}
        //return RedirectToAction("Signup2", "Cart");
        //        }

        public ActionResult SearchByTopic(
            [Core.DataTables.WebinarsBrowserRequestModelBinder] WebinarsBrowserRequestModel webinarsBrowserRequest)
        {

            var webinars = _webinarManagementService.GetByTopic(Convert.ToInt32(webinarsBrowserRequest.Search.Replace("TopicID=", "")));
            var wList = new List<WebinarsBrowserSearchResultEntryDTO>();
            foreach (var webinar in webinars)
            {
                var item = new WebinarsBrowserSearchResultEntryDTO();
                item.Webinar = webinar;
                item.RegistrationsCount = 0;
                wList.Add(item);
            }

            WebinarsBrowserSearchResultDTO searchResult = new WebinarsBrowserSearchResultDTO();
            searchResult.Entries = wList;
            searchResult.FoundCount = 2;
            searchResult.TotalCount = webinars.Count();

            var data = new WebinarsSearchDTOAssembler(webinarsBrowserRequest.EchoId).Entity2DTO(searchResult);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        //[AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Play(int w, int u)
        {
            if (_orderManagementService.CheckUserForRecordingAccess(u, w) > 0 || u == 19)
            {
                Webinar webinar = _webinarManagementService.GetWebinar(w);

                //how do I ensure that all child objects are included?
                var webinarFiles = webinar.WebinarFiles.Where(f => f.idWebinar == w)
                    .Select(f => f.fileDesc + "|" + f.fileLocation)
                    .ToArray();

                ViewBag.WebinarFiles = webinarFiles;

                Presenter presenter = webinar.Presenter;

                ViewBag.Presenter = presenter;

                return View(webinar);
            }

            ViewData["Expired"] = "This recording has expired. ";
            return View();
        }

        //public ActionResult ConnectionDetails(int id)
        //{
        //    try
        //    {
        //        //string connectionInfo = NotificationFacade.Instance.PreviewNotification(
        //        //    TemplateTypes.CONNECTION_INFORMATION, null, id, null);
        //        //return View((object)connectionInfo);
        //    }
        //    catch (EntityNotFoundException)
        //    {
        //        return View("MissingRecord", (object)("Webinar with ID=" + id + " does not exist!"));
        //    }
        //}

        public ActionResult Details(int id)
        {
            //_orderManagementService.GetOrderById(1162);
            ViewBag.userHasOpenOrder = 0;
            ViewBag.userOwnsThisEvent = 0;
            ;
            ViewBag.upList = null;
            ViewBag.regList = null;



            ViewBag.PageStyleType = "holy-grail-three-columns";
            //
            WebUser user = Request.IsAuthenticated ? _membershipService.GetUserByEmail(User.Identity.Name) : new WebUser();

            var usersOrders = _orderManagementService.GetOrdersByUserId(user.idUser).Where(o => o.OrderRows.SingleOrDefault(or => or.idWebinar == id) != null);

            var options = _orderManagementService.GetOptionsByWebinarId(id, false);

            var webinar = _webinarManagementService.GetWebinarByIdIncludingAllWebinarsByPresenter(id);
            if (webinar == null) return HttpNotFound();



            ViewBag.topics = _webinarManagementService.GetTopicsPerWebinar(webinar.idWebinar);

            var model = new WebinarDetailsViewModel()
            {
                WebUser = user
                ,
                Affiliate = stateService.GetValue<Affiliate>("CurrentAffiliate")
                ,
                Webinar = webinar
                ,
                Options = options
                ,
                Order = null
            };

            if (usersOrders != null && usersOrders.Any())
            {
                foreach (var checkOrder in usersOrders)
                {
                    //if (checkOrder.OrderRows.Single().idWebinar == id)


                    // OrderRow has a idRegType (the FK)
                    // but does not have an instantiated RegType object.

                    var row = checkOrder.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active);
                    //if (row != null)
                    //    ViewBag.orderMessages = row.RegistrationType;
                    var webinarFiles = webinar.WebinarFiles
                            .Select(f => f.fileDesc + "|" + f.fileLocation)
                            .ToArray();

                    ViewBag.WebinarFiles = webinarFiles;

                    ViewBag.userOwnsThisEvent = checkOrder.idOrder;
                    model.Order = checkOrder;

                    //var connectionText = new StringBuilder("<p>");
                    //connectionText.Append(
                    //    row.RegistrationType.Stage2EmailConfirmationMsg.Replace(
                    //        " and is also available at http://www.@Tenant.com", "</p><p>"));

                    ////connectionText.Append(webinar.ConnectionInfo.Replace(Environment.NewLine, "<br>"));
                    //connectionText.Append("</p>");

                    //ViewBag.connectionText = connectionText;


                    if (checkOrder.OrderStatus == OrderStatus.InProcess && row.idWebinar != id)
                    {
                        ViewBag.userHasOpenOrder = checkOrder.idOrder;
                        model.Order = checkOrder;
                    }
                }
            }

            if (model.Webinar == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        public ActionResult Details2(int id)
        {
            ViewBag.PageStyleType = "holy-grail-three-columns";
            var webinar = _webinarManagementService.GetWebinarByIdIncludingAllWebinarsByPresenter(id);

            if (webinar == null) return HttpNotFound();
            ViewBag.topics = _webinarManagementService.GetTopicsPerWebinar(webinar.idWebinar);
            ViewBag.userHasOpenOrder = 0;
            ViewBag.userOwnsThisEvent = 0;
            ;
            ViewBag.upList = null;
            ViewBag.regList = null;

            WebUser user = Request.IsAuthenticated ? _membershipService.GetUserByEmail(User.Identity.Name) : new WebUser();
            var usersOrders = _orderManagementService.GetOrdersByUserId(user.idUser)
                                .Where(o => o.OrderRows.SingleOrDefault(or => or.idWebinar == id) != null);

            var options = _orderManagementService.GetOptionsByWebinarId(id, false);

            var model = new WebinarDetailsViewModel()
            {
                WebUser = user,
                Affiliate = stateService.GetValue<Affiliate>("CurrentAffiliate"),
                Webinar = webinar,
                Options = options,
                Order = null
            };

            if (usersOrders.Any())
            //if (usersOrders != null && usersOrders.Any())
            {
                foreach (var checkOrder in usersOrders)
                {
                    var row = checkOrder.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active);
                    
                    var webinarFiles = webinar.WebinarFiles
                            .Select(f => f.fileDesc + "|" + f.fileLocation)
                            .ToArray();

                    ViewBag.WebinarFiles = webinarFiles;

                    ViewBag.userOwnsThisEvent = checkOrder.idOrder;
                    model.Order = checkOrder;
                    
                    if (checkOrder.OrderStatus == OrderStatus.InProcess && row.idWebinar != id)
                    {
                        ViewBag.userHasOpenOrder = checkOrder.idOrder;
                    }
                }
            }

            if (model.Webinar == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        public ActionResult Calendar(int? ID)
        {
            return View();
        }

        [System.Web.Mvc.AcceptVerbs(HttpVerbs.Get)]
        public ActionResult CalendarData()
        {
            //IList<Webinar> webinarsList = WebinarFacade.Instance.SelectAllActiveWebinars();
            IList<Webinar> webinarsList = _webinarManagementService.GetAllActive().ToList();

            var dtos = new CalendarDTOAssembler().Entities2DTOs(webinarsList);

            TempData["ListUpcoming"] = webinarsList;

            return Json(dtos, JsonRequestBehavior.AllowGet);
        }


        //
        // GET: /Webinar/Create

        public ActionResult Create()
        {
            ViewBag.PresenterId = new SelectList(_webinarManagementService.GetAllPresenters(), "Id", "Biography");
            return View();
        }

        //
        // POST: /Webinar/Create

        [System.Web.Mvc.HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Webinar webinar)
        {
            if (ModelState.IsValid)
            {
                _webinarManagementService.AddWebinar(webinar);
                return RedirectToAction("Index");
            }

            ViewBag.PresenterId = new SelectList(_webinarManagementService.GetAllPresenters(), "Id", "Biography", webinar.idPresenter);
            return View(webinar);
        }

        //
        // GET: /Webinar/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Webinar webinar = _webinarManagementService.GetWebinar(id);
            if (webinar == null)
            {
                return HttpNotFound();
            }
            ViewBag.PresenterId = new SelectList(_webinarManagementService.GetAllPresenters(), "Id", "Biography", webinar.idPresenter);
            return View(webinar);
        }

        //
        // POST: /Webinar/Edit/5

        [System.Web.Mvc.HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Webinar webinar)
        {
            if (ModelState.IsValid)
            {

                return RedirectToAction("Index");
            }
            ViewBag.PresenterId = new SelectList(_webinarManagementService.GetAllPresenters(), "Id", "Biography", webinar.idPresenter);
            return View(webinar);
        }

        //
        // GET: /Webinar/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Webinar webinar = _webinarManagementService.GetWebinar(id);
            if (webinar == null)
            {
                return HttpNotFound();
            }
            return View(webinar);
        }


        //
        // POST: /Webinar/Delete/5

        [System.Web.Mvc.HttpPost, System.Web.Mvc.ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            _webinarManagementService.DeleteWebinar(id);
            return RedirectToAction("Index");
        }

        [System.Web.Mvc.HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateConnectionInfo(Webinar model)
        {
            Webinar webinar = model;

            //_db.SaveChanges();

            return RedirectToAction("Details");
            //return PartialView("Partials/_UpdateConnectionInfo",webinar);
        }

        public ActionResult CreateICSForWebinar(int id)
        {
            try
            {
                var webinar = _webinarManagementService.GetWebinarByIdIncludingAllWebinarsByPresenter(id);

                string filePath = HostingEnvironment.MapPath("~/App_Data/ics/");

                string defaultDescription = "1. Click this link to start or to join the Webinar:<br><br>https://www2.gotomeeting.com/ojoin/325710074/922930 <br><br><br>2. Choose one of the following audio options:<br><br> TO USE YOUR COMPUTER’S AUDIO:<br> When the Webinar begins, you will be connected to audio using your computer’s microphone and speakers (VoIP). A headset is recommended.<br><br><br> TO USE YOUR TELEPHONE:<br> If you prefer to use your phone, you must select “Use Telephone” after joining the Webinar and call in using the numbers below.<br><br><br> Toll-free: 1 877 568 4108<br> Access Code: 529-918-465<br> Audio PIN: Shown after joining the meeting<br><br><br>GoToWebinar®<br>Webinars Made Easy™<br>";
                defaultDescription += webinar.DescriptionLong;

                string presentedBy = string.Empty;

                if (webinar.Presenter != null)
                    presentedBy = "<br>Presented By:<br>" + webinar.Presenter.Biography;

                // Now Contruct the ICS file using string builder
                StringBuilder str = new StringBuilder();

                str.AppendLine("BEGIN:VCALENDAR");
                str.AppendLine("PRODID:-//Schedule a Meeting");
                str.AppendLine("VERSION:2.0");
                //str.AppendLine("METHOD:REQUEST");
                str.AppendLine("METHOD:PUBLISH");

                //str.AppendLine("BEGIN:VTIMEZONE");
                //str.AppendLine("TZID:Pacific Time (US & Canada)");
                //str.AppendLine("BEGIN:STANDARD");
                //str.AppendLine("DTSTART:20061105T020000");
                //str.AppendLine("RRULE:FREQ=YEARLY;BYDAY=1SU;BYMONTH=11");
                //str.AppendLine("TZOFFSETFROM:-0700");
                //str.AppendLine("TZOFFSETTO:-0800");
                //str.AppendLine("END:STANDARD");
                //str.AppendLine("BEGIN:DAYLIGHT");
                //str.AppendLine("DTSTART:20070311T020000");
                //str.AppendLine("RRULE:FREQ=YEARLY;BYDAY=2SU;BYMONTH=3");
                //str.AppendLine("TZOFFSETFROM:-0800");
                //str.AppendLine("TZOFFSETTO:-0700");
                //str.AppendLine("TZNAME:Daylight Savings Time");
                //str.AppendLine("END:DAYLIGHT");
                //str.AppendLine("END:VTIMEZONE");

                str.AppendLine("BEGIN:VEVENT");

                str.AppendLine(string.Format("DTSTART:{0:yyyyMMddTHHmmssZ}", webinar.Date));

                //str.AppendLine(string.Format("DTSTAMP:{0:yyyyMMddTHHmmssZ}", DateTime.UtcNow));
                str.AppendLine(string.Format("DTSTAMP:{0:yyyyMMddTHHmmssZ}", webinar.Date));

                str.AppendLine(string.Format("DTEND:{0:yyyyMMddTHHmmssZ}", webinar.Date.AddHours(Convert.ToDouble(webinar.Duration))));

                str.AppendLine("LOCATION;ENCODING=QUOTED-PRINTABLE:Webinar – See conference call information below");

                str.AppendLine(string.Format("UID:{0}", Guid.NewGuid()));

                str.AppendLine(string.Format("DESCRIPTION:{0}", defaultDescription + presentedBy));

                str.AppendLine(string.Format("X-ALT-DESC;FMTTYPE=text/html:{0}", defaultDescription + presentedBy));

                str.AppendLine(string.Format("SUMMARY:{0}", webinar.Title));

                str.AppendLine("BEGIN:VALARM");
                str.AppendLine("TRIGGER:-PT15M");
                str.AppendLine("ACTION:DISPLAY");
                str.AppendLine("DESCRIPTION:Reminder");
                str.AppendLine("END:VALARM");
                str.AppendLine("END:VEVENT");
                str.AppendLine("END:VCALENDAR");

                //Code for save ics file in application
                filePath += "w_" + id.ToString() + ".ics";
                TextWriter tw = new StreamWriter(filePath);

                // write a line of text to the file
                tw.WriteLine(str.ToString());

                // close the stream
                tw.Close();
                //Code for save ics file in application
                return Json(new { success = true, result = "Successfully Created." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                _logger.Error("/Webinar/CreateICSForWebinar: " + e.Message);
                return Json(new { success = false, result = "File creation failed." }, JsonRequestBehavior.AllowGet);
            }

        }

        protected override void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                var logger = _logger as IDisposable;
                if (logger != null)
                    logger.Dispose();

                _membershipService.Dispose();
                _orderManagementService.Dispose();

                base.Dispose(true);
            }
            _disposed = true;
        }
    }
}
