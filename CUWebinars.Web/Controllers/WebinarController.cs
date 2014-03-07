using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using CUWebinars.Business.AccountService;
using CUWebinars.Business.Repository;
using CUWebinars.Business.Services;
using CUWebinars.Web.Data.Repositories.Interfaces;
using CUWebinars.Web.Core.Browsers.Webinars;
using CUWebinars.Web.Helpers;
using CUWebinars.Web.Services;
using CUWebinars.Web.ViewModel;
using Ninject.Extensions.Logging;
using CUWebinars.Business.Models;


namespace CUWebinars.Web.Controllers
{
    public class WebinarController : Controller
    {
        private TTSWebinarsContext db = new TTSWebinarsContext();
        private IMailService _mail;
        IStateService stateService = new StateService();

        //Steve added MembershipService dependancy to allow for 'currentUser' in Details.
        public IMembershipService membershipService;
        private readonly IWebinarRepository _webinarRepository;
        private readonly IOrderManagementService _orderManagementService;
        public ILogger Logger { get; set; }

        public WebinarController(MembershipService membershipService, IMailService mail, IWebinarRepository webinarRepository, ILogger logger, IOrderManagementService orderManagementService)
        {

            this.membershipService = membershipService;
            _mail = mail;
            _webinarRepository = webinarRepository;
            Logger = logger;
            _orderManagementService = orderManagementService;
        }
        //
        // GET: /Webinar/

        public ActionResult Index()
        {
            var webinars = _webinarRepository.GetAllActive();
            ViewBag.TopicCaption = " ";
            ViewBag.Title = "All Listed Events for CUWebinars";

            webinars = _webinarRepository.GetUpcoming().OrderBy(w => w.Date);
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
                    ViewBag.Title = "CUWebinars related to IRAs ";
                    break;
                case "15":
                    ViewBag.TopicCaption = "Compliance ";
                    ViewBag.Title = "CUWebinars related to Compliance ";
                    break;
                case "17":
                    ViewBag.TopicCaption = "Customer Service ";
                    ViewBag.Title = "CUWebinars related to Customer Service ";
                    break;
                case "18":
                    ViewBag.TopicCaption = "Security ";
                    ViewBag.Title = "CUWebinars related to Security ";
                    break;
                case "19":
                    ViewBag.TopicCaption = "Operations ";
                    ViewBag.Title = "CUWebinars related to Operations ";
                    break;
                case "20":
                    ViewBag.TopicCaption = "Auditing ";
                    ViewBag.Title = "CUWebinars related to Auditing ";
                    break;
                case "21":
                    ViewBag.TopicCaption = "Sales ";
                    ViewBag.Title = "CUWebinars related to Sales ";
                    break;
                case "22":
                    ViewBag.TopicCaption = "Lending ";
                    ViewBag.Title = "CUWebinars related to Lending ";
                    break;
                case "23":
                    ViewBag.TopicCaption = "Human Resources ";
                    ViewBag.Title = "CUWebinars related to Human Resources ";
                    break;
                case "25":
                    ViewBag.TopicCaption = "Computer Skills ";
                    ViewBag.Title = "CUWebinars related to Computer Skills ";
                    break;
                case "26":
                    ViewBag.TopicCaption = "Risk Management ";
                    ViewBag.Title = "CUWebinars related to Risk Management ";
                    break;
                case "27":
                    ViewBag.TopicCaption = "Teller ";
                    ViewBag.Title = "CUWebinars related to Teller ";
                    break;
            }

            var webinars = _webinarRepository.GetByTopic(ID);
            //var dtos = new WebinarDTOAssembler().Entities2DTOs(webinars);
            //return View(dtos);
            return View(webinars);
        }

