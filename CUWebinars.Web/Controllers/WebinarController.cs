using System.Web.Routing;
using CUWebinars.Business.AccountService;
using CUWebinars.Business.Models;
using CUWebinars.Business.Services;
using CUWebinars.Web.Core;
using CUWebinars.Web.Core.Browsers.Webinars;
using CUWebinars.Web.Helpers;
using CUWebinars.Web.Infrastructure.Extensions;
using CUWebinars.Web.Models;
using CUWebinars.Web.Services;
using CUWebinars.Web.ViewModel;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Web.Hosting;
using System.Web.Mvc;


namespace CUWebinars.Web.Controllers
{
    public class WebinarController : Controller
    {
        private IStateService _stateService;
        private readonly IAppHelper _appHelper;
        private GlobalConfig _globalConfig = GlobalConfig.GlobalConfigSingletonCreator.UniqueInstance;

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
            ILogger logger,
            IStateService stateService,
            IAppHelper appHelper)
        {
            _membershipService = membershipService;
            _orderManagementService = orderManagementService;
            _webinarManagementService = webinarManagementService;
            _logger = logger;
            _stateService = stateService;
            _appHelper = appHelper;
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
                    ViewBag.Title = _globalConfig.Tenant + " related to IRAs ";
                    break;
                case "15":
                    ViewBag.TopicCaption = "Compliance ";
                    ViewBag.Title = _globalConfig.Tenant + " related to Compliance ";
                    break;
                case "17":
                    ViewBag.TopicCaption = "Customer Service ";
                    ViewBag.Title = _globalConfig.Tenant + " related to Customer Service ";
                    break;
                case "18":
                    ViewBag.TopicCaption = "Security ";
                    ViewBag.Title = _globalConfig.Tenant + " related to Security ";
                    break;
                case "19":
                    ViewBag.TopicCaption = "Operations ";
                    ViewBag.Title = _globalConfig.Tenant + " related to Operations ";
                    break;
                case "20":
                    ViewBag.TopicCaption = "Auditing ";
                    ViewBag.Title = _globalConfig.Tenant + " related to Auditing ";
                    break;
                case "21":
                    ViewBag.TopicCaption = "Sales ";
                    ViewBag.Title = _globalConfig.Tenant + " related to Sales ";
                    break;
                case "22":
                    ViewBag.TopicCaption = "Lending ";
                    ViewBag.Title = _globalConfig.Tenant + " related to Lending ";
                    break;
                case "23":
                    ViewBag.TopicCaption = "Human Resources ";
                    ViewBag.Title = _globalConfig.Tenant + " related to Human Resources ";
                    break;
                case "25":
                    ViewBag.TopicCaption = "Computer Skills ";
                    ViewBag.Title = _globalConfig.Tenant + " related to Computer Skills ";
                    break;
                case "26":
                    ViewBag.TopicCaption = "Risk Management ";
                    ViewBag.Title = _globalConfig.Tenant + " related to Risk Management ";
                    break;
                case "27":
                    ViewBag.TopicCaption = "Teller ";
                    ViewBag.Title = _globalConfig.Tenant + " related to Teller ";
                    break;
            }

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
                //TODO: [UPDATED SEARCH TASK]: 
                // my original search code included a way
                var webinars = _webinarManagementService.GetWebinarByPresenterLastName(searchTerm);
                var webinarsByTopic = _webinarManagementService.GetWebinarByDescription(searchTerm);
                var unionOfResultSets = webinars.Union(webinarsByTopic);

                ViewBag.Title = "Search Results";

