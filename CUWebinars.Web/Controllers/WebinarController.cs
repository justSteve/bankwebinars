using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Web.Http;
using System.Web.Mvc;
using CUWebinars.Business.AccountService;
using CUWebinars.Business.Repository;
using CUWebinars.Business.Services;
using CUWebinars.Web.Core;
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
        private readonly TTSWebinarsContext _db;
        
        IStateService stateService = new StateService();

        //Steve added MembershipService dependancy to allow for 'currentUser' in Details.
        private readonly IMembershipService _membershipService;
        private readonly IWebinarRepository _webinarRepository;
        private readonly IOrderManagementService _orderManagementService;
        private readonly ILogger _logger;

        public WebinarController(
            IMembershipService membershipService, 
            IWebinarRepository webinarRepository, 
            IOrderManagementService orderManagementService, 
            ILogger logger,
            TTSWebinarsContext context)
        {
            _db = context;
            _membershipService = membershipService;
            _webinarRepository = webinarRepository;
            _orderManagementService = orderManagementService;
            _logger = logger;
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

        public PartialViewResult GetAdditionalLocationByOrderId(int webUserId, int webinarId)
        {
            //var order = _orderManagementService.GetOrderById(id);

            var addAdditionalLocationViewModel = new AddAdditionalLocationViewModel
            {
                //AdditionalLocations = order.OrderRows.First().AdditionalLocation.ToList()
                AdditionalLocations = new AdditionalLocation[] { new AdditionalLocation { Email = "dave@dave.com" }, new AdditionalLocation { Email = "monty@python.com" } }
            };

            return PartialView("~/Views/Webinar/Partials/_AdditionalLocationsModal.cshtml", addAdditionalLocationViewModel);
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
            //_orderManagementService.GetOrderById(1162);
            ViewBag.userHasOpenOrder = 0;
            ViewBag.userOwnsThisEvent = 0;
            ;
            ViewBag.upList = null;
            ViewBag.regList = null;



            ViewBag.PageStyleType = "holy-grail-three-columns";
            //
            WebUser user = Request.IsAuthenticated ? _membershipService.GetUserByEmail(User.Identity.Name) : new WebUser();

            var usersOrders = _orderManagementService.GetOrdersByUserId(user.idUser).Where(o => o.OrderRows.First().idWebinar == id);

            var options = _orderManagementService.GetOptionsByWebinarId(id, false);
            var webinar = _db.Webinars.Include(w => w.Presenter.WebUser).First(w => w.idWebinar == id);
            ViewBag.topics = _webinarRepository.GetTopicsPerWebinar(webinar.idWebinar);


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
                CheckoutOptionsViewModel = new CheckoutOptionsViewModel
                {
                    DisplayOptionsViewModel = new DisplayOptionsViewModel
                    {
                      Options = options.ToList(),
                      Order = null,
                      Webinar = webinar,
                      WebUser = user
                    },
                    Order = null, //    new order?
                    Webinar = webinar
                }
            };

            if (usersOrders != null && usersOrders.Any())
            {
                foreach (var checkOrder in usersOrders)
                {
                    //if (checkOrder.OrderRows.Single().idWebinar == id)
                    {

                        // OrderRow has a idRegType (the FK)
                        // but does not have an instantiated RegType object.

                        var row = checkOrder.OrderRows.SingleOrDefault();
                        //if (row != null)
                        //    ViewBag.orderMessages = row.RegistrationType;
                        var webinarFiles = _db.WebinarFiles.Where(f => f.idWebinar == id).Select(f => f.fileDesc +"|"+ f.fileLocation ).ToArray();
                        ViewBag.WebinarFiles = webinarFiles;

                        ViewBag.userOwnsThisEvent = checkOrder.idOrder;
                        model.CheckoutOptionsViewModel.Order = model.CheckoutOptionsViewModel.DisplayOptionsViewModel.Order = checkOrder;

                        var connectionText = new StringBuilder("<p>");
                        connectionText.Append(row.RegistrationType.Stage2EmailConfirmationMsg.Replace(" and is also available at http://www.BankWebinars.com", "</p><p>"));
                        //connectionText.Append(ViewBag.orderMessages.Stage2EmailConfirmationMsg.Replace(" and is also available at http://www.BankWebinars.com", "</p><p>"));
                        //var conn = 

                        connectionText.Append(webinar.ConnectionInfo.Replace(Environment.NewLine,"<br>"));
                        connectionText.Append("</p>");

                        ViewBag.connectionText = connectionText;
  
                    }
                    if (checkOrder.OrderStatus == OrderStatus.InProcess
                        && checkOrder.OrderRows.Single().idWebinar != id)
                    {
                        ViewBag.userHasOpenOrder = checkOrder.idOrder;
                        model.CheckoutOptionsViewModel.Order = checkOrder;
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
            IList<Webinar> webinarsList = _webinarRepository.GetAllActive().ToList();

            var dtos = new CalendarDTOAssembler().Entities2DTOs(webinarsList);

            TempData["ListUpcoming"] = webinarsList;

            return Json(dtos, JsonRequestBehavior.AllowGet);
        }


        //
        // GET: /Webinar/Create

        public ActionResult Create()
        {
            ViewBag.PresenterId = new SelectList(_db.Presenters, "Id", "Biography");
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
                _db.Webinars.Add(webinar);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.PresenterId = new SelectList(_db.Presenters, "Id", "Biography", webinar.idPresenter);
            return View(webinar);
        }

        //
        // GET: /Webinar/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Webinar webinar = _db.Webinars.Find(id);
            if (webinar == null)
            {
                return HttpNotFound();
            }
            ViewBag.PresenterId = new SelectList(_db.Presenters, "Id", "Biography", webinar.idPresenter);
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
                _db.Entry(webinar).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PresenterId = new SelectList(_db.Presenters, "Id", "Biography", webinar.idPresenter);
            return View(webinar);
        }

        //
        // GET: /Webinar/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Webinar webinar = _db.Webinars.Find(id);
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
            Webinar webinar = _db.Webinars.Find(id);
            _db.Webinars.Remove(webinar);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