        public ActionResult AllActive(string eventsToShow)
        {
            var webinars = _webinarRepository.GetAllActive();
            ViewBag.TopicCaption = " ";
            ViewBag.Title = "All Listed Events for CUWebinars";
            if (eventsToShow == "upcoming")
            {
                webinars = _webinarRepository.GetUpcoming().OrderBy(w => w.Date);
                ViewBag.Title = "All Upcoming Events for CUWebinars";
            }
            if (eventsToShow == "recorded")
            {
                webinars = _webinarRepository.GetRecorded().OrderBy(w => w.Date);
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

//        [AcceptVerbs(HttpVerbs.Post)]
//        public ActionResult Signup(int id
//                                    , int mode
//                                    , int? connectionsCount
//                                    , int? sixMonthPaidConnectionsCount
//                                    , int? twelveMonthPaidConnectionsCount
//                                    , string currentUserEmail
//            )
//        {

//            //        IDictionary<int, int> additionalLocations = Request.Params.AllKeys
//            //.Where(x => x.StartsWith("ChooseOptions.AdditionalLocationsCount"))
//            //.Where(x => Request.Params[x] != null && Request.Params[x].ToString().Length > 0)
//            //.Select(x => new { key = x.Replace(@"ChooseOptions.AdditionalLocationsCount", ""), value = Request.Params[x] })
//            //.ToDictionary(x => int.Parse(x.key), x => int.Parse(x.value));
//            IDictionary<string, string> addEmails = Request.Params.AllKeys
//                .Where(x => x.StartsWith("Email"))
//                .Where(x => Request.Params[x] != null && Request.Params[x].ToString().Length > 0)
//                .Select(x => new { key = x, value = Request.Params[x] })
//                .ToDictionary(x => (x.key), x => (x.value));
//            //IDictionary<int, int> sixMonthPaidAdditionalLocationsCount = Request.Params.AllKeys
//            //    .Where(x => x.StartsWith("ChooseOptions.SixMonthPaidAdditionalLocationsCount"))
//            //    .Where(x => Request.Params[x] != null && Request.Params[x].ToString().Length > 0)
//            //    .Select(x => new { key = x.Replace(@"ChooseOptions.SixMonthPaidAdditionalLocationsCount", ""), value = Request.Params[x] })
//            //    .ToDictionary(x => int.Parse(x.key), x => int.Parse(x.value));
//            //IDictionary<int, int> twelveMonthPaidAdditionalLocationsCount = Request.Params.AllKeys
//            //    .Where(x => x.StartsWith("ChooseOptions.TwelveMonthPaidAdditionalLocationsCount"))
//            //    .Where(x => Request.Params[x] != null && Request.Params[x].ToString().Length > 0)
//            //    .Select(x => new { key = x.Replace(@"ChooseOptions.TwelveMonthPaidAdditionalLocationsCount", ""), value = Request.Params[x] })
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
//                currentOrder.InitiatedBy = _orderManagementService.GetOrderInitiator();
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
//            var additionalLocationsOrderRowOptions = orderRow.OrderRowOptions.OfType<AdditionalLocationsOrderRowOption>();
//            if (additionalLocationsOrderRowOptions.Count() > 0)
//            {
//                int additionalLocationsCount;
//                if (int.TryParse(Request["AdditionalLocationsCount" + orderRow.idOrderRow], out additionalLocationsCount))
//                {
//                    additionalLocationsOrderRowOptions.Single().AdditionalLocationsCount = additionalLocationsCount;
//                }
//                else if (Request["AdditionalLocationsCount" + orderRow.idOrderRow] == "")
//                {
//                    additionalLocationsOrderRowOptions.Single().AdditionalLocationsCount = 0;
//                }
//            }

//            var options = _orderManagementService.GetOptionsByWebinarId(webinar.idWebinar);

//            //var option = options.OfType<AdditionalLocationsOption>().SingleOrDefault();
//            var option = options.SingleOrDefault(o => o.Type == "additional_location");

//            if (option != null)
//            {
//                var orderRowOption = new AdditionalLocationsOrderRowOption
//                {
//                    AdditionalLocationsCount = connectionsCount.HasValue ? connectionsCount.Value : 0,
//                    Option = option,
//                    OrderRow = orderRow,
//                    OptionDescription = option.OptionExplain,
//                    OptionPrice = Convert.ToDecimal(option.PriceToAdd),
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
//                    //orderRowOption.AdditionalLocationsCount = freeConnectionsCount;
//                    //if (orderRowOption.AdditionalLocationsCount > 3)
//                    //{
//                    //    orderRowOption.AdditionalLocationsCount = 3;
//                    //}


//                    //if ((RegistrationType)mode == RegistrationType.Twelve_Month_Subscription)
//                    //{
//                    //    orderRowOption.AdditionalLocationsCount += twelveMonthPaidConnectionsCount.HasValue ? twelveMonthPaidConnectionsCount.Value : 0;
//                    //}
//                    //else
//                    //{
//                    //    orderRowOption.AdditionalLocationsCount += sixMonthPaidConnectionsCount.HasValue ? sixMonthPaidConnectionsCount.Value : 0;
//                    //}
//                }

//                orderRow.OrderRowOptions.Add(orderRowOption);
//                //orderRow.Options.Add(orderRowOption);
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

            var webinars = _webinarRepository.GetByTopic(Convert.ToInt32(webinarsBrowserRequest.Search.Replace("TopicID=", "")));
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

        public ActionResult Details(int id)
        {
            var user = new WebUser();
            if (Request.IsAuthenticated)
            {
                user = membershipService.GetUserByEmail(User.Identity.Name);
            }
            var usersOrders = _orderManagementService.GetOrdersByUserId(user.idUser);
            //if (usersOrders == null) 
            //    throw new ArgumentNullException("usersOrders");
            ViewBag.userHasOpenOrder = 0;
            ViewBag.userOwnsThisEvent = 0;
            ;
            ViewBag.PageStyleType = "holy-grail-three-columns";
            //

            var model = new WebinarDetailsViewModel()
            {
                WebUser = user
                ,
                Affiliate = stateService.GetValue<Affiliate>("CurrentAffiliate")
                ,
                Webinar = db.Webinars.Find(id)
                ,
                Options = _orderManagementService.GetOptionsByWebinarId(id, false)
                ,
                Order = null
            };

            if (usersOrders != null && usersOrders.Count > 0)
            {
                foreach (var checkOrder in usersOrders)
                {
                    if (checkOrder.OrderRows.Single().idWebinar == id)
                    {
                        ViewBag.userOwnsThisEvent = checkOrder.idOrder;
                        model.Order = checkOrder;
                    }
                    if (checkOrder.OrderRows.Single().Status == OrderRowStatus.InProcess
                        && checkOrder.OrderRows.Single().idWebinar != id)
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

        //
        // GET: /Webinar/Create

        public ActionResult Create()
        {
            ViewBag.PresenterId = new SelectList(db.Presenters, "Id", "Biography");
            return View();
        }

        //
        // POST: /Webinar/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Webinar webinar)
        {
            if (ModelState.IsValid)
            {
                db.Webinars.Add(webinar);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.PresenterId = new SelectList(db.Presenters, "Id", "Biography", webinar.idPresenter);
            return View(webinar);
        }

        //
        // GET: /Webinar/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Webinar webinar = db.Webinars.Find(id);
            if (webinar == null)
            {
                return HttpNotFound();
            }
            ViewBag.PresenterId = new SelectList(db.Presenters, "Id", "Biography", webinar.idPresenter);
            return View(webinar);
        }

        //
        // POST: /Webinar/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Webinar webinar)
        {
            if (ModelState.IsValid)
            {
                db.Entry(webinar).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PresenterId = new SelectList(db.Presenters, "Id", "Biography", webinar.idPresenter);
            return View(webinar);
        }

        //
        // GET: /Webinar/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Webinar webinar = db.Webinars.Find(id);
            if (webinar == null)
            {
                return HttpNotFound();
            }
            return View(webinar);
        }

        //
        // POST: /Webinar/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Webinar webinar = db.Webinars.Find(id);
            db.Webinars.Remove(webinar);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
