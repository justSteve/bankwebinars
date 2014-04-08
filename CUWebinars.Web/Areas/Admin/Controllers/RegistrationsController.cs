using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CUWebinars.Business.AccountService;
using CUWebinars.Business.Models;
using CUWebinars.Business.Repository;
using CUWebinars.Business.Services;
using CUWebinars.Web.Services;
using Ninject.Extensions.Logging;
using Ninject.Infrastructure.Language;

namespace CUWebinars.Web.Areas.Admin.Controllers
{
    public class RegistrationsController : Controller
    {
        private TTSWebinarsContext db = new TTSWebinarsContext();
        private IMailService _mail;

        //Steve added MembershipService dependancy to allow for 'currentUser' in Details.
        public IMembershipService membershipService;
        private readonly IWebinarRepository _webinarRepository;
        private readonly IOrderManagementService _orderManagementService;
        public ILogger Logger { get; set; }

        public RegistrationsController(MembershipService membershipService, IMailService mail, IWebinarRepository webinarRepository, ILogger logger, IOrderManagementService orderManagementService)
        {
            this.membershipService = membershipService;
            _mail = mail;
            _webinarRepository = webinarRepository;
            Logger = logger;
            _orderManagementService = orderManagementService;
        }
        //
        // GET: /Admin/Registrations/
        public ActionResult Index()
        {
            return View();
        }


        //[Authorize(Roles = AppRoles.CustomerAffiliateAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult SetAdditionalLocation(FormCollection formValues)
        {
            var ID = Convert.ToInt32(formValues["ID"]);
            var connectionsCount = Convert.ToInt32(formValues["connectionsCount"]);
            var msg = "";
            var originalCost = _orderManagementService.LoadOrderRow(ID).Order.Total;
            int originalLocCount = 0;
            OrderRow row = _orderManagementService.LoadOrderRow(ID);
            
            //AdditionalLocation RegType in row.AdditionalLocation
            //var RegType = row.AdditionalLocation.SingleOrDefault(o => o.Type == "additional_location");

            //var locations =
            //    row.AdditionalLocation.SingleOrDefault(o => o.Type == "additional_location");
            //Options.OfType<AdditionalLocationOrderRowOption>().SingleOrDefault();

            IDictionary<string, string> addEmails = Request.Params.AllKeys
                .Where(x => x.StartsWith("Email"))
                .Where(x => Request.Params[x] != null && Request.Params[x].ToString().Length > 0)
                .Select(x => new {key = x, value = Request.Params[x]})
                .ToDictionary(x => (x.key), x => (x.value));

            connectionsCount = addEmails.Count;

            var emails = addEmails
                .Select(e => e.Value)
                .ToList();

            //var AdditionalLocation = new List<AdditionalEmails>(emails.Count);

            //emails.ForEach(email =>
            //{
            //    var additionalLocation = new AdditionalEmails {Email = email};
            //    AdditionalLocation.Add(additionalLocation);
            //});


            //if (connectionsCount <= 0 && locations != null)
            //{
            //    row.AdditionalLocation.Remove(locations);
            //    msg = "There are no Additional Locations specified.";
            //}

            //if (connectionsCount > 0 && locations == null)
            //{
            //    var options = _orderManagementService.GetOptionsByWebinarId(row.Webinar.idWebinar, false);
            //    var option = options.SingleOrDefault(o => o.Type == "additional_location");
            //    //                AdditionalLocationOption RegType = options.OfType<AdditionalLocationOption>().SingleOrDefault();



            //    if (option != null)
            //    {
            //        locations = new AdditionalLocation
            //        {
            //            //AdditionalLocation = AdditionalLocation,
            //            RegType = option,
            //            OrderRow = row,
            //            OptionDescription = option.OptionExplain,
            //            RegTypePrice = Convert.ToDecimal(option.Price)
            //        };
            //        row.AdditionalLocation.Add(locations);
            //    }
            //}
            //else
            //{
            //    if (locations != null)
            //    {
            //        //originalLocCount = locations.AdditionalLocation.Count;
            //        ////locations.additional_locations_count = connectionsCount;
            //        //locations.AdditionalLocation = AdditionalLocation;
            //    }
            //}

            var optionsCost = string.Empty;
            //if (connectionsCount == 1)
            //{
            //    msg = "One Additional Location.<br>";

            //    optionsCost = "Total cost of Additional Locations: " +
            //                  (locations.RegTypePrice*connectionsCount).ToString("C0");

            //}
            //if (connectionsCount > 1)
            //{
            //    msg = "This order carries " + connectionsCount + " Additional Locations.";
            //}

            try
            {
                //_orderManagementService.Save(row.Order);
            }
            catch (Exception)
            {

                throw;
            }

            if (originalCost != row.Order.Total && row.RowStatus != OrderRowStatus.InProcess)
            {
                try
                {
                    var BuildChangedOrderRow = new Dictionary<string, string>
                    {
                        {"Date", DateTime.Now.ToShortDateString()},
                        {"Order", row.Order.idOrder.ToString()},
                        {"Individual", row.Order.FirstName + ' ' + row.Order.LastName},
                        {
                            "Additional Locations Changed",
                            originalLocCount + " to " + row.RegistrationType.ToString()
                        },
                        {"OriginalCost", originalCost.ToString()},
                        {"UpdatedCost", row.Order.Total.ToString()},
                        {"Affiliate", row.Order.Affiliate.ttsDomain},
                        {"Billed", "N"},
                        {"Difference", (originalCost - row.Order.Total).ToString("C")},
                        {"ChangedBy", ""}
                        //TODO: How to get CurrentUser?
                        //UserFacade.Instance.GetCurrentUser().FullName}
                    };
                    //TTSTrain.Webinars.Business.RssBusService.Instance.AddChangedOrder(BuildChangedOrderRow);
                }
                catch (Exception ex)
                {
                    Logger.Error("ERROR AddLocations Op on ID: " + ID + " " + ex);
                }
            }
            return Json(new
            {
                numLocations = connectionsCount,
                msg = msg,
                Success = true,
                optionsCost
            }, JsonRequestBehavior.AllowGet);
        }
    }
}