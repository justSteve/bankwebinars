using System.Data.Entity.Validation;
using System.Text.RegularExpressions;
using System.Threading;
using BrockAllen.MembershipReboot;
using CUWebinars.Business.AccountService;
using CUWebinars.Business.Constants;
using CUWebinars.Business.Core.Helpers;
using CUWebinars.Business.Models;
using CUWebinars.Business.Repository;
using CUWebinars.Business.Services;
using CUWebinars.Web.Core;
using CUWebinars.Web.Core.Browsers.Webinars;
using CUWebinars.Web.Core.DataTables;
using CUWebinars.Web.Core.Orchestrators;
using CUWebinars.Web.Helpers;
using CUWebinars.Web.Infrastructure.Attributes;
using CUWebinars.Web.Infrastructure.Extensions;
using CUWebinars.Web.Mapping.Mappers;
using CUWebinars.Web.Models;
using CUWebinars.Web.Services;
using CUWebinars.Web.ViewModel;
using FluentValidation;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Web.Hosting;
using System.Web.Mvc;
using WebGrease.Css.Extensions;
using ClaimTypes = CUWebinars.Business.Constants.ClaimTypes;
using DateTimeHelper = CUWebinars.Web.Helpers.DateTimeHelper;


namespace CUWebinars.Web.Controllers
{
    public class WebinarController : Controller
    {
        private const string FromFluentPrefix = "fromfluent-";
        private const string EnterValidIdMsg = "Enter a valid id";
        private const string ServerErrorLoggedMsg = "Server Error Logged";
        public const string WebinarFromCode = "WebinarFromCode";
        private IStateService _stateService;
        private readonly IWebinarControllerOrchestrator _webinarControllerOrchestrator;
        private readonly IAppHelper _appHelper;
        private readonly IUniversalMapper _universalMapper;
        private GlobalConfig _globalConfig = GlobalConfig.GlobalConfigSingletonCreator.UniqueInstance;

        //Steve added MembershipService dependancy to allow for 'currentUser' in Details.
        private readonly IMembershipService _membershipService;
        private readonly IOrderManagementService _orderManagementService;
        private readonly IWebinarManagementService _webinarManagementService;

        private readonly IAffiliateManagementService _affiliateManagementService;
        private readonly ILogger _logger;
        private bool _disposed;

        public WebinarController(
            IAffiliateManagementService affiliateManagementService,
            IMembershipService membershipService,
            IOrderManagementService orderManagementService,
            IWebinarManagementService webinarManagementService,
            ILogger logger,
            IStateService stateService,
            IWebinarControllerOrchestrator webinarControllerOrchestrator,
            IAppHelper appHelper,
            IUniversalMapper universalMapper)
        {
            _affiliateManagementService = affiliateManagementService;
            _membershipService = membershipService;
            _orderManagementService = orderManagementService;
            _webinarManagementService = webinarManagementService;
            _logger = logger;
            _stateService = stateService;
            _webinarControllerOrchestrator = webinarControllerOrchestrator;
            _appHelper = appHelper;
            _universalMapper = universalMapper;
        }


