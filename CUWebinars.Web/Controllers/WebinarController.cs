using BrockAllen.MembershipReboot;
using CUWebinars.Business.AccountService;
using CUWebinars.Business.Constants;
using CUWebinars.Business.Core;
using CUWebinars.Business.Core.Helpers;
using CUWebinars.Business.Models;
using CUWebinars.Business.Services;
using CUWebinars.Web.Core;
using CUWebinars.Web.Core.Orchestrators;
using CUWebinars.Web.Helpers;
using CUWebinars.Web.Infrastructure.Attributes;
using CUWebinars.Web.Infrastructure.Extensions;
using CUWebinars.Web.Mapping.Mappers;
using CUWebinars.Web.Models;
using CUWebinars.Web.Models.JsonModels;
using CUWebinars.Web.Services;
using CUWebinars.Web.ViewModel;
using FluentValidation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Security.Claims;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using Citrix.GoToWebinar.Api;
using Citrix.GoToWebinar.Api.Model;
using MailChimp.Net;
using MailChimp.Net.Core;
using MailChimp.Net.Interfaces;
using MailChimp.Net.Models;
using Thinktecture.IdentityModel.Authorization;
using WebGrease.Css.Extensions;
using ClaimTypes = CUWebinars.Business.Constants.ClaimTypes;
using DateTimeHelper = CUWebinars.Web.Helpers.DateTimeHelper;
using Order = CUWebinars.Business.Models.Order;
using PostEventClaim = CUWebinars.Business.Services.PostEventClaim;
using Webinar = CUWebinars.Business.Models.Webinar;


namespace CUWebinars.Web.Controllers
{
    public class WebinarController : Controller
    {
        private const string FromFluentPrefix = "fromfluent-";
        private const string EnterValidIdMsg = "Enter a valid id";
        private const string ServerErrorLoggedMsg = "Server Error Logged";
        private IStateService _stateService;
        private readonly IWebinarControllerOrchestrator _webinarControllerOrchestrator;
        private readonly IAppHelper _appHelper;
        private readonly IUniversalMapper _universalMapper;
        private readonly IDataTablesService _dataTablesService;
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
            IUniversalMapper universalMapper,
            IDataTablesService dataTablesService)
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
            _dataTablesService = dataTablesService;
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
                case "Caribbean":
                    tz = USTimeZone.Caribbean;
                    break;
                case "Alaska":
                    tz = USTimeZone.Alaska;
                    break;
                case "Hawaii":
                    tz = USTimeZone.Hawaii;
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


        public ActionResult Recorded(int? idAff)
        {
            return RedirectToAction("AllActive", "Webinar", new { eventsToShow = "recorded", idAff = idAff });
        }


        public ActionResult Upcoming(int? idAff)
        {
            return RedirectToAction("AllActive", "Webinar", new { eventsToShow = "upcoming", idAff = idAff });
        }

        public ActionResult OnDemandPlayback(int? idWebinar, int? idUser)
        {

            return
                Redirect("http://legacy.bankwebinars.com/Webinar/OnDemandPlayback?idWebinar=" + idWebinar + "&idUser=" +
                         idUser);
            //_webinarControllerOrchestrator.OnDemandLegacy(idWebinar.Value, idUser.Value);
            //return View(webinar);
        }

        public ActionResult ConnectionDetails(int id)
        {
            return RedirectToAction("ClickToJoin", new { joinCode = "0000", idWebinar = id });
        }


        public ActionResult RedirectLegacy(int? w, int? u)
        {

            return _webinarControllerOrchestrator.OnDemandLegacy(w.Value, u.Value);
            //return View(webinar);
        }

        public ActionResult RedirectLegacyRecordings(string recordingURL)
        {

            return Redirect("http://legacy.bankwebinars.com/recordings/" + recordingURL);

        }

        public ActionResult RedirectLegacyHandouts(string handout)
        {

            return Redirect("http://legacy.bankwebinars.com/handouts/" + handout);

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
            Session["TopicID"] = ID;
            ViewBag.SearchTerm = "TopicID=" + Session["TopicID"].ToString();

            var webinars = _webinarManagementService.GetByTopic(ID).OrderByDescending(d => d.Date);

            return View(webinars);
        }

        //

        public ActionResult Search()
        {
            ViewBag.PageStyleType = "two-columns-right-sidebar";
            ViewBag.TopicCaption = " ";


            string searchTerm = Request["searchTerm"].Trim(' ');
            ViewBag.SearchTerm = searchTerm;
            _logger.Info("Searching on: " + searchTerm + " by: " + _appHelper.GetUserAuditInfo());

            try
            {
                ShowWebinarsViewModel model = new ShowWebinarsViewModel
                {
                    SearchTerm = searchTerm
                };
                ViewBag.Title = "Search Results";

                if (User != null)
                {
                    ClaimsIdentity claimsIdentityOfAuthenticatedUser = (ClaimsIdentity)User.Identity;
                    if (claimsIdentityOfAuthenticatedUser.HasClaim(
                        (claim) => claim.Type == Business.Constants.ClaimTypes.Admin
                                   || claim.Type == Business.Constants.ClaimTypes.Affiliate))
                    {

                        var currentAffiliate = _orderManagementService.GetAffiliateById(19);

                        if (claimsIdentityOfAuthenticatedUser.HasClaim(
                            (claim) => claim.Type == Business.Constants.ClaimTypes.Affiliate))
                        {
                            currentAffiliate =
                                _orderManagementService.GetAffiliateByDomain(claimsIdentityOfAuthenticatedUser.Claims
                                    .Where(c => c.Type == ClaimTypes.Affiliate).Select(c => c.Value).Single());
                            model.UserIsAdmin = false;
                            model.Affiliate = currentAffiliate;
                        }

                        var oModel = new ShowOrdersViewModel
                        {
                            Affiliate = currentAffiliate,
                            SearchTerm = searchTerm,
                            UserIsAdmin = false,
                            Webinar = null
                        };
                        if (claimsIdentityOfAuthenticatedUser.HasClaim(
                            (claim) => claim.Type == Business.Constants.ClaimTypes.Admin))
                        {
                            model.UserIsAdmin = true;
                            oModel.UserIsAdmin = true;
                        }

                        //searchByidOrder
                        if (searchTerm != "" && searchTerm.All(Char.IsDigit))
                        {

                            var order =
                                _orderManagementService.GetOrderById(Convert.ToInt32(searchTerm));
                            if (order != null)
                            {
                                oModel.Orders = new List<Order> { order };

                                return View("~/Views/Admin/SearchAdmin.cshtml", oModel);
                            }

                        }

                        //searchDomainOnly (select * where email like '%@ttstrain.com')
                        if (searchTerm.StartsWith("@") || searchTerm.StartsWith("aa") || searchTerm.StartsWith("l ") ||
                            searchTerm.StartsWith("inprocess") || searchTerm.Contains("@") ||
                            searchTerm.StartsWith("inprocess"))
                        {

                            if (currentAffiliate.idUserAff != 19)
                            {
                                oModel.UserIsAdmin = false;
                                return View("~/Views/Admin/SearchAdmin.cshtml", oModel);
                            }
                            else
                            {
                                oModel.UserIsAdmin = true;
                                return View("~/Views/Admin/SearchAdmin.cshtml", oModel);
                            }

                        }

                    }
                }

                if (searchTerm != "" && searchTerm.All(Char.IsDigit))
                {
                    if (searchTerm.Length < 5)
                    {
                        Session["TopicID"] = searchTerm;
                        ViewBag.SearchTerm = "TopicID=" + Session["TopicID"].ToString();

                        var webinars =
                            _webinarManagementService.GetByTopic(Convert.ToInt32(searchTerm))
                                .OrderByDescending(d => d.Date);
                    }
                }
                return View("search2", model);
            }
            catch (Exception exception)
            {
                _logger.ErrorException(string.Format("Search | Session {0}", _appHelper.GetUserAuditInfo()),
                    exception);
                ModelState.AddModelError(string.Empty, WebUiConstants.ServerErrorWithAssistNumber);
                throw;
            }
        }