                return View(unionOfResultSets);
            }
            catch (Exception exception)
            {
                _logger.ErrorException(string.Format("Search | Session {0}", _appHelper.GetUserAuditInfo()), exception);
                ModelState.AddModelError(string.Empty, "There's been an error at the server. If the error recurs, please call 800-831-0678 ext 706 for immediate assistance.");
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

                var usersOrders = _orderManagementService.GetOrdersByUserId(model.WebUser.idUser)
                    .Where(o => o.OrderRows.SingleOrDefault(or => or.idWebinar == id.Value) != null);

                var checkOrders = usersOrders as Order[] ?? usersOrders.ToArray();
                // perf tweak: ensures no multiple enumerations of usersOrders
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
                            model.RegistrationSummaryViewModel.WebinarFiles = webinarFiles;
                            model.RegistrationSummaryViewModel.UserOwnsThisEvent =
                                model.UserOwnsThisEvent = checkOrder.idOrder;
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
                    var webUser = orderRow.Order.WebUser;
                    var userFullName = string.Concat(webUser.FirstName, " ", webUser.LastName);
                    var addresses = webUser.Addresses.ToArray();
                    var billingAddress = addresses.First(a => a.AddressType == WebUiConstants.BillingAddress);
                    var shippingAddress = addresses.FirstOrDefault(a => a.AddressType == WebUiConstants.ShippingAddress);
                    //var CheckoutDiscount = orderRow.Discount == null ? string.Empty : _orderManagementService.GetDiscountByCode()


                    model.CheckoutConfirmViewModel = new CheckoutConfirmViewModel
                    {
                        AdditionalLocationCaption = DomainHelpers.BuildAdditionalLocationsCaption(orderRow),
                        AdminComments = model.Order.AdminComments,
                        AffiliateComments = model.Order.AffiliateComments,
                        //CCUserDetails = "",
                        CheckoutDiscountCode = orderRow.Discount == null ? string.Empty : orderRow.Discount.DiscountCode,
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
                            AdditionalLocations = model.Order.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active).AdditionalLocation,
                            Addresses = additionalLocationsViewModel.Addresses,
                            OptionsCost = additionalLocationsViewModel.OptionsCost,
                        },
                        OrderRowExists = true,
                        OrderRowHasId = true,
                        OrderStatus = OrderStatus.InProcess,
                        Origin = model.Order.Origin,
                        UserComments = model.Order.UserComments,
                        UserDetails =
                            string.Concat(userFullName, " - ", orderRow.Order.Institution, "<br>", webUser.email),
                        UserFullname = userFullName,
                        UserType = UserType.Customer
                    };
                }

                if (model.Webinar == null)
                {
                    return HttpNotFound();
                }

                ClaimsIdentity claimsIdentityOfAuthenticatedUser = (ClaimsIdentity)User.Identity;

                //if (claimsIdentityOfAuthenticatedUser.HasClaim(Business.Constants.ClaimTypes.Admin, Business.Constants.ClaimValues.Admin))
                //{
                //    return View("DetailsAdmin", model);
                //}

                if (claimsIdentityOfAuthenticatedUser.HasClaim((claim) => claim.Type == Business.Constants.ClaimTypes.Affiliate))
                {
                    return PartialView("DetailsAffiliate", model);
                }

                return View(model);
            }

            _logger.Error("Details Action invoked with null 'id' parameter");

            return RedirectToAction("allActive", new { eventsToShow = "upcoming" } );
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

            if (Request.IsAuthenticated)
            {
                
            }

            model.WebUser = Request.IsAuthenticated
                ? _membershipService.GetUserByEmailLoadedWithOrdersData(User.Identity.Name)
                : new WebUser();

            if (Request.IsAuthenticated && model.WebUser.Orders.FirstOrDefault() != null)
            {
                model.Order = model.WebUser.Orders.FirstOrDefault(o => o.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active).idWebinar == id);
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

            //Check if needed. Can't the same info be obtained (within RAZOR)
            // by simply checking 'currentUser'?
            // ->
            // Not strictly needed, but more readable in the razor e.g. if(Model.UserIsLoggedIn)
            // [sjh] agreed.
            model.UserIsLoggedIn = userExists;

            model.SignUpCaption = "Sign Up!";
            model.ConfirmationCaption = "Confirmation";
            model.Identity = ((ClaimsIdentity)User.Identity);
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

            ViewBag.PresenterId = new SelectList(_webinarManagementService.GetAllPresenters(), "Id", "Biography",
                webinar.idPresenter);
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
            ViewBag.PresenterId = new SelectList(_webinarManagementService.GetAllPresenters(), "Id", "Biography",
                webinar.idPresenter);
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
            ViewBag.PresenterId = new SelectList(_webinarManagementService.GetAllPresenters(), "Id", "Biography",
                webinar.idPresenter);
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
        public ActionResult UpdateConnectionInfo(ConnectionInfoModel model)
        {
            var webinar = _webinarManagementService.GetWebinar(model.idWebinar);
            webinar.AccessCodeAttendee = model.AccessCodeAttendee;
            webinar.AccessCodeOrganizer = model.AccessCodeOrganizer;
            webinar.AccessCodePresenter = model.AccessCodePresenter;
            webinar.CitrixRegisterUrl = model.CitrixRegisterURL;
            webinar.OrganizerOAuthKey = model.OrganizerOAuthKey;
            webinar.OrganizerKey = model.OrganizerKey;
            webinar.WebinarKey = model.WebinarKey;


            _webinarManagementService.UpdateWebinar(webinar);

            return PartialView("Partials/_UpdateConnectionInfo", model);
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
                _webinarManagementService.Dispose();

                base.Dispose(true);
            }
            _disposed = true;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateWebinarFiles(ConnectionInfoModel connectionInfoModel)
        {
            /*
                This is a batch update operation. The WebinarFiles collection contains the webinar files to be updated. 
                The three possibilities are updated, deleted and created. (Even if a file was unchanged at the client, it will be treated as updated.)
                    * If the idWebinar in the WebinarFiles is a positive number and does not end with -D, it is updated
                    * If the idWebinar in the WebinarFiles is 0, and does not end with -ND, it has been created (suffix of 'N' appended at client deserializes to 0)
                    * If the fileDesc in the WebinarFile ends with -D, it has been marked for deletion.             
             */

            try
            {
                var deletedFiles = connectionInfoModel.WebinarFiles.Where(f => f.fileDesc.EndsWith("-D")).ToList();
                var newFiles =
                    connectionInfoModel.WebinarFiles.Where(f => f.idWebinarFile == 0 && !f.fileDesc.EndsWith("-ND"))
                        .ToList();
                var updatedFiles =
                    connectionInfoModel.WebinarFiles.Where(f => f.idWebinarFile > 0 && !f.fileDesc.EndsWith("-D"))
                        .ToList();

                _webinarManagementService.AddWebinarFiles(newFiles);
                _webinarManagementService.DeleteWebinarFiles(deletedFiles);
                _webinarManagementService.UpdateWebinarFiles(updatedFiles);

                return Json(new { result = WebUiConstants.Success });
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
    }
}