        [ValidateInput(false)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GenerateHTML4PerDayVR(int ID)
        {
            var webinar = _webinarManagementService.GetWebinar(ID);

            IList<Webinar> webinarsList = _webinarManagementService.GetUpcomingWebinars().ToList();

            DateTime cDate = Convert.ToDateTime(Request["cDate"]).AddHours(22);

            StringBuilder upcoming = new StringBuilder();
            int i = 0;
            foreach (Webinar x in webinarsList)
            {
                if (x.idWebinar != ID && i < 5 && x.Date > cDate)
                {
                    i++;
                    upcoming.Append("<p><a href=\"http://www.bankwebinars.com/Webinar/Details/" + x.idWebinar +
                                    "?idaff=" + Request["idAffiliate"].ToString() + "\">");
                    upcoming.Append(x.Title + "</a><br><font size='-3'> (" + x.Date.ToLongDateString() + ")</font></p>");
                }
            }

            USTimeZone tz = USTimeZone.Central;
            switch (Request["tz"].ToString())
            {
                case "Eastern":
                    tz = USTimeZone.Eastern;
                    break;
                case "Central":
                    tz = USTimeZone.Central;
                    break;
                case "Mountain":
                    tz = USTimeZone.Mountain;
                    break;
                case "Pacific":
                    tz = USTimeZone.Pacific;
                    break;
            }
            string wDate = "<b>" + DateTimeHelper.FormatDate(webinar.Date) + "</b><br>";
            wDate = wDate + DateTimeHelper.FormatTimeWithDuration(webinar.Date, tz, false, webinar.Duration) + "<br>";

            IDictionary<string, string> VRObject = new Dictionary<string, string>();

            string ceu;
            if (String.IsNullOrEmpty(webinar.ceu) == false)
            {
                string[] ceufull = webinar.ceu.Split('|');
                ceu = ceufull[0].ToString();
                VRObject.Add("ceu", ceu);
            }
            VRObject.Add("desc", webinar.Description);
            VRObject.Add("upcoming", upcoming.ToString());
            VRObject.Add("cDate", cDate.ToShortDateString());
            VRObject.Add("wDate", wDate);
            VRObject.Add("presenterImage", webinar.Presenter.PhotoFull);
            VRObject.Add("presenter", webinar.Presenter.WebUser.FullName);
            VRObject.Add("presenterBio", webinar.Presenter.BiographyLong);
            VRObject.Add("title", webinar.Title);
            VRObject.Add("ID", ID.ToString());
            VRObject.Add("from", Request["from"]);
            VRObject.Add("logo", Request["logo"]);
            VRObject.Add("url", Request["url"]);
            VRObject.Add("description", webinar.Description);
            VRObject.Add("learnCaption", webinar.LearnCaption);
            VRObject.Add("learnBody", webinar.LearnBody);
            VRObject.Add("whoAttend", webinar.WhoAttend);
            VRObject.Add("cost", "$265");
            VRObject.Add("contact", Request["contact"]);
            VRObject.Add("email", Request["email"]);
            VRObject.Add("phone", Request["phone"]);
            VRObject.Add("idAffiliate", Request["idAffiliate"]);
            VRObject.Add("social", Request["social"]);
            VRObject.Add("footer", Request["footer"]);




            //string myHTML = NotificationFacade.Instance.SendVRPerDay(TemplateTypes.VR_PER_DAY, VRObject, adminEmail, "VR Code for " + Request["from"].ToString());


            //return Content(myHTML);
            return null;

        }

        [ValidateInput(false)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GenerateHTML4PerWeek()
        {

            IEnumerable<int> featured = AppHelper.StringToIntList(Request["featuredWebinars"]);

            IList<Webinar> webinarsList = _webinarManagementService.GetUpcomingWebinars().ToList();

            DateTime cDate;
            cDate = Convert.ToDateTime(Request["cDate"]).AddHours(5);
            StringBuilder upcoming = new StringBuilder();
            StringBuilder upcomingDetail = new StringBuilder();

            USTimeZone tz = USTimeZone.Central;
            switch (Request["tz"].ToString())
            {
                case "Eastern":
                    tz = USTimeZone.Eastern;
                    break;
                case "Central":
                    tz = USTimeZone.Central;
                    break;
                case "Mountain":
                    tz = USTimeZone.Mountain;
                    break;
                case "Pacific":
                    tz = USTimeZone.Pacific;
                    break;
            }

            //create featured listings
            foreach (int w in featured)
            {
                var webinar = _webinarManagementService.GetWebinar(w);

                string wDate = "<b>" + DateTimeHelper.FormatDate(webinar.Date) + "</b><br>";
                wDate = wDate + DateTimeHelper.FormatTimeWithDuration(webinar.Date, tz, false, webinar.Duration) +
                        "<br>";

                upcomingDetail.Append(
                    "<p style='font-size:20px; font-weight:bold; color:#CC6600; font-family:trebuchet ms;'><a href='http://www.bankwebinars.com/Webinar/Details/" +
                    webinar.idWebinar + "?idaff=#idAffiliate#'>" + webinar.Title + "</a></p>");
                upcomingDetail.Append(
                    "<p style='color:#FFFF99; padding-bottom:5px; padding-top:5px; background-color:#333333; text-align:center; font-size:9px'>" +
                    wDate + "</p>");
                upcomingDetail.Append("<p style='font-family:trebuchet ms;'>" + webinar.Description + "</p>");
                upcomingDetail.Append("<p style='font-family:trebuchet ms;'><b>" + webinar.Presenter.WebUser.FullName +
                                      "</b></p>");
                upcomingDetail.Append(
                    "<p font-family:trebuchet ms;'><a href='http://www.bankwebinars.com/Webinar/Details/" +
                    webinar.idWebinar + "?idaff=#idAffiliate#'>Click here for more info!</a></p>");
            }


            int i = 0;
            foreach (Webinar x in webinarsList)
            {

                if (!featured.Contains(x.idWebinar) && i < 5 && x.Date > cDate)
                {
                    i++;
                    upcoming.Append("<p><a href=\"http://www.bankwebinars.com/Webinar/Details/" + x.idWebinar +
                                    "?idaff=" + Request["idAffiliate"].ToString() + "\">");
                    upcoming.Append(x.Title + "<br><font size='smaller'> (" + x.Date.ToLongDateString() +
                                    ")</font></a></p>");
                }
            }



            IDictionary<string, string> VRObject = new Dictionary<string, string>();
            //var VRObject = AppHelper.DictionaryFromAnonymousObject(webinar);

            VRObject.Add("upcoming", upcoming.ToString());
            VRObject.Add("cDate", cDate.ToLongDateString());
            VRObject.Add("upcomingDetailed", upcomingDetail.ToString());
            VRObject.Add("tz", Request["tz"].ToString());
            VRObject.Add("from", Request["from"]);
            VRObject.Add("logo", Request["logo"]);
            VRObject.Add("url", Request["url"]);
            VRObject.Add("contact", Request["contact"]);
            VRObject.Add("email", Request["email"]);
            VRObject.Add("phone", Request["phone"].ToString());
            VRObject.Add("idAffiliate", Request["idAffiliate"].ToString());
            VRObject.Add("social", Request["social"].ToString());
            VRObject.Add("footer", Request["footer"].ToString());


            string adminEmail = "steve@ttstrain.com";

            //string myHTML = NotificationFacade.Instance.SendVRPerDay(TemplateTypes.VR_PER_WEEK, VRObject, adminEmail, "VR Code for " + Request["from"].ToString());

            //string[] myReturn =  {"myString", myHTML};

            //return Content(myHTML);
            return null;
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

            var webinars = _webinarManagementService.GetByTopic(ID);
            //var dtos = new WebinarDTOAssembler().Entities2DTOs(webinars);
            //return View(dtos);
            return View(webinars);
        }

        public ActionResult Search()
        {
            try
            {
                ViewBag.PageStyleType = "two-columns-right-sidebar";
                ViewBag.TopicCaption = " ";
                string searchTerm = Request["searchTerm"];

                var unionOfResultSets = _webinarControllerOrchestrator.SearchWebinars(searchTerm);

                ViewBag.Title = "Search Results";

                return View(unionOfResultSets);
            }
            catch (Exception exception)
            {
                _logger.ErrorException(string.Format("Search | Session {0}", _appHelper.GetUserAuditInfo()), exception);
                ModelState.AddModelError(string.Empty, WebUiConstants.ServerErrorWithAssistNumber);
                throw;
            }
        }

        public ActionResult AllActive(string eventsToShow)
        {
            var webinars = _webinarManagementService.GetAllActive();
            ViewBag.TopicCaption = " ";
            ViewBag.Title = "All Listed Events for " + _globalConfig.Tenant;

            if (eventsToShow == "upcoming")
            {
                webinars = _webinarManagementService.GetUpcomingWebinars().OrderBy(w => w.Date);
                ViewBag.Title = "All Upcoming Events for " + _globalConfig.Tenant;
            }

            if (eventsToShow == "recorded")
            {
                webinars = _webinarManagementService.GetRecordedWebinars().OrderBy(w => w.Date);
                ViewBag.Title = "All Recorded Events for " + _globalConfig.Tenant;
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
            var model = _webinarControllerOrchestrator.BuildAdhocNotificationViewModel(WebinarType.Upcoming);

            return PartialView(@"Partials/_SendConnectionInfo", model);
        }

        [System.Web.Mvc.HttpPost]
        public JsonResult SendConnectionInfo(int webinarId)
        {
            try
            {
                _webinarControllerOrchestrator.FireSendConnectionInfoNotificationEvent(webinarId);

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

            //is this depricated?
            throw new NotImplementedException();
            //var model = _webinarControllerOrchestrator.BuildAdhocNotificationViewModel(WebinarType.Recorded);

            //return PartialView(@"Partials/_SendRecordingPosted", model);
        }


        [System.Web.Mvc.HttpPost]
        public JsonResult SendRecordingPosted(int webinarId)
        {
            //is this depricated?
            throw new NotImplementedException();
            //try
            //{
            //    String setEventToRecorded = _webinarControllerOrchestrator.SetEventToRecorded(webinarId);
            //}
            //catch (Exception)
            //{

            //    throw;
            //}
            //try
            //{

            //    var orders = _orderManagementService.GetOrdersForRecordedNotifications(webinarId);

            //    if (orders.Any())
            //    {
            //        _orderManagementService.FireSendRecordingIsPostedEvent(orders);

            //        return Json(new { Result = WebUiConstants.Success });
            //    }

            //    return Json(new { Result = WebUiConstants.NoOrdersForWebinar });
            //}
            //catch (Exception exception)
            //{
            //    _logger.Error(string.Format("SendRecordingPosted Action: {0}", exception.Message), exception);
            //}
            //return Json(new { Result = WebUiConstants.Fail });
        }


        public ActionResult SearchByTopic(
            [Core.DataTables.WebinarsBrowserRequestModelBinder] WebinarsBrowserRequestModel webinarsBrowserRequest)
        {

            var webinars =
                _webinarManagementService.GetByTopic(
                    Convert.ToInt32(webinarsBrowserRequest.Search.Replace("TopicID=", "")));
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

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult OnDemand(int? id)
        {
            if (id.HasValue)
            {
                var order = _orderManagementService.GetOrderById(id.Value);

                var webinar =
                    _webinarManagementService.GetWebinar(
                        order.OrderRows.SingleOrDefault(s => s.RowStatus == OrderRowStatus.Active).idWebinar);

                var playModel = new OnDemandPlaybackModel
                {
                    Presenter = webinar.Presenter,
                    Webinar = webinar
                };

                ClaimsIdentity claimsIdentityOfAuthenticatedUser = (ClaimsIdentity)User.Identity;

                if (claimsIdentityOfAuthenticatedUser.IsAuthenticated)
                {
                    if (claimsIdentityOfAuthenticatedUser.HasClaim((claim) => claim.Type == ClaimTypes.Admin))
                    {
                        return RedirectToAction("Index", "Admin");
                    }

                    ProcessAccessPermissionsForMaterials(order, playModel);

                    //if (accessPermitted)
                    //{

                    //    playModel.WebinarFiles = playModel.Webinar.WebinarFiles.Where(f => f.idWebinar == order.OrderRows.FirstOrDefault().idWebinar)
                    //        .Select(f => f.fileDesc + "|/webinar/GetWebinarFile?idWebinarFile=" + f.idWebinarFile)
                    //        .ToArray();

                    //    return View(playModel);
                    //}

                    ViewData["Expired"] = "This recording has expired. ";
                    return View(playModel);
                }

                if (_stateService.HasValue(WebUiConstants.AnonUserIdentified) && _stateService.GetValue<bool>(WebUiConstants.AnonUserIdentified))
                {
                    ProcessAccessPermissionsForMaterials(order, playModel);

                    return View(playModel);
                }

                return RedirectToAction("Identify", new { orderId = id.Value });
            }

            //TODO: implement response for error condition. Here, no id passed in.
            return null;
        }

        private void ProcessAccessPermissionsForMaterials(Order order, OnDemandPlaybackModel playModel)
        {
            var webUser = _membershipService.GetWebUserById(order.idUser);

            if (order.idUser == 19)
            {
                playModel.AuthorizedToAccessMaterials = true;
            }
            else if (webUser != null)
            {
                string messageIfFalse;

                if (_membershipService.CheckDisplayPostEventMaterials(
                    _globalConfig.Tenant,
                    webUser.email,
                    out messageIfFalse))
                {
                    playModel.AuthorizedToAccessMaterials = true;
                }
                else
                {
                    playModel.AuthorizedToAccessMaterials = false;
                    _logger.Error(messageIfFalse);
                }
            }
            else
            {
                _logger.Error("No WebUser exists with the Id {0}", order.idUser);
                ModelState.AddModelError(string.Empty,
                    string.Format("No WebUser exists with the Id {0}", order.idUser));
            }
        }

        public ActionResult Identify(int orderId)
        {
            var model = new IdentifyModel
            {
                Email = string.Empty,
                idOrder = orderId,
                FullName = string.Empty,
                SignInModel = new SignInModel
                {
                    ReturnUrl = "Webinar/OnDemand/" + orderId
                }
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Identify(IdentifyModel identifyModel)
        {
            if (ModelState.IsValid)
            {
                // do something with name and email address
                var order = _orderManagementService.GetOrderById(identifyModel.idOrder);

                if (!ReferenceEquals(null, order))
                {

                    JObject existingJObject = null;

                    string comments = string.Empty;


                    if (!ReferenceEquals(null, order.AdminComments))
                    {
                        comments = order.AdminComments.Trim();
                    }

                    var newJson = new JProperty(string.Concat("PostEventAccessByAnonUser-", DateTime.Now.ToString(DomainConstants.DateTimeLongFormat)),
                        new JObject(
                            new JProperty("Name", identifyModel.FullName),
                            new JProperty("Email", identifyModel.Email)
                            ));

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

                    _orderManagementService.SaveChanges();

                    _stateService.SetValue(WebUiConstants.AnonUserIdentified, true);

                    return RedirectToAction("OnDemand", new { id = identifyModel.idOrder });
                }
            }

            return View(identifyModel);
        }

        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult RegistrationsTableDataLoader(
        //    [DataTablesRequestModelBinder] DataTablesRequestModel dataTablesRequest)
        //{
        //    int currentUserID = 62;
        //    var searchResult = _orderManagementService.SearchRegistrations
        //        (
        //            currentUserID,
        //            null,
        //            dataTablesRequest.DisplayStart,
        //            dataTablesRequest.DisplayLength,
        //            dataTablesRequest.Search
        //        );

        //    var data = new RegistrationsBrowserTableDataDTOAssembler(dataTablesRequest.EchoId).Entity2DTO(searchResult);
        //    return Json(data);
        //}

        public ActionResult Details(int? id)
        {
            if (id.HasValue)
            {

                var webinar = _webinarManagementService.GetWebinarByIdIncludingAllWebinarsByPresenter(id.Value);

                if (webinar == null) return HttpNotFound();

                var model = new WebinarDetailsViewModel()
                {
                    Webinar = webinar,
                    WebinarFiles = webinar.WebinarFiles.ToList()
                };

                InitializeDetailsState(webinar, model, id.Value); // form state, incl. stuff that will be posted back. 
                InitializeViewCentricProperties(model);
                // mostly just stuff that helps determine layout of the page on load. Not meant to be sent back here to Server from the View
                if (model.Webinar == null)
                {
                    return HttpNotFound();
                }

                ClaimsIdentity claimsIdentityOfAuthenticatedUser = (ClaimsIdentity)User.Identity;

                if (claimsIdentityOfAuthenticatedUser.HasClaim(
                        (claim) => claim.Type == Business.Constants.ClaimTypes.Admin))
                {
                    return PartialView("DetailsAdmin", model);
                }

                if (claimsIdentityOfAuthenticatedUser.HasClaim(
                        (claim) => claim.Type == Business.Constants.ClaimTypes.Affiliate))
                {

                    // Get the claims values
                    var ttsDomain = claimsIdentityOfAuthenticatedUser.Claims
                        .Where(c => c.Type == ClaimTypes.Affiliate)
                        .Select(c => c.Value)
                        .SingleOrDefault();

                    var aff = _affiliateManagementService.LoadByTTSDomain(ttsDomain);

                    var orders = _orderManagementService.GetOrdersForWebinar(model.Webinar.idWebinar)
                        .Where(o => o.Affiliate == aff)
                        .ToList();

                    model.ShowOrdersViewModel = new ShowOrdersViewModel
                    {
                        Affiliate = aff,
                        Orders = orders,
                        Webinar = model.Webinar
                    };
                    return PartialView("DetailsAffiliate", model);
                }

                var usersOrders = _orderManagementService.GetOrdersByUserId(model.WebUser.idUser)
                    .Where(o => o.OrderRows.SingleOrDefault(or => or.idWebinar == id.Value) != null);

                // perf tweak: ensures no multiple enumerations of usersOrders
                var checkOrders = usersOrders as Order[] ?? usersOrders.ToArray();

                if (checkOrders.Any())
                // Webinar.Status > scheduled - WebinarFiles presenter files etc. Files only exist until init or activated Webinar
                {
                    foreach (var checkOrder in checkOrders) // assumption that there will be only 1 ?
                    {
                        var row = checkOrder.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active);

                        var webinarFiles = webinar.WebinarFiles
                            .Select(f => f.fileDesc + "|" + f.fileLocation)
                            .ToArray();

                        if (row.idWebinar == id)
                        {
                            var userAccount = _membershipService.GetUserAccountByEmail(_globalConfig.Tenant, checkOrder.WebUser.email);

                            var postEventAccessExpireyDate = _membershipService.GetPostEventAccessExpireyDate(userAccount, checkOrder.idOrder);

                            if (postEventAccessExpireyDate != null && postEventAccessExpireyDate.Value >= DateTime.Today)
                            {
                                model.RegistrationSummaryViewModel.DisplayPostEventMaterials = checkOrder.idOrder;
                            };

                            model.RegistrationSummaryViewModel.WebinarFiles = webinarFiles;
                            model.RegistrationSummaryViewModel.UserOwnsThisEvent =
                                model.UserOwnsThisEvent = checkOrder.idOrder;
                            model.RegistrationSummaryViewModel.DisplayPostEventMaterials = 0;
                            model.Order = checkOrder;
                        }

                        if (checkOrder.OrderStatus == OrderStatus.InProcess && row.idWebinar != id)
                        {
                            model.UserHasOpenOrder = checkOrder.idOrder;
                        }
                    }
                }


                /*****************************************************************************************/
                /* Significance of next 'if': must complete cart transaction before commencing a new one.*/
                /*****************************************************************************************/
                if (model.UserOwnsThisEvent > 0 && model.Order.OrderStatus == OrderStatus.InProcess)
                {
                    model.CheckoutInProcess = true;
                    model.MessageOrderStatus =
                        "<div align=\"center\" class=\"label-warning label\">Your order is InProcess and needs to be confirmed or canceled.</div>";

                    var additionalLocationsViewModel =
                        model.RegistrationSummaryViewModel.OrderHasAdditionalLocationsViewModel;
                    var orderRow = model.Order.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active);
                    var order = orderRow.Order;
                    var webUser = orderRow.Order.WebUser;
                    var userFullName = string.Concat(webUser.FirstName, " ", webUser.LastName);
                    var addresses = webUser.Addresses.ToArray();
                    var billingAddress = addresses.First(a => a.AddressType == WebUiConstants.BillingAddress);
                    var shippingAddress = addresses.FirstOrDefault(a => a.AddressType == WebUiConstants.ShippingAddress);
                    //var CheckoutDiscount = orderRow.Discount == null ? string.Empty : _orderManagementService.GetDiscountByCode()

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
                                string.Concat("LegacyCommentsInDetails-", DateTime.Now.ToString(DomainConstants.DateTimeLongFormat)),
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

                        _orderManagementService.SaveChanges();
                        model.CheckoutConfirmViewModel = new CheckoutConfirmViewModel
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
                            //AffiliateComments = model.Order.AffiliateComments,
                            //CCUserDetails = "",
                            CheckoutDiscountCode =
                                orderRow.Discount == null ? string.Empty : orderRow.Discount.DiscountCode,
                            DisplayOptionsInDropDownViewModel = new DisplayOptionsInDropDownViewModel
                            {
                                Options = _orderManagementService.GetOptionsByWebinarId(orderRow.idWebinar, true),
                                OrderRowId = orderRow.idOrderRow,
                                OrderRowRegistrationType = orderRow.RegistrationType
                            },
                            DisplayRowPriceViewModel =
                                model.CheckoutOptionsViewModel.DisplayOptionsViewModel.DisplayRowPriceViewModel,
                            idUser = model.Order.idUser,
                            DiscountDetailsModel = new DiscountDetailsModel()
                            {
                                UserId = webUser.idUser,
                                Discount = orderRow.Discount == null
                                    ? new DiscountModel()
                                    : new DiscountModel()
                                    {
                                        //Cost = discountModel.Cost,
                                        //DateBilled = discountModel.DateBilled,
                                        //DateValidFrom = discountModel.DateValidFrom,
                                        ////idUser = idUser,
                                        //DateValidTo = discountModel.DateValidTo,
                                        //DiscountCode = discountModel.DiscountCode,
                                        //DiscountType = discountModel.TypeOfDiscount,
                                        //FlatOff = discountModel.FlatOff,
                                        //Notes = discountModel.Notes,
                                        //PercentOff = discountModel.PercentOff,
                                        //RenewalTerm = discountModel.RenewalTerm,
                                        //Status = discountModel.Status,
                                        //UsesCount = discountModel.UsesCount,
                                        //UsesRemain = discountModel.UsesRemain,
                                        ////WebUserDiscountXref = 
                                        ////idDiscount = 
                                    },
                            },
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
                                    },
                            },
                            OptionLabel = orderRow.RegistrationType.OptionLabel,
                            OrderExists = true,
                            OrderHasAdditionalLocationsViewModel = new OrderHasAdditionalLocationsViewModel
                            {
                                AdditionalLocations =
                                    model.Order.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active)
                                        .AdditionalLocation,
                                Addresses = additionalLocationsViewModel.Addresses,
                                OptionsCost = additionalLocationsViewModel.OptionsCost,
                            },
                            OrderRowExists = true,
                            OrderRowHasId = true,
                            OrderStatus = OrderStatus.InProcess,
                            Origin = model.Order.Origin,
                            UserComments = model.Order.UserComments,
                            UserDetails = string.Concat("<span id='userFullnameLabel'>", userFullName,
                                "</span> - <span id='userInstitutionLabel'>", orderRow.Order.Institution, "</span><br>",
                                "<span id='userEmailLabel'>", webUser.email, "</span>"),
                            UserFullname = userFullName,
                            UserType = UserType.Customer
                        };
                    }
                }
                return View(model);
            }

            _logger.Error("Details Action invoked with null 'id' parameter");

            return RedirectToAction("allActive", new { eventsToShow = "upcoming" });
        }

        public IEnumerable<Order> CheckDisplayPostEventMaterials(UserAccount userAccount, Order checkOrder)
        {
            if (userAccount != null && userAccount.HasClaim(Business.Constants.ClaimTypes.DisplayPostEventMaterials))
            {
                var claimsForOrder =
                    userAccount.Claims.FirstOrDefault(c => c.Value.ToLower().Contains(checkOrder.idOrder.ToString()));

                // extract the date
                if (claimsForOrder != null)
                {
                    var expiryAsString = claimsForOrder.Value.Substring(claimsForOrder.Value.IndexOf(":") + 1);

                    DateTime expiryDate;

                    if (DateTime.TryParse(expiryAsString, out expiryDate))
                    {
                        if (DateTime.Today <= expiryDate)
                        {
                            yield return checkOrder;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// This method initializes state for variables which needed to be retrieved from the Database
        /// </summary>
        /// <param name="webinar"></param>
        /// <param name="model"></param>
        /// <param name="id"></param>
        private void InitializeDetailsState(Webinar webinar, WebinarDetailsViewModel model, int id)
        {
            IEnumerable<AdditionalLocation> additionalLocations = null;
            OrderRow orderRowForOrder = null;

            model.WebUser = Request.IsAuthenticated
                ? _membershipService.GetUserByEmailLoadedWithOrdersData(User.Identity.Name)
                : new WebUser();

            if (Request.IsAuthenticated && model.WebUser.Orders.FirstOrDefault() != null)
            {
                model.Order =
                    model.WebUser.Orders.FirstOrDefault(
                        o => o.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active).idWebinar == id);
            }

            var orderExists = model.Order != null;

            if (orderExists)
            {
                orderRowForOrder = model.Order.OrderRows.SingleOrDefault(or => or.RowStatus == OrderRowStatus.Active);
                Debug.Assert(orderRowForOrder != null, "orderRowForOrder object should always have a value here.");

                additionalLocations = orderRowForOrder.AdditionalLocation.ToList();
            }

            model.Topics = _webinarManagementService.GetTopicsPerWebinar(webinar.idWebinar);

            model.WebinarFiles = _webinarManagementService.GetWebinarFilesPerWebinar(webinar.idWebinar);
            var AddLocPrice = _orderManagementService.GetPriceOfAdditionalLocation(webinar.idWebinar);
            model.CheckoutOptionsViewModel = new CheckoutOptionsViewModel
            {
                DisplayOptionsViewModel = new DisplayOptionsViewModel
                {
                    AdditionalLocationOfferViewModel = new AdditionalLocationOfferViewModel
                    {
                        AdditionalLocations = additionalLocations,
                        //AdditionalLocationAddViewModel property is assigned further below as more data comes to hand. 
                        OrderExists = orderExists,
                        Webinar = webinar,
                        Price = AddLocPrice
                    },
                    // DisplayRowPriceViewModel property is assigned further below as more data comes to hand. 
                    EventTitle = model.Webinar.Title,
                    idWebinar = model.Webinar.idWebinar,
                    Options = _orderManagementService.GetOptionsByWebinarId(id, false),
                    OrderRowExists = orderRowForOrder != null,
                    WebinarDuration = model.Webinar.Duration,
                    WebinarStatus = model.Webinar.Status
                },
                AdditionalLocations = additionalLocations,
                ConnectionInfoPresent = model.Webinar.ConnectionInfo != null,
                WebinarDuration = model.Webinar.Duration,
                idWebinar = model.Webinar.idWebinar,
                idUser = model.WebUser.idUser,
                OrderExists = orderExists,
                SelectedWebUser = -1,
                WebinarStatus = model.Webinar.Status
            };

            if (orderExists)
            {
                model.CheckoutOptionsViewModel.OrderStatus = model.Order.OrderStatus;
                model.CheckoutOptionsViewModel.OrderHasId = model.Order.idOrder > 0;
                var row = orderRowForOrder;

                if (row != null)
                    model.CheckoutOptionsViewModel.RegistrationType = orderRowForOrder.RegistrationType;
                else
                    row = model.Order.OrderRows.SingleOrDefault();

                var additionalLocationsPricing =
                    _orderManagementService.GetCostOfAdditionalLocations(orderRowForOrder.AdditionalLocation,
                        webinar.idWebinar
                        );

                // populate DisplayRowPriceViewModel of DisplayOptionsViewModel
                model.CheckoutOptionsViewModel.DisplayOptionsViewModel.DisplayRowPriceViewModel =
                    new DisplayRowPriceViewModel
                    {
                        Discount = row.Discount,
                        NumberOfAdditionalLocations = row.AdditionalLocation.Count(),
                        OrderStatus = row.Order.OrderStatus,
                        Price = row.RegistrationType.Price,
                        PricesAndDiscounts =
                            _orderManagementService.CalculateOrderCost(row.Order, additionalLocationsPricing.Item2),
                        RowPrice = row.RowPrice,
                        RegistrationType = row.RegistrationType
                    };

                // populate AdditionalLocationOfferViewModel and AdditionalLocationAddViewModel
                model.CheckoutOptionsViewModel.DisplayOptionsViewModel.AdditionalLocationOfferViewModel.Emails =
                    model.CheckoutOptionsViewModel.DisplayOptionsViewModel.AdditionalLocationOfferViewModel
                        .AdditionalLocations.Select(al => al.Email).ToArray();
                model.CheckoutOptionsViewModel.DisplayOptionsViewModel.AdditionalLocationOfferViewModel
                    .AdditionalLocationAddViewModel = new AdditionalLocationAddViewModel
                    {
                        AdditionalLocations = additionalLocations,
                        Price = additionalLocationsPricing.Item2
                    };
                model.CheckoutOptionsViewModel.DisplayOptionsViewModel.AdditionalLocationOfferViewModel.Price =
                    additionalLocationsPricing.Item2;

                // populate RegistrationSummaryViewModel and OrderHasAdditionalLocationsViewModel
                model.RegistrationSummaryViewModel = new RegistrationSummaryViewModel
                {
                    OrderHasAdditionalLocationsViewModel = new OrderHasAdditionalLocationsViewModel
                    {
                        AdditionalLocations = row.AdditionalLocation,
                        Addresses = additionalLocationsPricing.Item1,
                        OptionsCost = additionalLocationsPricing.Item2
                    },
                    OrderRow = orderRowForOrder,
                    RecordingLink =
                        "<a href='" + GlobalConfig.GlobalConfigSingleton.WMVRepository + webinar.RecordingUrl +
                        "' target=_blank /> Recording Playback</a>",
                    WebinarStatus = webinar.Status
                };
            }
        }

        /// <summary>
        /// This method initializes state which is more relevant to display-text and variables used in the razor view
        /// (as distinct from variables which are meant to be captured in form submissions).
        /// </summary>
        /// <param name="model"></param>
        private void InitializeViewCentricProperties(WebinarDetailsViewModel model)
        {
            ViewBag.PageStyleType = "holy-grail-three-columns";
            model.UserHasOpenOrder = 0;
            model.UserOwnsThisEvent = 0;
            model.CheckoutInProcess = false;

            ViewBag.metaDesc = string.Empty;
            ViewBag.metaKeywords = string.Empty;

            var userExists = model.WebUser.idUser > 0;

            model.TimeZone = userExists ? model.WebUser.timeZone : USTimeZone.Central;

            model.UserIsLoggedIn = userExists;

            model.SignUpCaption = "Sign Up!";
            model.ConfirmationCaption = "Confirmation";
            model.Identity = ((ClaimsIdentity)User.Identity);
            model.UserAddressVerified = model.Identity.HasClaim(ClaimTypes.AddressVerified);
            model.TimeFormatDisplay = "<i>" + DateTimeHelper.FormatTime(model.Webinar.Date, model.TimeZone, false) +
                                      " - " +
                                      DateTimeHelper.FormatTime(
                                          model.Webinar.Date.AddHours((double)model.Webinar.Duration), model.TimeZone,
                                          true) + "<br /></i>";

            model.CeuShort = string.Empty;
            model.CeuStatement = string.Empty;

            if (!string.IsNullOrEmpty(model.Webinar.ceu))
            {
                string[] ceu = model.Webinar.ceu.Split('|');
                model.CeuShort = ceu[0];
                model.CeuStatement = ceu[1];
            }
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

        [HandleAjaxException]
        public ActionResult Clone(int? id)
        {
            if (id.HasValue)
            {
                try
                {
                    var webinarEditModel = _webinarControllerOrchestrator.BuildEditModelForWebinar(id.Value);

                    webinarEditModel.DateChanged = webinarEditModel.DateCreated = DateTime.Now;

                    return PartialView("Partials/_CloneWebinar", webinarEditModel);
                }
                catch (Exception exception)
                {
                    _logger.ErrorException(string.Format("Clone Webinar | Session{0}", _appHelper.GetUserAuditInfo()), exception);
                }
                return Json(new { Result = WebUiConstants.Fail, Msg = ServerErrorLoggedMsg }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { Result = WebUiConstants.Fail, Msg = EnterValidIdMsg }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken(Order = 0)]
        [ValidateInput(false)]
        [HandleAjaxException(Order = 1)]
        public ActionResult Clone(WebinarEditModel webinarEditModel)
        {
            try
            {
                _webinarControllerOrchestrator.CreateWebinarFromViewInput(webinarEditModel);
                return Json(new { Result = WebUiConstants.Success });
            }
            catch (ValidationException validationException)
            {
                validationException.Errors.ForEach(error => ModelState.AddModelError(FromFluentPrefix + error.PropertyName, error.ErrorMessage));

                return this.ModelStateJsonFromFluentValidator(ModelState);
            }
            catch (Exception exception)
            {
                _logger.ErrorException(string.Format("Create Webinar | Session{0}", _appHelper.GetUserAuditInfo()), exception);
            }

            return Json(new { Result = WebUiConstants.Fail });
        }

        [HandleAjaxException]
        public ActionResult Create()
        {
            try
            {
                var webinarEditModel = _webinarControllerOrchestrator.BuildEditModelForWebinarCreate();

                return PartialView("Partials/_CreateWebinar", webinarEditModel);
            }
            catch (Exception exception)
            {
                _logger.ErrorException(string.Format("Create Webinar | Session{0}", _appHelper.GetUserAuditInfo()), exception);
                return Json(new { Result = WebUiConstants.Fail, Msg = ServerErrorLoggedMsg }, JsonRequestBehavior.AllowGet);
            }
        }


        [System.Web.Mvc.HttpPost]
        [ValidateAntiForgeryToken(Order = 0)]
        [ValidateInput(false)]
        [HandleAjaxException(Order = 1)]
        public ActionResult Create(WebinarEditModel webinarEditModel)
        {
            try
            {
                _webinarControllerOrchestrator.CreateWebinarFromViewInput(webinarEditModel);

                return Json(new { Result = WebUiConstants.Success });
            }
            catch (ValidationException validationException)
            {
                validationException.Errors.ForEach(error => ModelState.AddModelError(FromFluentPrefix + error.PropertyName, error.ErrorMessage));

                return this.ModelStateJsonFromFluentValidator(ModelState);
            }
            catch (Exception exception)
            {
                _logger.ErrorException(string.Format("Create Webinar | Session{0}", _appHelper.GetUserAuditInfo()), exception);
                return Json(new { Result = WebUiConstants.Fail });
            }
        }


        [HandleAjaxException]
        public ActionResult Edit(int? id)
        {
            if (id.HasValue)
            {
                try
                {
                    var webinarEditModel = _webinarControllerOrchestrator.BuildEditModelForWebinar(id.Value);

                    return PartialView("Partials/_EditWebinar", webinarEditModel);
                }
                catch (Exception exception)
                {
                    _logger.ErrorException(string.Format("Edit Webinar | Session{0}", _appHelper.GetUserAuditInfo()), exception);
                }

                return Json(new { Result = WebUiConstants.Fail, Msg = ServerErrorLoggedMsg }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { Result = WebUiConstants.Fail, Msg = EnterValidIdMsg }, JsonRequestBehavior.AllowGet);
        }


        [System.Web.Mvc.HttpPost]
        [ValidateAntiForgeryToken(Order = 0)]
        [ValidateInput(false)]
        [HandleAjaxException(Order = 1)]
        public ActionResult Edit(WebinarEditModel webinarEditModel)
        {
            try
            {
                _webinarControllerOrchestrator.UpdateWebinarFromViewInput(webinarEditModel);

                return Json(new { Result = WebUiConstants.Success });
            }
            catch (ValidationException validationException)
            {
                validationException.Errors.ForEach(error => ModelState.AddModelError(FromFluentPrefix + error.PropertyName, error.ErrorMessage));

                return this.ModelStateJsonFromFluentValidator(ModelState);
            }
            catch (DbEntityValidationException dbEntityValidationException)
            {
                var stringBuilder = new StringBuilder();

                foreach (var validationErrors in dbEntityValidationException.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName,
                            validationError.ErrorMessage);
                        stringBuilder.AppendFormat("Property: {0} Error: {1} ", validationError.PropertyName,
                            validationError.ErrorMessage);
                    }
                }
                _logger.Error("UpdateShippingDetails dbEntityValidationException errors | {0}",
                    stringBuilder.ToString());
                return null;
            }
            catch (Exception exception)
            {
                _logger.ErrorException(string.Format("Edit Webinar | Session{0}", _appHelper.GetUserAuditInfo()), exception);
                return Json(new { Result = WebUiConstants.Fail });
            }
        }

        public ActionResult EditWebinarFromDetails(int? id)
        {
            if (id.HasValue)
            {
                var webinarEditModel = _webinarControllerOrchestrator.BuildEditModelForWebinar(id.Value);

                return View(webinarEditModel);
            }
            return View();
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
        [ValidateJsonAntiForgeryToken]
        public ActionResult UpdateConnectionInfo(ConnectionInfoEditModel connectionInfoModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    bool detailsValid;
                    var webinar = _webinarControllerOrchestrator.PopulateWebinarFromViewModel(connectionInfoModel, out detailsValid);

                    if (!detailsValid)
                    {
                        return Json(new { Result = WebUiConstants.Fail });
                    }

                    return Json(new { Result = WebUiConstants.Success });

                }
                catch (Exception exception)
                {
                    _logger.ErrorException(
                        string.Format("UpdateConnectionInfo | Session {0}", exception.Message + " : " + _appHelper.GetUserAuditInfo()), exception
                        );
                    ModelState.AddModelError(string.Empty, WebUiConstants.ServerErrorWithAssistNumber);

                    return Json(new { Result = WebUiConstants.Fail });
                }
            }
            return this.ModelStateJson(ModelState);
        }

        public ActionResult CreateICSForWebinar(int id)
        {
            try
            {
                var webinar = _webinarManagementService.GetWebinarByIdIncludingAllWebinarsByPresenter(id);

                string filePath = HostingEnvironment.MapPath("~/App_Data/ics/");

                string defaultDescription =
                    "1. Click this link to start or to join the Webinar:<br><br>https://www2.gotomeeting.com/ojoin/325710074/922930 <br><br><br>2. Choose one of the following audio options:<br><br> TO USE YOUR COMPUTER’S AUDIO:<br> When the Webinar begins, you will be connected to audio using your computer’s microphone and speakers (VoIP). A headset is recommended.<br><br><br> TO USE YOUR TELEPHONE:<br> If you prefer to use your phone, you must select “Use Telephone” after joining the Webinar and call in using the numbers below.<br><br><br> Toll-free: 1 877 568 4108<br> Access Code: 529-918-465<br> Audio PIN: Shown after joining the meeting<br><br><br>GoToWebinar®<br>Webinars Made Easy™<br>";
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

                str.AppendLine(string.Format("DTEND:{0:yyyyMMddTHHmmssZ}",
                    webinar.Date.AddHours(Convert.ToDouble(webinar.Duration))));

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
                _webinarControllerOrchestrator.Dispose();
                _webinarManagementService.Dispose();

                base.Dispose(true);
            }
            _disposed = true;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateWebinarFiles(WebinarFilesEditModel webinarFilesEditModel)
        {
            /*
                This is a batch update operation. The WebinarFiles collection contains the webinar files to be updated. 
                The three possibilities are updated, deleted and created. (Even if a file was unchanged at the client, it will be treated as updated.)
                    * If the idWebinar in the WebinarFiles is a positive number and does not end with -D, it is updated
                    * If the idWebinar in the WebinarFiles is 0, and does not end with -ND, it has been created (suffix of 'N' appended at client deserializes to 0)
                    * If the fileDesc in the WebinarFile ends with -D, it has been marked for deletion.             
             */
            var filesForThisEvent = _webinarManagementService.GetWebinarFilesPerWebinar(webinarFilesEditModel.idWebinar);
            try
            {
                var deletedFiles = webinarFilesEditModel.WebinarFiles.Where(f => f.fileDesc.EndsWith("-D")).ToList();
                var newFiles =
                    webinarFilesEditModel.WebinarFiles.Where(f => f.idWebinarFile == 0
                        && !f.fileDesc.EndsWith("-ND")
                        && f.fileLocation != filesForThisEvent.Select(wf => wf.fileLocation).ToString())
                        .ToList();
                var updatedFiles =
                    webinarFilesEditModel.WebinarFiles.Where(f => f.idWebinarFile > 0 && !f.fileDesc.EndsWith("-D"))
                        .ToList();

                foreach (var webinarFile in newFiles)
                {
                    webinarFile.fileLocation = Regex.Replace(webinarFile.fileLocation, @"\s+", "");
                    var checkThatNewFilesExist = CheckThatFilesExists(webinarFile);
                    if (!checkThatNewFilesExist)
                    {
                        return Json(new { Result = webinarFile + "does not exist." });
                    }
                }

                _webinarManagementService.AddWebinarFiles(newFiles);
                _webinarManagementService.DeleteWebinarFiles(deletedFiles);
                _webinarManagementService.UpdateWebinarFiles(updatedFiles);

                return Json(new { Result = WebUiConstants.Success });
            }
            catch (Exception exception)
            {
                _logger.ErrorException(
                    string.Format("UpdateWebinarFiles| UpdateWebinarFiles failed {0}", exception.Message), exception);
                ModelState.AddModelError(string.Empty,
                    "There was a problem with the update operation. Please consult with the system administrator to resolve the issue.");
            }

            return this.ModelStateJson(ModelState);
        }

        public ActionResult GetWebinarRecording(int? id)
        {
            if (id.HasValue)
            {
                var webinar = _webinarManagementService.GetWebinar(id.Value);

                return Redirect(_globalConfig.WMVRepository + webinar.RecordingUrl);
            }
            return View();
        }


        public ActionResult GetWebinarFile(int? idWebinarFile)
        {
            if (idWebinarFile.HasValue)
            {
                var webinarFile = _webinarManagementService.GetWebinarFile(idWebinarFile.Value);

                return Redirect(_globalConfig.WMVRepository + webinarFile.fileLocation);
            }
            return View();
        }

        private class MyClient : WebClient
        {
            public bool HeadOnly { get; set; }

            protected override WebRequest GetWebRequest(Uri address)
            {
                WebRequest req = base.GetWebRequest(address);
                if (HeadOnly && req.Method == "GET")
                {
                    req.Method = "HEAD";
                }
                return req;
            }
        }

        private bool CheckThatFilesExists(WebinarFile newFile)
        {
            var handoutRepo = "http://ttsmedia.ttstrain.com/";
            //http://stackoverflow.com/questions/153451/how-to-check-if-system-net-webclient-downloaddata-is-downloading-a-binary-file#156750

            using (MyClient client = new MyClient())
            {
                client.HeadOnly = true;
                string uri = handoutRepo + newFile.fileLocation;
                byte[] body = client.DownloadData(uri); // note should be 0-length
                string type = client.ResponseHeaders["content-type"];
                client.HeadOnly = false;
                //
                //there's probably a better way to test that the file exists
                if (type.Contains(@"/"))
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
        }

        private bool CheckThatRecordingExists(string checkFile)
        {
            var handoutRepo = "http://ttsmedia.ttstrain.com/";
            //http://stackoverflow.com/questions/153451/how-to-check-if-system-net-webclient-downloaddata-is-downloading-a-binary-file#156750

            using (MyClient client = new MyClient())
            {
                client.HeadOnly = true;
                string uri = handoutRepo + checkFile;
                byte[] body = client.DownloadData(uri); // note should be 0-length
                string type = client.ResponseHeaders["content-type"];
                client.HeadOnly = false;
                //
                //there's probably a better way to test that the file exists
                if (type.Contains(@"/"))
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
        }

        public ActionResult ClickToJoin(string joinCode)
        {
            var webinar = _orderManagementService.GetWebinarByJoinCode(joinCode);

            var clickToJoinViewModel = new ClickToJoinViewModel
            {
                JoinCode = joinCode,
                RedirectLinkText = @"http://" + _globalConfig.Tenant + ".com/" + joinCode,
                Webinar = webinar
            };

            if (_stateService.HasValue(WebinarFromCode))
                _stateService.ClearValue(WebinarFromCode);
            _stateService.SetValue(WebinarFromCode, webinar);

            return View(clickToJoinViewModel);
        }

        public ActionResult OpenMeeting(string joinCode)
        {
            string webinarUrl = string.Empty;
            Webinar webinar = null;

            if (_stateService.HasValue(WebinarFromCode))
            {
                webinar = _stateService.GetValue<Webinar>(WebinarFromCode);
                _stateService.ClearValue(WebinarFromCode);

                var orderRow = webinar.OrderRows.SingleOrDefault(or => or.TtsJoinUrl == joinCode);


                if (!ReferenceEquals(null, orderRow))
                {

                    if (User.Identity.IsAuthenticated && !ReferenceEquals(null, orderRow.JoinURL)) // JoinURL will be null for impromtu user
                    {

                        webinarUrl = orderRow.JoinURL; // fully qualified authorative webinar-access link from Citrix.
                    }
                    else
                    {
                        // non-individualized version of webinar-access link from Citrix.
                        webinarUrl = webinar.CitrixRegisterUrl;
                    }
                }

            }
            return new RedirectResult(webinarUrl);
        }

        public ActionResult UpdateWebinarRecording(WebinarDetailsViewModel webinarDetailsViewModel)
        {
            Webinar webinar = _webinarManagementService.GetWebinar(webinarDetailsViewModel.Webinar.idWebinar);

            try
            {
                var checkThatNewFilesExist = CheckThatRecordingExists(webinarDetailsViewModel.Webinar.RecordingUrl);

                if (!checkThatNewFilesExist)
                {
                    return Json(new { result = webinarDetailsViewModel.Webinar.RecordingUrl + " does not exist." });
                }

                var ordersForWebinar = _orderManagementService.GetOrdersForWebinar(webinar.idWebinar);

                AddClaimForPostEventMaterials(ordersForWebinar);

                webinar.RecordingUrl = webinarDetailsViewModel.Webinar.RecordingUrl;

                webinar.Status = WebinarStatus.Recorded;

                //var sendRecording = SendRecordingPosted();

                _webinarManagementService.UpdateWebinar(webinar);

                _orderManagementService.FireSendRecordingIsPostedEvent(ordersForWebinar);

                return Json(new { result = WebUiConstants.Success });
            }
            catch (Exception exception)
            {
                _logger.ErrorException(
                    string.Format("UpdateWebinarRecording| UpdateWebinarRecording failed {0} on idWebinar: {1}", exception.Message, webinar.idWebinar), exception);
                ModelState.AddModelError(string.Empty,
                    "There was a problem with the update operation. Please consult with the system administrator to resolve the issue.");
            }

            return this.ModelStateJson(ModelState);
        }

        private void AddClaimForPostEventMaterials(IEnumerable<Order> orders)
        {
            foreach (var order in orders)
            {
                var userAccountOfOrderer = _membershipService.GetUserAccountByEmail(
                    _globalConfig.Tenant,
                    order.WebUser.email
                    );

                var expiryDate = GetPostEventMaterialsAccessExpiry(order);

                try
                {
                    var onDemandCode = RandomHelpers.GetUniqueCode(8);

                    var orderIdProperty = new JProperty(JsonPropertyKeys.OrderId, order.idOrder);
                    var expiryDateProperty = new JProperty(JsonPropertyKeys.ExpiryDate, expiryDate.ToString("yyyy-MM-dd"));
                    var obfuscationStringProperty = new JProperty(JsonPropertyKeys.ObfuscationString, onDemandCode);

                    var claimValue = new JObject(
                        orderIdProperty,
                        expiryDateProperty,
                        obfuscationStringProperty
                        );

                    _membershipService.AddClaim(
                        userAccountOfOrderer, ClaimTypes.DisplayPostEventMaterials, claimValue.ToString(Formatting.None)
                        );

                    _logger.Info("Claim added for " + order.idOrder);

                }
                catch (Exception)
                {
                    _logger.Error(string.Format("AddClaimForPostEventMaterials| MR record not found {0}", order.BillingEmail));
                }
            }
        }

        private DateTime GetPostEventMaterialsAccessExpiry(Order order)
        {
            if (order == null) throw new ArgumentNullException("order");

            var orderRow = order.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active);
            // let exception be thrown if there is not a single 

            var regType = _orderManagementService.GetRegTypeOfOrderRow(orderRow.idRegType);

            if (regType.ShowRecordingNotifications.Equals("yes", StringComparison.OrdinalIgnoreCase))
                return DateTime.Today.AddMonths(6);

            return DateTime.Today.AddDays(5);
        }
    }
}