        public ActionResult AllActive(string eventsToShow, int? idAff)
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
                webinars = _webinarManagementService.GetRecordedWebinars().OrderByDescending(w => w.Date);
                ViewBag.Title = "All Recorded Events for " + _globalConfig.Tenant;
            }

            if (eventsToShow == "des")
            {
                webinars = _webinarManagementService.GetDesWebinars().OrderByDescending(w => w.Date);
                ViewBag.Title = "All Director Education Series Courses";
                return View("~/Views/Webinar/AllActiveDes.cshtml", webinars);
            }

            return View(webinars);
        }


        public PartialViewResult SendConnectionInfo()
        {
            var model = _webinarControllerOrchestrator.BuildAdhocNotificationViewModel(WebinarType.Upcoming);

            return PartialView(@"Partials/_SendConnectionInfo", model);
        }

        [System.Web.Mvc.HttpPost]
        public JsonResult SendConnectionInfo(int webinarId)
        {
            _logger.Info("ConnectionInfo Send is started: " + webinarId);
            var webinar = _webinarManagementService.GetWebinar(webinarId);


            try
            {
                PanelistsApi pApi = new PanelistsApi();

                pApi.createPanelists(_globalConfig.CitrixAuthMark,
                    _globalConfig.ConvertToCitrixOrgKey(_globalConfig.CitrixOrgKeyMark),
                    _globalConfig.ConvertToCitrixWebinarKey(webinar.WebinarKey), new List<PanelistReqCreate>
                    { new PanelistReqCreate {
                    email = webinar.Presenter.WebUser.email,
                    name = webinar.Presenter.WebUser.FullName}
                    });
            }
            catch (Exception ex)
            {
                _logger.FatalException("CreateCitrixWebinar | AddPresenter", ex);
            }

            _webinarControllerOrchestrator.FireSendConnectionInfoNotificationEvent(webinarId);

            _logger.Info("ConnectionInfo Send is ended: " + webinarId);
            return Json(new { Result = WebUiConstants.Success });

        }



        //public ActionResult SearchByTopic(
        //    [Core.DataTables.WebinarsBrowserRequestModelBinder] WebinarsBrowserRequestModel webinarsBrowserRequest)
        //{

        //    var webinars =
        //        _webinarManagementService.GetByTopic(
        //            Convert.ToInt32(webinarsBrowserRequest.Search.Replace("TopicID=", "")));
        //    var wList = new List<WebinarsBrowserSearchResultEntryDTO>();
        //    foreach (var webinar in webinars)
        //    {
        //        var item = new WebinarsBrowserSearchResultEntryDTO();
        //        item.Webinar = webinar;
        //        item.RegistrationsCount = 0;
        //        wList.Add(item);
        //    }

        //    WebinarsBrowserSearchResultDTO searchResult = new WebinarsBrowserSearchResultDTO();
        //    searchResult.Entries = wList;
        //    searchResult.FoundCount = 2;
        //    searchResult.TotalCount = webinars.Count();

        //    var data = new WebinarsSearchDTOAssembler(webinarsBrowserRequest.EchoId).Entity2DTO(searchResult);
        //    return Json(data, JsonRequestBehavior.AllowGet);
        //}

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult OnDemand(string onDemandCode)
        {
            if (ReferenceEquals(onDemandCode, null)) return null;
            int _id;
            var isNum = Int32.TryParse(onDemandCode.Split('-')[0], out _id);
            string hasVal = null;
            if (onDemandCode.Split('-').Length > 1)
            {
                hasVal = onDemandCode.Split('-')[1];
            }
            var order = _orderManagementService.GetOrderById(_id);

            if (order != null && hasVal != null)
            {
                return _webinarControllerOrchestrator.OnDemand(_id, hasVal, User.Identity);
            }

            string codeByVal = _orderManagementService.GetOnDemandClaimByCode(hasVal);

            var thisClaim = JsonConvert.DeserializeObject<Models.JsonModels.PostEventClaim>(codeByVal.ToString());
            if (thisClaim == null) throw new ArgumentNullException("thisClaim");


            order = _orderManagementService.GetOrderByOnDemandClaim(thisClaim.OnDemandCode);



            if (order != null)
            {
                return _webinarControllerOrchestrator.OnDemand(thisClaim.OrderId, thisClaim.OnDemandCode, User.Identity);
            }
            ModelState.AddModelError(string.Empty, "Invalid Code");
            return View();
        }

        public ActionResult Identify(string onDemandCode)
        {
            int _id;
            var isNum = Int32.TryParse(onDemandCode.Split('-')[0], out _id);
            string hasVal = null;
            if (onDemandCode.Split('-').Length > 1)
            {
                hasVal = onDemandCode.Split('-')[1];
            }
            var order = _orderManagementService.GetOrderById(_id);

            if (order != null && hasVal != null)
            {
                var model = new IdentifyModel
                {
                    Email = string.Empty,
                    OnDemandCode = onDemandCode,
                    FullName = string.Empty,
                    Institution = order.Institution,
                    idOrder = order.idOrder,
                    SignInModel = new SignInModel
                    {
                        ReturnUrl = "o/" + onDemandCode
                    }
                };

                return View(model);
            }
            else
            {
                string codeByVal = _orderManagementService.GetOnDemandClaimByCode(hasVal);

                var thisClaim = JsonConvert.DeserializeObject<Models.JsonModels.PostEventClaim>(codeByVal.ToString());
                if (thisClaim == null) throw new ArgumentNullException("thisClaim");

                order = _orderManagementService.GetOrderById(thisClaim.OrderId);

                if (order != null && hasVal != null)
                {
                    var model = new IdentifyModel
                    {
                        Email = string.Empty,
                        OnDemandCode = thisClaim.OnDemandCode,
                        FullName = string.Empty,
                        Institution = order.Institution,
                        idOrder = order.idOrder,
                        SignInModel = new SignInModel
                        {
                            ReturnUrl = "o/" + thisClaim.OnDemandCode
                        }
                    };

                    return View(model);
                }

            }


            _logger.Fatal("Unfound idOrder from demandcode: " + onDemandCode);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Identify(IdentifyModel identifyModel)
        {
            if (ModelState.IsValid)
            {
                int id;

                if (int.TryParse(identifyModel.OnDemandCode.Split('-')[0], out id))
                {
                    return _webinarControllerOrchestrator.Identify(identifyModel, id);
                }

                ModelState.AddModelError(string.Empty, "The Order Id in the browser address bar is not valid.");
            }

            return View(identifyModel);
        }

        public ActionResult GetWebinarDetailsCompact(int? id)
        {
            var html = "";
            if (id.HasValue)
            {
                var webinar = _webinarManagementService.GetWebinar(id.Value);

                if (webinar == null) return HttpNotFound();
                var model = new WebinarDetailsViewModel()
                {
                    Webinar = webinar,
                    WebinarFiles = webinar.WebinarFiles.ToList()
                };

                html = ViewHelpers.RenderViewToString(ControllerContext,
                    "~/Views/Shared/DisplayTemplates/DataTablesDisplayTemplates/GetWebinarDetails_Compact.cshtml",
                    model, true);
            }

            return Json(new { html = html });

        }

        [Route("webinar/getaddtocartjson")]
        [Route("webinar/geteditincartjson")]
        public ActionResult GetAddToCartJson(int? id)
        {
            var html = "";

            // ok, this is going to be a bit, ah, complicated, perhaps...
            // we want to show the "cart", which is a stack of partial views supported by a variety of models
            // what do we need to instantiate model(s) so we can feed the (nested) partial view the right things

            // Details method below has much of the same processing / uses many of the same routines

            if (id.HasValue)
            {
                Webinar webinar = null;
                WebinarDetailsViewModel model = null;

                InitializeDetailsWebinar(id.Value, out webinar);

                if (webinar == null) return HttpNotFound();

                InitializeDetailsModel(webinar, out model);

                // Incoming Order?  (Express checkout?)
                // for the time-being we are going to punt on in-process orders.  there is code in the _ShoppingCart view to show the use alterative content in the Search results grid

                InitializeDetailsState(webinar, model, id.Value);

                InitializeViewCentricProperties(model);

                InitializeInProcessProperties(id.Value, model, webinar);

                BuildConfirmOrderView(model);

                html = ViewHelpers.RenderViewToString(ControllerContext,
                    "~/Views/Webinar/Partials/_ShoppingCart.cshtml",
                    model, true);
            }

            return Json(new { html = html });

        }


        public ActionResult GetPresenterCompact(int? id)
        {
            var html = "";
            if (id.HasValue)
            {
                var webinar = _webinarManagementService.GetWebinar(id.Value);

                if (webinar == null) return HttpNotFound();
                var model = new WebinarDetailsViewModel()
                {
                    Webinar = webinar,
                    WebinarFiles = webinar.WebinarFiles.ToList()
                };

                html = ViewHelpers.RenderViewToString(ControllerContext,
                    "~/Views/Shared/DisplayTemplates/DataTablesDisplayTemplates/GetPresenter_Compact.cshtml",
                    model, true);
            }

            return Json(new { html = html });

        }

        public ActionResult Details(int? id, int? idOrder)
        {

            ClaimsIdentity claimsIdentityOfAuthenticatedUser = (ClaimsIdentity)User.Identity;
            var currentUser = User.Identity.Name ?? "anon";

            int incomingOrder = 0;
            if (idOrder != null)
            {
                incomingOrder = idOrder.Value;
            }

            if (id.HasValue)
            {

                Webinar webinar = null;
                WebinarDetailsViewModel model = null;

                InitializeDetailsWebinar(id.Value, out webinar);

                if (webinar == null) return HttpNotFound();

                InitializeDetailsModel(webinar, out model, incomingOrder);

                if (incomingOrder == 0)
                {
                    InitializeDetailsState(webinar, model, id.Value);
                    // form state, incl. stuff that will be posted back. 
                }
                else
                {
                    try
                    {
                        InitializeDetailsStateFromExpChcSubmit(webinar, model, id.Value, incomingOrder);
                        var logModelState = JsonConvert.SerializeObject(model.Order, Formatting.None,
                            new JsonSerializerSettings()
                            {
                                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                            });

                        _logger.Info("Details | returning checkout session by" + currentUser + " on: " + incomingOrder +
                                     " {" + logModelState + "}, Audit: {" + _appHelper.GetUserAuditInfo() + "}");

                    }
                    catch (Exception ex)
                    {
                        _logger.FatalException("Details | returning checkout session - ", ex);
                        throw;
                    }
                }
                InitializeViewCentricProperties(model);
                // mostly just stuff that helps determine layout of the page on load. Not meant to be sent back here to Server from the View
                if (model.Webinar == null)
                {
                    return HttpNotFound();
                }


                if (claimsIdentityOfAuthenticatedUser.HasClaim(
                    (claim) => claim.Type == Business.Constants.ClaimTypes.Admin))
                {
                    var webinars = _webinarManagementService.GetUpcomingWebinars().OrderBy(w => w.Date).Take(15);
                    //int totalNumberOrders;
                    //var ListOfUpcomingEvents =
                    //    webinars.Select(x => new 
                    //    {
                    //        idWebinar = x.idWebinar.ToString(),
                    //        Title = x.Title
                    //    }).ToList();

                    ViewBag.ListOfWebinars = new MultiSelectList(webinars, "idWebinar", "Title");

                    var aff = _affiliateManagementService.LoadByTTSDomain("bankwebinars");
                    // find a better check for admin
                    if (model.CheckoutInProcess)
                    {
                        CheckoutResumeByAdmin(id, model);
                    }

                    //if (webinar.Status == WebinarStatus.Active || webinar.Status == WebinarStatus.InProgress)
                    //{
                    //    _webinarControllerOrchestrator.GetCitrixRegsPerWebinar(webinar);
                    //}

                    model.ShowOrdersViewModel = new ShowOrdersViewModel
                    {
                        Affiliate = aff,
                        Orders = _orderManagementService.GetOrdersByWebinar(id.Value),
                        UserIsAdmin = true,
                        Webinar = model.Webinar
                    };

                    BuildConfirmOrderView(model);

                    return PartialView("DetailsAdmin", model);
                }

                if (claimsIdentityOfAuthenticatedUser.HasClaim(
                    (claim) => claim.Type == Business.Constants.ClaimTypes.Affiliate))
                {
                    var webinars = _webinarManagementService.GetUpcomingWebinars().OrderBy(w => w.Date).Take(15);

                    ViewBag.ListOfWebinars = new MultiSelectList(webinars, "idWebinar", "Title");


                    // Get the claims values
                    var ttsDomain = claimsIdentityOfAuthenticatedUser.Claims
                        .Where(c => c.Type == ClaimTypes.Affiliate)
                        .Select(c => c.Value)
                        .SingleOrDefault();

                    var aff = _affiliateManagementService.LoadByTTSDomain(ttsDomain);

                    if (model.CheckoutInProcess)
                    {
                        CheckoutResumeByAdmin(id, model);
                    }
                    model.ShowOrdersViewModel = new ShowOrdersViewModel
                    {
                        Affiliate = aff,
                        Orders =
                            _orderManagementService.GetOrdersByWebinar(id.Value)
                                .Where(o => o.idAffiliate == aff.idUserAff)
                                .ToList(),
                        UserIsAdmin = false,
                        Webinar = model.Webinar
                    };
                    model.PromoLinks = new PromoLinks
                    {
                        Affiliate = aff,
                        Links =
                            _affiliateManagementService.GetPromosByAffiliate(_globalConfig.Tenant, aff.idUserAff,
                                webinar.idWebinar)
                    };
                    BuildConfirmOrderView(model);
                    return PartialView("DetailsAffiliate", model);
                }

                InitializeInProcessProperties(id.Value, model, webinar);
                if (_stateService.HasValue(DomainConstants.OriginExpress))
                    if (model.Order != null)
                        model.Order.Origin = DomainConstants.OriginExpress;
                BuildConfirmOrderView(model);

                return View(model);
            }

            _logger.Error("Details Action invoked with null 'id' parameter");

            return RedirectToAction("allActive", new { eventsToShow = "upcoming" });
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult AddToICalendariCal(int icsWebinar, int icsOrder)
        {

            var webinar = _webinarControllerOrchestrator.GetWebinar(icsWebinar);

            var desc = webinar.Description;
            desc = HtmlHelpers.StripHtmlTags(desc);


            var icalStringbuilder = new StringBuilder();

            icalStringbuilder.AppendLine("BEGIN:VCALENDAR");
            icalStringbuilder.AppendLine("PRODID:-//Scheduled Event//EN");
            icalStringbuilder.AppendLine("VERSION:2.0");

            icalStringbuilder.AppendLine("BEGIN:VEVENT");
            icalStringbuilder.AppendLine("SUMMARY;LANGUAGE=en-us:" + webinar.Title);
            icalStringbuilder.AppendLine("CLASS:PUBLIC");
            icalStringbuilder.AppendLine(string.Format("CREATED:{0:yyyyMMddTHHmmssZ}", DateTime.UtcNow));
            icalStringbuilder.AppendLine("DESCRIPTION:" + desc);
            icalStringbuilder.AppendLine("X-ALT-DESC;FMTTYPE=text/html:" + webinar.Description);
            icalStringbuilder.AppendLine(string.Format("DTSTART:{0:yyyyMMddTHHmmss}", webinar.Date));
            icalStringbuilder.AppendLine(string.Format("DTEND:{0:yyyyMMddTHHmmss}",
                webinar.Date.AddHours((double)webinar.Duration)));
            icalStringbuilder.AppendLine("SEQUENCE:0");
            icalStringbuilder.AppendLine("UID:" + Guid.NewGuid());
            icalStringbuilder.AppendLine("END:VEVENT");
            icalStringbuilder.AppendLine("END:VCALENDAR");

            var bytes = Encoding.UTF8.GetBytes(icalStringbuilder.ToString());

            return this.File(bytes, "text/calendar", "thisEvent.ics");

        }

        //[System.Web.Mvc.HttpGet]
        //public ActionResult CalPicker(int icsOrder)
        //{

        //    var model = new CalPickerViewModel();

        //    return View(model);
        //}

        [System.Web.Mvc.HttpGet]
        public ActionResult ICalOrder(int icsOrder)
        {
            _logger.Info("ICalBuilder for: " + icsOrder);
            var order = _orderManagementService.GetOrderById(icsOrder);

            var webinar = _webinarControllerOrchestrator.GetWebinar(
                order.OrderRows.First(r => r.RowStatus == OrderRowStatus.Active).Webinar.idWebinar);
            var descBuilder = new StringBuilder();

            descBuilder.Append("A reminder of your webinar (" + _globalConfig.TenantPrefix + icsOrder + "). ");

            if (
                order.OrderRows.SingleOrDefault(r => r.RowStatus == OrderRowStatus.Active)
                    .RegistrationType.ShowLiveNotifications.ToLower() != "yes")
            {
                descBuilder.Append("At the time this reminder was generated, your registration did not include " +
                                   "'Live' attendance. If you would like to change that please visit us at "
                                   + _globalConfig.TenantURL + "/resume/" + order.idOrder + ". Otherwise, you'll see a notification that the recording is posted within 24 hours following the event. ");
            }
            else
            {
                descBuilder.Append("Pre-event and other information about this webinar can be reviewed at:  " +
                                   _globalConfig.TenantURL + "/j/" +
                                   order.OrderRows.SingleOrDefault(r => r.RowStatus == OrderRowStatus.Active)
                                       .TtsJoinUrl);
            }

            var icalStringbuilder = new StringBuilder();

            icalStringbuilder.AppendLine("BEGIN:VCALENDAR");
            icalStringbuilder.AppendLine("PRODID:-//Scheduled Event//EN");
            icalStringbuilder.AppendLine("VERSION:2.0");

            icalStringbuilder.AppendLine("BEGIN:VEVENT");
            icalStringbuilder.AppendLine("SUMMARY;LANGUAGE=en-us:" + webinar.Title);
            icalStringbuilder.AppendLine("CLASS:PUBLIC");
            icalStringbuilder.AppendLine(string.Format("CREATED:{0:yyyyMMddTHHmmssZ}", DateTime.UtcNow));
            icalStringbuilder.AppendLine("DESCRIPTION:" + descBuilder.ToString());
            icalStringbuilder.AppendLine("X-ALT-DESC;FMTTYPE=text/html:" + descBuilder.ToString());
            icalStringbuilder.AppendLine(string.Format("DTSTART:{0:yyyyMMddTHHmmss}", webinar.Date));
            icalStringbuilder.AppendLine(string.Format("DTEND:{0:yyyyMMddTHHmmss}",
                webinar.Date.AddHours((double)webinar.Duration)));
            icalStringbuilder.AppendLine("SEQUENCE:0");
            icalStringbuilder.AppendLine("UID:" + Guid.NewGuid());

            icalStringbuilder.AppendLine("BEGIN:VALARM");
            icalStringbuilder.AppendLine("TRIGGER:-PT60M");
            icalStringbuilder.AppendLine("ACTION:DISPLAY");
            icalStringbuilder.AppendLine("DESCRIPTION:Reminder");
            icalStringbuilder.AppendLine("END:VALARM");

            icalStringbuilder.AppendLine("END:VEVENT");
            icalStringbuilder.AppendLine("END:VCALENDAR");

            var bytes = Encoding.UTF8.GetBytes(icalStringbuilder.ToString());

            return this.File(bytes, "text/calendar", "thisEvent.ics");

        }

        private string MakeTitleSafe(string webinarTitle)
        {
            return webinarTitle;
        }

        public ActionResult AddToICalendarGmail(int glWebinar, int glOrder)
        {
            var webinar = _webinarControllerOrchestrator.GetWebinar(glWebinar);

            var icalStringbuilder = new StringBuilder();

            icalStringbuilder.Append("http://www.google.com/calendar/event?action=TEMPLATE");
            icalStringbuilder.Append("&text=" + webinar.Title);
            icalStringbuilder.Append("&dates=" + HttpUtility.HtmlDecode(webinar.Date.ToString("O")) + "/" +
                                     HttpUtility.HtmlDecode(webinar.Date.AddHours(1).ToString("O")));
            icalStringbuilder.Append("&location=Online");
            icalStringbuilder.Append("&details=" + HttpUtility.HtmlDecode(webinar.Description));
            //icalStringbuilder.Append("&trp=
            //icalStringbuilder.Append("&sprop=
            icalStringbuilder.Append("&sprop=name:" + _globalConfig.Tenant);

            return Redirect(icalStringbuilder.ToString());

        }

        public ActionResult DetailsDes(int? id, int? idOrder)
        {

            ClaimsIdentity claimsIdentityOfAuthenticatedUser = (ClaimsIdentity)User.Identity;
            var currentUser = User.Identity.Name ?? "anon";

            //if (currentUser.)

            int incomingOrder = 0;
            if (idOrder != null)
            {
                incomingOrder = idOrder.Value;
                //if ()
            }

            if (id.HasValue)
            {

                Webinar webinar = null;
                WebinarDetailsViewModel model = null;

                InitializeDetailsWebinar(id.Value, out webinar);

                if (webinar == null) return HttpNotFound();

                InitializeDetailsModel(webinar, out model, incomingOrder);

                if (incomingOrder == 0)
                {
                    InitializeDetailsState(webinar, model, id.Value);
                    // form state, incl. stuff that will be posted back. 
                }
                else
                {
                    try
                    {
                        InitializeDetailsStateFromExpChcSubmit(webinar, model, id.Value, incomingOrder);
                        var logModelState = JsonConvert.SerializeObject(model.Order, Formatting.None,
                            new JsonSerializerSettings()
                            {
                                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                            });

                        _logger.Info("Details | returning checkout session by" + currentUser + " on: " + incomingOrder +
                                     " {" + logModelState + "}, Audit: {" + _appHelper.GetUserAuditInfo() + "}");

                    }
                    catch (Exception ex)
                    {
                        _logger.FatalException("Details | returning checkout session - ", ex);
                        throw;
                    }
                }
                InitializeViewCentricProperties(model);
                // mostly just stuff that helps determine layout of the page on load. Not meant to be sent back here to Server from the View
                if (model.Webinar == null)
                {
                    return HttpNotFound();
                }


                if (claimsIdentityOfAuthenticatedUser.HasClaim(
                    (claim) => claim.Type == Business.Constants.ClaimTypes.Admin))
                {
                    var webinars = _webinarManagementService.GetUpcomingWebinars().OrderBy(w => w.Date).Take(15);
                    //int totalNumberOrders;
                    //var ListOfUpcomingEvents =
                    //    webinars.Select(x => new 
                    //    {
                    //        idWebinar = x.idWebinar.ToString(),
                    //        Title = x.Title
                    //    }).ToList();

                    ViewBag.ListOfWebinars = new MultiSelectList(webinars, "idWebinar", "Title");

                    var aff = _affiliateManagementService.LoadByTTSDomain("bankwebinars");
                    // find a better check for admin
                    if (model.CheckoutInProcess)
                    {
                        CheckoutResumeByAdmin(id, model);
                    }

                    model.ShowOrdersViewModel = new ShowOrdersViewModel
                    {
                        Affiliate = aff,
                        Orders = _orderManagementService.GetOrdersByWebinar(id.Value),
                        UserIsAdmin = true,
                        Webinar = model.Webinar
                    };
                    BuildConfirmOrderView(model);

                    return PartialView("DetailsAdmin", model);
                }

                if (claimsIdentityOfAuthenticatedUser.HasClaim(
                    (claim) => claim.Type == Business.Constants.ClaimTypes.Affiliate))
                {
                    var webinars = _webinarManagementService.GetUpcomingWebinars().OrderBy(w => w.Date).Take(15);

                    ViewBag.ListOfWebinars = new MultiSelectList(webinars, "idWebinar", "Title");


                    // Get the claims values
                    var ttsDomain = claimsIdentityOfAuthenticatedUser.Claims
                        .Where(c => c.Type == ClaimTypes.Affiliate)
                        .Select(c => c.Value)
                        .SingleOrDefault();

                    var aff = _affiliateManagementService.LoadByTTSDomain(ttsDomain);

                    if (model.CheckoutInProcess)
                    {
                        CheckoutResumeByAdmin(id, model);
                    }
                    model.ShowOrdersViewModel = new ShowOrdersViewModel
                    {
                        Affiliate = aff,
                        Orders =
                            _orderManagementService.GetOrdersByWebinar(id.Value)
                                .Where(o => o.idAffiliate == aff.idUserAff)
                                .ToList(),
                        UserIsAdmin = false,
                        Webinar = model.Webinar
                    };

                    BuildConfirmOrderView(model);
                    return PartialView("DetailsAffiliate", model);
                }

                InitializeInProcessProperties(id.Value, model, webinar);
                if (_stateService.HasValue(DomainConstants.OriginExpress))
                    if (model.Order != null)
                        model.Order.Origin = DomainConstants.OriginExpress;
                BuildConfirmOrderView(model);
                return View(model);
            }

            _logger.Error("Details Action invoked with null 'id' parameter");

            return RedirectToAction("allActive", new { eventsToShow = "upcoming" });
        }

        private void CheckoutResumeByAdmin(int? id, WebinarDetailsViewModel model)
        {
            var row = model.Order.OrderRows.Single(r => r.RowStatus == OrderRowStatus.Active);

            if (row.idWebinar == id)
            {
                //var userAccount = _membershipService.GetUserAccountByEmail(_globalConfig.Tenant, checkOrder.WebUser.email);
                if (model.Order.OrderStatus == OrderStatus.Billed
                    || model.Order.OrderStatus == OrderStatus.Paid
                    || model.Order.OrderStatus == OrderStatus.Submitted)
                {
                    if (_orderManagementService.FindPostEventClaimByOnDemandCode(model.Order).ExpiryDate >=
                        DateTime.Today)
                        model.RegistrationSummaryViewModel.DisplayPostEventMaterials = model.Order.idOrder;
                }

                //model.RegistrationSummaryViewModel.WebinarFiles = webinarFiles;
                model.UserOwnsThisEvent = model.Order.idOrder;
                model.Order = model.Order;
            }

            if ((model.Order.OrderStatus == OrderStatus.AwaitingVerification
                 || model.Order.OrderStatus == OrderStatus.InProcess) && row.idWebinar == id)
            {
                //var newJson =
                //    new JProperty(
                //        string.Concat("PendingReturnsToCheckoutBy-" + currentUser, TtsConfig.UtcNowAsCts.ToString(DomainConstants.DateTimeLongFormat)),
                //            new JObject(new JProperty("LegacyComments", model.Order.AdminComments))
                //        );

                //model.Order.AdminComments = newJson +", 'Prior Comments ': {" + model.Order.AdminComments + "}";
                model.UserHasOpenOrder = model.Order.idOrder;
            }
        }

        private void BuildConfirmOrderView(WebinarDetailsViewModel model)
        {

            Affiliate aff = _stateService.GetValue<Affiliate>(WebUiConstants.CurrentAffiliate);

            if (model.UserHasOpenOrder > 0)
            {
                model.CheckoutInProcess = true;
                model.MessageOrderStatus =
                    "<div align=\"center\" class=\"label-warning label\">Your order is InProcess and needs to be confirmed or canceled.</div>";

                var additionalLocationsViewModel =
                    model.RegistrationSummaryViewModel.AdditionalLocationsViewModel;
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
                    //populate viewbag for expresscheckout viewmodel
                    ViewBag.Order = order;


                    JObject existingJObject = null;
                    string comments = string.Empty;
                    if (!ReferenceEquals(null, order.AdminComments))
                    {
                        comments = order.AdminComments.Trim();
                    }

                    var newJson =
                        new JProperty(
                            string.Concat("ReturningOrder-",
                                TtsConfig.UtcNowAsCts.ToString(DomainConstants.DateTimeLongFormat)),
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
                    order.OrderDate = DateTime.Now;

                    PricesAndDiscounts pricesAndDiscounts = default(PricesAndDiscounts);
                    _orderManagementService.UpdateOrderChanges(orderRow.Order, ref pricesAndDiscounts);

                    if (orderRow.Discount != null)
                    {
                        ViewBag.DiscountCaption =
                            _orderManagementService.CalculateDiscountRedemption(orderRow.Discount, orderRow).Notes;
                    }


                    if (aff != null && aff.idUserAff == 19)
                    {
                        var _aff = _orderManagementService.DetermineAffiliateByAlternativeMeans(model.Order.idUser);
                        if (_aff.idUserAff != 19)
                        {
                            _stateService.SetValue(WebUiConstants.CurrentAffiliate, _aff);
                            aff = _aff;
                        }
                    }
                    try
                    {

                        order.Affiliate = aff;
                        order.idAffiliate = aff.idUserAff;
                        model.CheckoutConfirmViewModel = new CheckoutConfirmViewModel
                        {
                            AdditionalLocationCaption = DomainHelpers.BuildAdditionalLocationsCaption(orderRow),
                            AdjustUserDetailsPanel = new AdjustUserDetailsEditModel
                            {
                                Email = webUser.email,
                                FirstName = webUser.FirstName,
                                idUser = webUser.idUser,
                                LastName = webUser.LastName,
                                Title = webUser.Title,
                                Institution = orderRow.Order.Institution,
                                BillingAddress = new AddressModel()
                                {
                                    TypeOfAddress = AddressType.Billing,
                                    StreetAddress = order.BillingAddress,
                                    StreetAddress2 = order.BillingAddress2,
                                    City = order.BillingCity,
                                    Name = order.FirstName + ' ' + order.LastName,
                                    Phone = order.BillingPhone,
                                    State = order.BillingState,
                                    Zip = order.BillingZip
                                },
                                ShippingAddress = new AddressModel()
                                {
                                    TypeOfAddress = AddressType.Shipping,
                                    StreetAddress = order.ShippingAddress,
                                    StreetAddress2 = order.ShippingAddress2,
                                    City = order.ShippingCity,
                                    Name = order.FirstName + ' ' + order.LastName,
                                    Phone = order.ShippingPhone,
                                    State = order.ShippingState,
                                    Zip = order.ShippingZip
                                }
                            },
                            Affiliate = aff,
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
                            AdditionalLocationsViewModel = new AdditionalLocationsViewModel
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
                        _orderManagementService.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        _logger.FatalException("BuildConfirmOrderView: ", ex);
                    }
                }
            }
        }


        private void InitializeDetailsWebinar(int webinarId, out Webinar webinar)
        {
            webinar = null;

            if (webinarId == 883)
            {
                webinarId = _webinarManagementService.GetNextCompliancePerspectives() ?? -1;
            }

            webinar = _webinarManagementService.GetWebinar(webinarId);
        }

        private void InitializeDetailsModel(Webinar webinar, out WebinarDetailsViewModel model, int incomingOrder = -1)
        {
            model = null;

            if (webinar == null)
                return;

            model = new WebinarDetailsViewModel()
            {
                Webinar = webinar,
                WebinarFiles = webinar.WebinarFiles.ToList()
            };

            if (incomingOrder != -1)
            {
                model.UserOwnsThisEvent = incomingOrder;
                model.UserHasOpenOrder = incomingOrder;
            }
        }

        public void InitializeInProcessProperties(int webinarId, WebinarDetailsViewModel model, Webinar webinar)
        {

            // does use have on in-process or do they own it already?
            var usersOrders = _orderManagementService.GetOrdersByUserId(model.WebUser.idUser)
                .Where(o => o.OrderRows.SingleOrDefault(or => or.idWebinar == webinarId) != null);

            // perf tweak: ensures no multiple enumerations of usersOrders
            var checkOrders = usersOrders as Order[] ?? usersOrders.ToArray();

            if (checkOrders.Any()) // we know user has order in some state
            {
                foreach (var checkOrder in checkOrders)
                {
                    var row = checkOrder.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active);

                    var webinarFiles = webinar.WebinarFiles
                        .Select(f => f.fileDesc + "|" + f.fileLocation)
                        .ToArray();

                    if (row.idWebinar == webinarId)
                    {
                        //var userAccount = _membershipService.GetUserAccountByEmail(_globalConfig.Tenant, checkOrder.WebUser.email);
                        if (checkOrder.OrderStatus == OrderStatus.Billed
                            || checkOrder.OrderStatus == OrderStatus.Paid
                            || checkOrder.OrderStatus == OrderStatus.Submitted)
                        {
                            if (_orderManagementService.FindPostEventClaimByOnDemandCode(checkOrder).ExpiryDate >=
                                DateTime.Today)
                                model.RegistrationSummaryViewModel.DisplayPostEventMaterials = checkOrder.idOrder;
                        }

                        model.RegistrationSummaryViewModel.WebinarFiles = webinarFiles;
                        model.UserOwnsThisEvent = checkOrder.idOrder;

                        model.Order = checkOrder;

                        if (checkOrder.OrderStatus == OrderStatus.AwaitingVerification
                            || checkOrder.OrderStatus == OrderStatus.InProcess)
                        {
                            //var newJson =
                            //    new JProperty(
                            //        string.Concat("PendingReturnsToCheckoutBy-" + currentUser, TtsConfig.UtcNowAsCts.ToString(DomainConstants.DateTimeLongFormat)),
                            //            new JObject(new JProperty("LegacyComments", checkOrder.AdminComments))
                            //        );

                            //checkOrder.AdminComments = newJson +", 'Prior Comments ': {" + checkOrder.AdminComments + "}";
                            model.UserHasOpenOrder = checkOrder.idOrder;
                        }
                    }
                }
            }
        }

        private void InitializeDetailsState(Webinar webinar, WebinarDetailsViewModel model, int id)
        {
            IEnumerable<AdditionalLocation> additionalLocations = null;
            OrderRow orderRowForOrder = null;

            model.WebUser = Request.IsAuthenticated
                ? _membershipService.GetUserByEmailLoadedWithOrdersData(User.Identity.Name) ?? new WebUser()
                // new for admin/affiliates
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

                //additionalLocations = _appHelper.CheckAdditionalLocationsForValidEmail(orderRowForOrder.AdditionalLocation.ToList());
            }

            model.Topics = _webinarManagementService.GetTopicsPerWebinar(webinar.idWebinar).ToList();

            model.WebinarFiles = _webinarManagementService.GetWebinarFilesPerWebinar(webinar.idWebinar);
            var AddLocPrice = _orderManagementService.GetAdditionalLocationsPricing(webinar.idWebinar);
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
                        //Discount = row.Discount,
                        NumberOfAdditionalLocations = row.AdditionalLocation.Count(),
                        OrderStatus = row.Order.OrderStatus,
                        //Price = Convert.ToDecimal(row.RegistrationType.Price),
                        PricesAndDiscounts =
                            _orderManagementService.CalculateOrderCost(row.Order, additionalLocationsPricing.Item2),
                        //RowPrice = row.RowPrice,
                        RegistrationType = row.RegistrationType
                    };

                // populate AdditionalLocationOfferViewModel and AdditionalLocationAddViewModel

                if (model.CheckoutOptionsViewModel.DisplayOptionsViewModel.AdditionalLocationOfferViewModel
                        .AdditionalLocations != null)
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

                // populate RegistrationSummaryViewModel and AdditionalLocationsViewModel
                model.RegistrationSummaryViewModel = new RegistrationSummaryViewModel
                {
                    AdditionalLocationsViewModel = new AdditionalLocationsViewModel
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
        /// This method initializes state for variables which needed to be retrieved from the Database
        /// </summary>
        /// <param name="webinar"></param>
        /// <param name="model"></param>
        /// <param name="id"></param>
        /// <param name="idOrder"></param>
        private void InitializeDetailsStateFromExpChcSubmit(Webinar webinar, WebinarDetailsViewModel model, int id,
            int idOrder)
        {
            var order = _orderManagementService.GetOrderById(idOrder);
            if (order != null)
            {
                var user = _orderManagementService.GetWebUser(order.BillingEmail);
                _orderManagementService.AssignWebUserToOrder(user, order);
                OrderRow orderRowForOrder = order.OrderRows.Single(r => r.RowStatus == OrderRowStatus.Active);
                IEnumerable<AdditionalLocation> additionalLocations = orderRowForOrder.AdditionalLocation.ToList();
                ;
                additionalLocations = _appHelper.CheckAdditionalLocationsForValidEmail(additionalLocations.ToList());
                model.WebUser = user;
                model.Order = order;
                model.CheckoutInProcess = true;

                if (model.WebUser.idSubscriptionDiscount != null)
                {
                    model.UserHasDiscount =
                        _orderManagementService.GetDiscountById(model.WebUser.idSubscriptionDiscount.Value);
                    orderRowForOrder.Discount = model.UserHasDiscount;
                }

                var orderExists = model.Order != null;

                model.Topics = _webinarManagementService.GetTopicsPerWebinar(webinar.idWebinar).ToList();

                model.WebinarFiles = _webinarManagementService.GetWebinarFilesPerWebinar(webinar.idWebinar);
                var AddLocPrice = _orderManagementService.GetAdditionalLocationsPricing(webinar.idWebinar);
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
                    idOrder = model.Order.idOrder,
                    WebinarStatus = model.Webinar.Status
                };

                if (orderExists)
                {
                    model.CheckoutOptionsViewModel.OrderStatus = model.Order.OrderStatus;
                    model.CheckoutOptionsViewModel.OrderHasId = model.Order.idOrder > 0;
                    var row = orderRowForOrder;

                    model.CheckoutOptionsViewModel.RegistrationType = orderRowForOrder.RegistrationType;

                    var additionalLocationsPricing =
                        _orderManagementService.GetCostOfAdditionalLocations(orderRowForOrder.AdditionalLocation,
                            webinar.idWebinar
                        );

                    //if (orderRowForOrder.AdditionalLocation != null)
                    //{
                    //    orderRowForOrder.AdditionalLocation =
                    //        _appHelper.CheckAdditionalLocationsForValidEmail(orderRowForOrder.AdditionalLocation)
                    //            .ToList();
                    //}
                    // populate DisplayRowPriceViewModel of DisplayOptionsViewModel
                    model.CheckoutOptionsViewModel.DisplayOptionsViewModel.DisplayRowPriceViewModel =
                        new DisplayRowPriceViewModel
                        {
                            //Discount = row.Discount,
                            NumberOfAdditionalLocations = row.AdditionalLocation.Count(),
                            OrderStatus = row.Order.OrderStatus,
                            //Price = Convert.ToDecimal(row.RegistrationType.Price),
                            PricesAndDiscounts =
                                _orderManagementService.CalculateOrderCost(row.Order, additionalLocationsPricing.Item2),
                            //RowPrice = row.RowPrice,
                            RegistrationType = row.RegistrationType
                        };

                    // populate AdditionalLocationOfferViewModel and AdditionalLocationAddViewModel
                    model.CheckoutOptionsViewModel.DisplayOptionsViewModel.AdditionalLocationOfferViewModel.Emails =
                        row.AdditionalLocation.Select(al => al.Email.Trim()).ToList();

                    //model.CheckoutOptionsViewModel.DisplayOptionsViewModel.AdditionalLocationOfferViewModel.Emails =
                    //    model.CheckoutOptionsViewModel.DisplayOptionsViewModel.AdditionalLocationOfferViewModel
                    //        .AdditionalLocations.Select(al => al.Email).ToArray();


                    model.CheckoutOptionsViewModel.DisplayOptionsViewModel.AdditionalLocationOfferViewModel
                        .AdditionalLocationAddViewModel = new AdditionalLocationAddViewModel
                        {
                            AdditionalLocations = additionalLocations,
                            Price = additionalLocationsPricing.Item2
                        };
                    model.CheckoutOptionsViewModel.DisplayOptionsViewModel.AdditionalLocationOfferViewModel.Price =
                        additionalLocationsPricing.Item2;

                    // populate RegistrationSummaryViewModel and AdditionalLocationsViewModel
                    model.RegistrationSummaryViewModel = new RegistrationSummaryViewModel
                    {
                        AdditionalLocationsViewModel = new AdditionalLocationsViewModel
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
        }

        /// <summary>
        /// This method initializes state which is more relevant to display-text and variables used in the razor view
        /// (as distinct from variables which are meant to be captured in form submissions).
        /// </summary>
        /// <param name="model"></param>
        private void InitializeViewCentricProperties(WebinarDetailsViewModel model)
        {
            ViewBag.PageStyleType = "holy-grail-three-columns";

            model.UserHasOpenOrder = model.UserHasOpenOrder > 0 ? model.UserHasOpenOrder : 0;
            model.UserOwnsThisEvent = model.UserOwnsThisEvent > 0 ? model.UserOwnsThisEvent : 0;

            if (!model.CheckoutInProcess) model.CheckoutInProcess = false;

            ViewBag.metaDesc = string.Empty;
            ViewBag.metaKeywords = string.Empty;

            var userExists = model.WebUser.idUser > 0;
            var detectedTimeZone = USTimeZone.Central; //
            if (!ReferenceEquals(null, Request.Cookies["timezoneoffset"]))
            {
                AppHelper.ComputeTimeZone(Request.Cookies["timezoneoffset"].Value);
            }
            model.TimeZone = userExists ? model.WebUser.timeZone : detectedTimeZone;

            model.UserIsLoggedIn = userExists;

            model.SignUpCaption = "Sign Up!";
            model.ConfirmationCaption = "Confirmation";
            //model.Identity = _membershipService.GetUserAccountByEmail(_globalConfig.Tenant, model.WebUser.email);// ((ClaimsIdentity)User.Identity);
            model.Identity = ((ClaimsIdentity)User.Identity);
            model.UserAddressVerified = model.Identity.HasClaim(ClaimTypes.AddressVerified);
            model.TimeFormatDisplay = "<i>" + DateTimeHelper.FormatTime(model.Webinar.Date, model.TimeZone, false) +
                                      " - " +
                                      DateTimeHelper.FormatTime(
                                          model.Webinar.Date.AddHours((double)model.Webinar.Duration), model.TimeZone,
                                          true) + "<br /></i>";

            model.CeuShort = string.Empty;
            if (model.WebUser.idSubscriptionDiscount != null)
                model.UserHasDiscount = _orderManagementService.GetDiscountByUser(model.WebUser);

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

            //refactor according to: http://rickyrosario.com/blog/creating-an-rss-feed-in-asp-net-mvc/
            IList<Webinar> webinarsList = _webinarManagementService.GetAllActive().ToList();

            var dtos = new CalendarDTOAssembler().Entities2DTOs(webinarsList);

            TempData["ListUpcoming"] = webinarsList;
            SyndicationFeed feed = new SyndicationFeed("Custom JSON feed", "A Syndication extensibility sample", null);
            feed.LastUpdatedTime = DateTime.Now;
            feed.Items = from s in new string[] { "hello", "world" }
                         select new SyndicationItem()
                         {
                             Summary = SyndicationContent.CreatePlaintextContent(s)
                         };


            return Json(dtos, JsonRequestBehavior.AllowGet);
        }

        [System.Web.Mvc.AcceptVerbs(HttpVerbs.Get)]
        public ActionResult CalendarRss()
        {
            IList<Webinar> webinarsList = _webinarManagementService.GetUpcomingWebinars().ToList();

            var data = new CalendarRssDTOAssembler().Entities2DTOs(webinarsList);

            //var blog = data.SingleOrDefault();
            var postItems = data //.Where(p => p.Title = blog)
                .OrderBy(p => p.EventDate).Take(25)
                .Select(p => new SyndicationItem(p.Title, p.Content, new Uri(p.Url)));

            var feed = new SyndicationFeed("Events from " + _globalConfig.Tenant,
                _globalConfig.TenantURL + " is Webinars for the financial industry.",
                new Uri("http://www.bankwebinars.com/blog"), postItems)
            {

            };
            return new FeedResult(new Rss20FeedFormatter(feed));

        }

        [System.Web.Mvc.AcceptVerbs(HttpVerbs.Get)]
        public ActionResult CalendarRssFull()
        {
            IList<Webinar> webinarsList = _webinarManagementService.GetUpcomingWebinars().ToList();

            var data = new CalendarRssFullDTOAssembler().Entities2DTOs(webinarsList);

            //var blog = data.SingleOrDefault();
            var postItems = data //.Where(p => p.Title = blog)
                .OrderBy(p => p.EventDate).Take(25)
                .Select(p => new SyndicationItem(p.Title, p.Content, new Uri(p.Url)));

            var feed = new SyndicationFeed("Events from " + _globalConfig.Tenant,
                _globalConfig.TenantURL + " is Webinars for the financial industry.",
                new Uri("http://www.bankwebinars.com/blog"), postItems)
            {

            };
            return new FeedResult(new Rss20FeedFormatter(feed));

        }

        [HandleAjaxException]
        public ActionResult Clone(int? id)
        {
            if (id.HasValue)
            {
                try
                {
                    var webinarEditModel = _webinarControllerOrchestrator.BuildEditModelForWebinar(id.Value);

                    webinarEditModel.DateChanged = webinarEditModel.DateCreated = TtsConfig.UtcNowAsCts;

                    return PartialView("Partials/_CloneWebinar", webinarEditModel);
                }
                catch (Exception exception)
                {
                    _logger.ErrorException(string.Format("Clone Webinar | Session{0}", _appHelper.GetUserAuditInfo()),
                        exception);
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
                validationException.Errors.ForEach(
                    error => ModelState.AddModelError(FromFluentPrefix + error.PropertyName, error.ErrorMessage));

                return this.ModelStateJsonFromFluentValidator(ModelState);
            }
            catch (Exception exception)
            {
                _logger.ErrorException(string.Format("Create Webinar | Session{0}", _appHelper.GetUserAuditInfo()),
                    exception);
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
                _logger.ErrorException(string.Format("Create Webinar | Session{0}", _appHelper.GetUserAuditInfo()),
                    exception);
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
                validationException.Errors.ForEach(
                    error => ModelState.AddModelError(FromFluentPrefix + error.PropertyName, error.ErrorMessage));

                return this.ModelStateJsonFromFluentValidator(ModelState);
            }
            catch (Exception exception)
            {
                _logger.ErrorException(string.Format("Create Webinar | Session{0}", _appHelper.GetUserAuditInfo()),
                    exception);
                return Json(new { Result = WebUiConstants.Fail });
            }
        }

        [Authorize]
        [HandleAjaxException]
        public ActionResult Edit(int? id)
        {
            if (User != null)
            {
                ClaimsIdentity claimsIdentityOfAuthenticatedUser = (ClaimsIdentity)User.Identity;
                if (claimsIdentityOfAuthenticatedUser.HasClaim(
                    (claim) => claim.Type == Business.Constants.ClaimTypes.Affiliate))
                {

                    return Json(new { Result = WebUiConstants.Fail, Msg = "Not Authorized" }, JsonRequestBehavior.AllowGet);
                }
            }
            if (id.HasValue)
            {
                try
                {
                    var webinarEditModel = _webinarControllerOrchestrator.BuildEditModelForWebinar(id.Value);

                    return PartialView("Partials/_EditWebinar", webinarEditModel);
                }
                catch (Exception exception)
                {
                    _logger.ErrorException(string.Format("Edit Webinar | Session{0}", _appHelper.GetUserAuditInfo()),
                        exception);
                }

                return Json(new { Result = WebUiConstants.Fail, Msg = ServerErrorLoggedMsg }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { Result = WebUiConstants.Fail, Msg = EnterValidIdMsg }, JsonRequestBehavior.AllowGet);
        }


        [System.Web.Mvc.HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken(Order = 0)]
        [ValidateInput(false)]
        [HandleAjaxException(Order = 1)]
        public ActionResult Edit(WebinarEditModel webinarEditModel)
        {
            if (User != null)
            {
                ClaimsIdentity claimsIdentityOfAuthenticatedUser = (ClaimsIdentity)User.Identity;
                if (claimsIdentityOfAuthenticatedUser.HasClaim(
                    (claim) => claim.Type == Business.Constants.ClaimTypes.Affiliate))
                {
                    return Json(new { Result = WebUiConstants.Fail, Msg = "Not Authorized" }, JsonRequestBehavior.AllowGet);
                }
            }
            try
            {
                _webinarControllerOrchestrator.UpdateWebinarFromViewInput(webinarEditModel);

                return Json(new { Result = WebUiConstants.Success });
            }
            catch (ValidationException validationException)
            {
                validationException.Errors.ForEach(
                    error => ModelState.AddModelError(FromFluentPrefix + error.PropertyName, error.ErrorMessage));

                return this.ModelStateJsonFromFluentValidator(ModelState);
            }
            catch (DbEntityValidationException dbEntityValidationException)
            {
                var stringBuilder = new StringBuilder();

                foreach (var validationErrors in dbEntityValidationException.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        //Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName,
                        //    validationError.ErrorMessage);
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
                _logger.ErrorException(string.Format("Edit Webinar | Session{0}", _appHelper.GetUserAuditInfo()),
                    exception);
                return Json(new { Result = WebUiConstants.Fail });
            }
        }

        [Authorize]
        public ActionResult EditWebinarFromDetails(int? id)
        {
            if (User != null)
            {
                ClaimsIdentity claimsIdentityOfAuthenticatedUser = (ClaimsIdentity)User.Identity;
                if (claimsIdentityOfAuthenticatedUser.HasClaim(
                    (claim) => claim.Type == Business.Constants.ClaimTypes.Affiliate))
                {

                    return Json(new { Result = WebUiConstants.Fail, Msg = "Not Authorized" }, JsonRequestBehavior.AllowGet);
                }
            }
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

        [HandleAjaxException]
        [HttpPost]
        [AllowAnonymous]
        public JsonResult WebinarDataHandler(DTParametersWebinars param)
        {

            // based heavily on https://www.echosteg.com/jquery-datatables-asp.net-mvc5-server-side

            int totalNumberWebinars = 0;
            string searchTerm = param.searchTerm;
            int? affiliateId = param.affiliateId;

            try
            {
                IEnumerable<Webinar> dtsource = _dataTablesService.SearchWebinars(searchTerm, affiliateId ?? 19,
                    out totalNumberWebinars);

                // use automapper to flatten out the webinar records, in this specific case the data 
                //  model has circular references which cause problems with JSON serialization
                List<SearchDTO> dtoSource = new List<SearchDTO>();
                AutoMapper.Mapper.Map(dtsource, dtoSource);


                List<String> columnSearch = new List<string>();
                foreach (var col in param.Columns)
                {
                    columnSearch.Add(col.Search.Value);
                }

                List<SearchDTO> data = new DTResultSetWebinars().GetResult(param.Search.Value, param.SortOrder,
                    param.Start, param.Length, dtoSource, columnSearch);
                int count = new DTResultSetWebinars().Count(param.Search.Value, dtoSource, columnSearch);

                DataTableService<SearchDTO> result = new DataTableService<SearchDTO>
                {
                    draw = param.Draw,
                    data = data,
                    recordsFiltered = count,
                    recordsTotal = count
                };

                JsonResult jsonresult = Json(result);
                jsonresult.MaxJsonLength = int.MaxValue; // needed if/when the data is > 4mb

                return jsonresult;
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }

            //return Json(new { NotAuthorized = true });

        }

        [System.Web.Mvc.HttpGet]
        public JsonResult SetStatus(int idWebinar, string status, string subject)
        {
            var webinar = _webinarManagementService.GetWebinar(idWebinar);
            webinar.TitleAnnouncement = subject;
            if (status == "Pending")
            {
                webinar.Status = WebinarStatus.Scheduled;
                status = "Scheduled";
            }
            else
            {
                webinar.Status = WebinarStatus.Pending;
                status = "Pending";
            }

            _webinarManagementService.SaveChanges();

            try
            {

            }
            catch (Exception)
            {
                throw;
            }

            return Json(new { success = true, status }, JsonRequestBehavior.AllowGet);
        }

        [System.Web.Mvc.HttpGet]
        [ValidateJsonAntiForgeryToken]
        public JsonResult CreateCitrixWebinar(int idWebinar, string webinarKey)
        {
            _logger.Info("CreateCitrixWebinar begins");
            var webinar = _webinarManagementService.GetWebinar(idWebinar);

            webinar.WebinarKey = webinarKey;

            WebinarsApi api = new WebinarsApi();

            var cWebinar = api.getWebinar(_globalConfig.CitrixAuthMark,
                _globalConfig.ConvertToCitrixOrgKey(_globalConfig.CitrixOrgKeyMark),
                _globalConfig.ConvertToCitrixWebinarKey(webinar.WebinarKey));

            webinar.CitrixRegisterUrl = cWebinar.registrationUrl;


            var desc = webinar.Description.Replace("\n", String.Empty);
            desc = desc.Replace("\r", String.Empty);

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(desc);
            string output = doc.DocumentNode.ChildNodes.Aggregate("", (current, node) => current + node.InnerText);

            try
            {

                var wReqUpdate = new WebinarReqUpdate
                {
                    description = output,
                    subject = webinar.Title,
                    times = new List<DateTimeRange> {
                    new DateTimeRange
                    {
                        startTime = webinar.Date.AddHours(6),
                        endTime = webinar.Date.AddHours((double)webinar.Duration).AddHours(6)
                    }
                },
                    timeZone = "America/Chicago",
                    locale = WebinarReqUpdate.LocaleEnum.en_US
                };

                api.updateWebinar(_globalConfig.CitrixAuthMark,
                    _globalConfig.ConvertToCitrixOrgKey(_globalConfig.CitrixOrgKeyMark),
                    _globalConfig.ConvertToCitrixWebinarKey(webinar.WebinarKey), false, wReqUpdate);
            }
            catch (Exception ex)
            {
                _logger.FatalException("CreateCitrixWebinar | UpdateWebinar", ex);
                return Json(new { Result = "Failed", Msg = ex.Message }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                var audioUpdateBody = new AudioUpdate
                {
                    pstnInfo = new PstnInfoUpdate
                    {
                        tollFreeCountries = new List<TollFreeCountries> { TollFreeCountries.US }
                    },
                    type = AudioType.Hybrid
                };
                api.updateAudioInformation(_globalConfig.CitrixAuthMark,
                    _globalConfig.ConvertToCitrixOrgKey(_globalConfig.CitrixOrgKeyMark),
                    _globalConfig.ConvertToCitrixWebinarKey(webinar.WebinarKey), false, audioUpdateBody);
            }
            catch (Exception ex)
            {
                _logger.FatalException("CreateCitrixWebinar | UpdateAudio", ex);
                return Json(new { Result = "Failed", Msg = ex.Message }, JsonRequestBehavior.AllowGet);
            }

            var orgList = new List<CoorganizerReqCreate>();
            for (var i = 1; i < 5; i++)
            {
                var orgEmail = "";
                var orgKey = "";
                var orgName = "";
                var external = "";
                switch (i)
                {
                    case 2:

                        orgList.Add(new CoorganizerReqCreate
                        {
                            email = "kbennett@ttstrain.com",
                            external = false,
                            givenName = "Kyle Bennett",
                            organizerKey = _globalConfig.CitrixOrgKeyKyle,
                        });
                        break;
                    case 1:
                        orgList.Add(new CoorganizerReqCreate
                        {
                            email = "steve@ttstrain.com",
                            organizerKey = _globalConfig.CitrixOrgKeySteve,
                            givenName = "Steve Hueners",
                            external = false
                        });
                        break;
                    case 3:
                        orgList.Add(new CoorganizerReqCreate
                        {
                            email = "Wesley@ttstrain.com",
                            organizerKey = "",
                            givenName = "Wesley Kavelaris",
                            external = true,
                        });
                        break;
                    case 4:
                        orgList.Add(new CoorganizerReqCreate
                        {
                            email = "Dan@ttstrain.com",
                            organizerKey = "",
                            givenName = "Dan Heldmann",
                            external = true,
                        });
                        break;
                }

                try
                {
                    CoorganizersApi orgApi = new CoorganizersApi();

                    var createOrg = orgApi.createCoorganizers(_globalConfig.CitrixAuthMark,
                        _globalConfig.ConvertToCitrixOrgKey(_globalConfig.CitrixOrgKeyMark),
                        _globalConfig.ConvertToCitrixWebinarKey(webinar.WebinarKey), orgList);
                }
                catch (Exception ex)
                {
                    _logger.FatalException("CreateCitrixWebinar | AddOrg", ex);
                    return Json(new { Result = "Failed", Msg = ex.Message }, JsonRequestBehavior.AllowGet);
                }
            }

            try
            {
                var audio = api.getAudioInformation(_globalConfig.CitrixAuthMark,
                    _globalConfig.ConvertToCitrixOrgKey(_globalConfig.CitrixOrgKeyMark),
                    _globalConfig.ConvertToCitrixWebinarKey(webinar.WebinarKey));


                webinar.AccessCodeAttendee = audio.confCallNumbers["US"].accessCodes.attendee;
                webinar.AccessCodePresenter = audio.confCallNumbers["US"].accessCodes.panelist;
                webinar.AccessCodeOrganizer = audio.confCallNumbers["US"].accessCodes.organizer;
                webinar.AccessPhone = audio.confCallNumbers["US"].tollFree;
            }
            catch (Exception ex)
            {
                _logger.FatalException("CreateCitrixWebinar | GetAudio", ex);

                return Json(new { Result = "Failed", Msg = ex.Message }, JsonRequestBehavior.AllowGet);
            }

            // have we made sure this is called "outside" enough so updates above are retained
            // with current positioning? 

            webinar.Status = WebinarStatus.Active;

            _webinarControllerOrchestrator.UpdateWebinar(webinar);

            //_orderManagementService.CreateTestRegistration(webinar);

            return Json(new { Result = "Success", Msg = "" }, JsonRequestBehavior.AllowGet);
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
                    ConnectionInfoEditModel model = _webinarControllerOrchestrator.GetConnectionInfo(connectionInfoModel.WebinarKey);
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
                    ModelState.AddModelError(string.Empty, "Error condition. Email us at " + _globalConfig.TenantEmail + ". For immediate assistance, use our Help & Feedback button in your lower right screen.");

                    return Json(new { Result = WebUiConstants.Fail });
                }
            }
            return this.ModelStateJson(ModelState);
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
            try
            {
                string message;
                if (_webinarControllerOrchestrator.UpdateWebinarFiles(webinarFilesEditModel, out message))
                {
                    return Json(new { Result = WebUiConstants.Success });
                }

                return Json(new { Result = WebUiConstants.Fail, Message = message });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "There was a problem with the update operation. Please consult with the system administrator to resolve the issue."
                    );
                _logger.Fatal("UpdateWebinarFiles on " + webinarFilesEditModel.idWebinar + " " + ex.Message);
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

        public ActionResult ClickToJoin(string joinCode, int? idWebinar)
        {
            var webinar = _orderManagementService.GetWebinarByJoinCode(joinCode);

            var order = _orderManagementService.GetOrderByJoinCode(joinCode);

            order.OrderRows.SingleOrDefault(r => r.RowStatus == OrderRowStatus.Active).RegistrationType =
                _orderManagementService.GetRegTypeOfOrderRow(
                    order.OrderRows.SingleOrDefault(r => r.RowStatus == OrderRowStatus.Active).idRegType);

            var clickToJoinViewModel = new ClickToJoinViewModel
            {
                JoinCode = joinCode,
                RedirectLinkText = _globalConfig.TenantURL + "/" + joinCode,
                Webinar = webinar,
                Order = order
            };

            if (idWebinar.HasValue)
            {
                webinar = _orderManagementService.GetWebinarById(idWebinar.Value);

                clickToJoinViewModel = new ClickToJoinViewModel
                {
                    JoinCode = joinCode,
                    RedirectLinkText = _globalConfig.TenantURL + "/" + webinar.CitrixRegisterUrl,
                    Webinar = webinar,
                    Order = order
                };
            }

            clickToJoinViewModel.TimeZone = _orderManagementService.GetWebUser(order.idUser).timeZone;
            if (_stateService.HasValue(WebUiConstants.WebinarFromCode))
                _stateService.ClearValue(WebUiConstants.WebinarFromCode);
            _stateService.SetValue(WebUiConstants.WebinarFromCode, webinar);

            if (webinar.Status == WebinarStatus.Active)
            {
                string webinarUrl = _webinarControllerOrchestrator.OpenMeeting(joinCode, User.Identity);

                return new RedirectResult(webinarUrl);
            }

            return View(clickToJoinViewModel);
        }

        public ActionResult OpenMeeting(string joinCode)
        {
            try
            {
                // It is assumed that the url will always be in the database by now. Hance, no special message/handling for an empty string.
                string webinarUrl = _webinarControllerOrchestrator.OpenMeeting(joinCode, User.Identity);

                return new RedirectResult(webinarUrl);
            }
            catch (Exception exception)
            {
                _logger.ErrorException(string.Format("OpenMeeting | Session {0}", _appHelper.GetUserAuditInfo()), exception);
                throw;
            }
        }

        [HttpGet]
        public ActionResult UpdateWebinarRecordingBatch(int idWebinar, string recordingURL, DateTime livePlusFive)
        {
            var webinar = _webinarManagementService.GetWebinar(idWebinar);
            webinar.RecordingUrl = recordingURL;
            webinar.LivePlusFiveValue = livePlusFive;

            _webinarControllerOrchestrator.SendRecordingIsPostedBatch(idWebinar);
            return Content("ok");
        }

        [HttpGet]
        public ActionResult UpdateWebinarRecordingPerOrder(int idOrder, string recordingURL, string livePlusFive, string note)
        {
            var order = _orderManagementService.GetOrderById(idOrder);
            order.UserComments = note + " Prior Comments: " + order.UserComments;
            _orderManagementService.SaveOrderChanges(order, null, null, OrderGenesis.Resend);
            _webinarControllerOrchestrator.SendRecordingIsPostedPerOrder(idOrder, note);
            return Content("ok");
        }

        public ActionResult UpdateWebinarRecording(WebinarDetailsViewModel webinarDetailsViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string message;
                    if (_webinarControllerOrchestrator.UpdateWebinarRecording(webinarDetailsViewModel, out message))
                    {
                        return Json(new { Result = WebUiConstants.Success });
                    }

                    return Json(new { Result = WebUiConstants.Fail, Message = message });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, "Invalid recording location requested: " + ex.Message);
                }
            }
            return this.ModelStateJson(ModelState);
        }
    }
}
