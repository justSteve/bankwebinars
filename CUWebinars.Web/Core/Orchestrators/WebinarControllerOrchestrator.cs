using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using CUWebinars.Business.AccountService;
using CUWebinars.Business.Constants;
using CUWebinars.Business.Core;
using CUWebinars.Business.Core.Helpers;
using CUWebinars.Business.Models;
using CUWebinars.Business.Services;
using CUWebinars.Web.Helpers;
using CUWebinars.Web.Infrastructure;
using CUWebinars.Web.Mapping.Mappers;
using CUWebinars.Web.Models;
using CUWebinars.Web.Services;
using CUWebinars.Web.ViewModel;
using Microsoft.VisualBasic.ApplicationServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Ninject.Extensions.Logging;
using WebGrease.Css.Extensions;
using ClaimTypes = CUWebinars.Business.Constants.ClaimTypes;

namespace CUWebinars.Web.Core.Orchestrators
{
    public class WebinarControllerOrchestrator : IWebinarControllerOrchestrator
    {
        private GlobalConfig _globalConfig = GlobalConfig.GlobalConfigSingletonCreator.UniqueInstance;

        private readonly IMembershipService _membershipService;
        private readonly IOrderManagementService _orderManagementService;
        private readonly IWebinarManagementService _webinarManagementService;
        private readonly ILogger _logger;
        private readonly IStateService _stateService;
        private readonly IAppHelper _appHelper;
        private readonly IUniversalMapper _universalMapper;
        private bool _disposed;

        public WebinarControllerOrchestrator(
            IMembershipService membershipService,
            IOrderManagementService orderManagementService,
            IWebinarManagementService webinarManagementService,
            ILogger logger,
            IStateService stateService,
            IAppHelper appHelper,
            IUniversalMapper universalMapper)
        {
            _membershipService = membershipService;
            _orderManagementService = orderManagementService;
            _webinarManagementService = webinarManagementService;
            _logger = logger;
            _stateService = stateService;
            _appHelper = appHelper;
            _universalMapper = universalMapper;
        }

        public void FireSendConnectionInfoNotificationEvent(int idWebinar)
        {
            var orders = _orderManagementService.GetOrdersForLiveNotifications(idWebinar);

            orders.ToList().ForEach((order) =>
            {
                var notificationStorage = new NotificationStorage
                {
                    idOrder = order.idOrder,
                    SessionStartInfo = _appHelper.GetSessionStartInfo()
                };

                order.NotificationStorage = JsonConvert.SerializeObject(notificationStorage);
            });

            _orderManagementService.FireSendConnectionInfoNotificationEvent(orders, false);
        }

        public Webinar GetWebinar(int idWebinar)
        {
            return _webinarManagementService.GetWebinar(idWebinar);
        }

        public ActionResult Identify(IdentifyModel identifyModel)
        {

            var idOrder = Convert.ToInt32(identifyModel.OnDemandCode.Substring(0, 5));
            // do something with name and email address
            var order = _orderManagementService.GetOrderById(idOrder);

            if (!ReferenceEquals(null, order))
            {
                var newJson = new JProperty(string.Concat("PostEventAccessByAnonUser-", DateTime.Now.ToString(DomainConstants.DateTimeLongFormat)),
                    new JObject(
                        new JProperty("Name", identifyModel.FullName),
                        new JProperty("Email", identifyModel.Email)
                        ));

                order.AdminComments = JsonHelpers.MergeJsonWithStoredField(order.AdminComments, newJson);

                _orderManagementService.SaveChanges();

                _stateService.SetValue(WebUiConstants.AnonUserIdentified, true);

                return new RedirectToRouteResult(new RouteValueDictionary(new { action = "OnDemand", controller = "Webinar", onDemandCode = identifyModel.OnDemandCode }));
            }

            return new ViewResult { ViewName = "Identify", ViewData = { Model = identifyModel } };
        }

        public ActionResult OnDemand(int id, string onDemandCode, IIdentity userIdentity)
        {
            var claimsIdentityOfAuthenticatedUser = (ClaimsIdentity)userIdentity;

            var order = _orderManagementService.GetOrderById(id);

            var webinar =
                _webinarManagementService.GetWebinar(order.OrderRows.SingleOrDefault(s => s.RowStatus == OrderRowStatus.Active).idWebinar);

            var playModel = new OnDemandPlaybackModel
            {
                Presenter = webinar.Presenter,
                Webinar = webinar,
                idOrder = order.idOrder,
                OnDemandCode = onDemandCode
            };

            var viewResult = new ViewResult { ViewName = "OnDemand" };
            viewResult.ViewData.Model = playModel;


            if (claimsIdentityOfAuthenticatedUser.IsAuthenticated)
            {
                //permits admins to preview onDemand pages
                //if (claimsIdentityOfAuthenticatedUser.HasClaim((claim) => claim.Type == ClaimTypes.Admin))
                //{
                //    return new RedirectToRouteResult(new RouteValueDictionary(new { action = "Index", controller = "Admin" }));
                //}

                ProcessAccessPermissionsForMaterials(order, playModel);
                
                return viewResult;
            }

            if (_stateService.HasValue(WebUiConstants.AnonUserIdentified) && _stateService.GetValue<bool>(WebUiConstants.AnonUserIdentified))
            {
                ProcessAccessPermissionsForMaterials(order, playModel);

                return viewResult;
            }

            return new RedirectToRouteResult(new RouteValueDictionary(new { action = "Identify", controller = "Webinar", onDemandCode = id.ToString() + "-" + onDemandCode }));
        }

        private string ProcessAccessPermissionsForMaterials(Order order, OnDemandPlaybackModel playModel)
        {
            var webUser = _membershipService.GetWebUserById(order.idUser);

            if (webUser != null)
            {

                var myClaim = _membershipService.GetDisplayPostEventMaterialsClaimValue(
                    _globalConfig.Tenant,
                    webUser.email, playModel.idOrder, playModel.OnDemandCode);

                if (myClaim != null)
                {
                    playModel.AuthorizedToAccessMaterials = true;
                    //here's a chance for a handy bit of scripting

                    _logger.Info("AuthorizedToAccessMaterials granted to: " + webUser.email);

                    return myClaim;
                }
                playModel.AuthorizedToAccessMaterials = false;
                _logger.Warn("AuthorizedToAccessMaterials denied to: " + webUser.email);

                return "Not Authorized";
            }

            _logger.Error("No WebUser exists with the Id {0}", order.idUser);

            return "User Not Found";
            //ModelState.AddModelError(string.Empty,
            //    string.Format("No WebUser exists with the Id {0}", order.idUser));
        }

        private void AddJsonCommentToUser(Order order, string s)
        {
            var a = 1;
            //throw new NotImplementedException();
        }

        public string OpenMeeting(string joinCode, IIdentity userIdentity)
        {
            string webinarUrl = string.Empty;

            if (_stateService.HasValue(WebUiConstants.WebinarFromCode))
            {
                Webinar webinar = _stateService.GetValue<Webinar>(WebUiConstants.WebinarFromCode);
                _stateService.ClearValue(WebUiConstants.WebinarFromCode);

                var orderRow = webinar.OrderRows.SingleOrDefault(or => or.TtsJoinUrl == joinCode);

                if (!ReferenceEquals(null, orderRow))
                {

                    if (userIdentity.IsAuthenticated && !ReferenceEquals(null, orderRow.CitrixJoinUrl)) // CitrixJoinUrl will be null for impromtu user
                    {
                        webinarUrl = orderRow.CitrixJoinUrl; // fully qualified authorative webinar-access link from Citrix.
                    }
                    else
                    {
                        // non-individualized version of webinar-access link from Citrix.
                        webinarUrl = webinar.CitrixRegisterUrl;
                    }
                }
            }
            return webinarUrl;
        }

        public Webinar PopulateWebinarFromViewModel(ConnectionInfoEditModel connectionInfoModel, out bool detailsValid)
        {
            //using (var client = new System.Net.WebClient())
            //{
            //TODO: Figure out how to authenticate and then pass URL

            //var filename = System.IO.Path.GetTempFileName();

            //client.DownloadFile(model.ManageURL, filename);

            var doc = new HtmlAgilityPack.HtmlDocument();
            //doc.Load(filename);
            doc.LoadHtml(connectionInfoModel.ManageURL);

            var root = doc.DocumentNode;
            var audioNode = root.SelectNodes("//*[text()[contains(., 'Access')]]");

            var citrixRegisterUrl = "";
            var accessCodeAttendee = "";
            var accessCodePresenter = "";
            var accessCodeOrganizer = "";
            var webinarKey = "";
            var accessPhone = "";

            webinarKey = root.SelectSingleNode("//span[contains(@id,'WebinarInfoID')]").InnerHtml;
            webinarKey = webinarKey.Replace("-", "").Trim();
            accessPhone = root.SelectSingleNode("//*[text()[contains(., 'Toll-')]]").InnerHtml;
            accessPhone = Regex.Split(accessPhone, @"\<br\>")[1].Replace("Toll-free: 1 ", "").Trim().Replace(" ", "-");


            citrixRegisterUrl = root.SelectSingleNode("//a[contains(@id,'registrationURL')]").InnerHtml;

            if (ReferenceEquals(audioNode, null))
            {
                throw new Exception("Audio node was null");
                // TODO: [dar] handle error condition. Better message? Right Exception-type?
            }

            foreach (var aNode in audioNode.Nodes())
            {
                if (aNode.InnerHtml.Contains("Panelist"))
                {
                    accessCodePresenter = aNode.ParentNode.OuterHtml.Split(':')[1].Trim().Split(' ')[0];
                }
                if (aNode.InnerHtml.Contains("Organizer"))
                {
                    accessCodeOrganizer = aNode.ParentNode.OuterHtml.Split(':')[1].Trim().Split(' ')[0];
                }

                if (aNode.InnerHtml.Contains("Attendee"))
                {
                    accessCodeAttendee = aNode.ParentNode.OuterHtml.Split(':')[1].Trim().Split(' ')[0];
                }
            }

            var webinar = _webinarManagementService.GetWebinar(connectionInfoModel.idWebinar);
            webinar.AccessCodeAttendee = accessCodeAttendee;
            webinar.AccessCodeOrganizer = accessCodeOrganizer;
            webinar.AccessCodePresenter = accessCodePresenter;
            webinar.AccessPhone = accessPhone;
            webinar.CitrixRegisterUrl = citrixRegisterUrl;
            webinar.OrganizerOAuthKey = connectionInfoModel.OrganizerOAuthKey;
            webinar.OrganizerKey = connectionInfoModel.OrganizerKey;
            webinar.WebinarKey = webinarKey.Trim();


            if (webinar.CitrixRegisterUrl == "" ||
                webinar.AccessCodeAttendee == "" ||
                webinar.AccessCodePresenter == "" ||
                webinar.AccessCodeOrganizer == "" ||
                webinar.WebinarKey == "" ||
                webinar.AccessPhone == "")
            {
                detailsValid = false;
            }
            else
            {

                if (webinar.WebinarFiles != null || webinar.WebinarFiles.Count < 1)
                {
                    webinar.Status = WebinarStatus.Active;

                    var changeString = PublishStateChange(webinar.Status.ToString() + " to " + "Active", webinar);

                    webinar.ConnectionInfo = changeString;
                }


                UpdateWebinar(webinar);

                detailsValid = true;
            }

            return webinar;
        }

        public string PublishStateChange(string stateFromTo, Webinar webinar)
        {
            var sb = new StringBuilder();

            switch (stateFromTo.ToLower())
            {
                case "scheduled to active":
                    sb.Append("[{STATECHANGE : scheduled to active," + System.Environment.NewLine);
                    sb.Append("       CitrixRegisterUrl   : " + webinar.CitrixRegisterUrl + "," +
                              System.Environment.NewLine);
                    sb.Append("       WebinarKey          : " + webinar.WebinarKey + "," + System.Environment.NewLine);
                    sb.Append("       AccessPhone         : " + webinar.AccessPhone + "," + System.Environment.NewLine);
                    sb.Append("       AccessCodeAttendee  : " + webinar.AccessCodeAttendee + "," +
                              System.Environment.NewLine);
                    sb.Append("       AccessCodePresenter : " + webinar.AccessCodePresenter + "," +
                              System.Environment.NewLine);
                    sb.Append("       AccessCodeOrganizer : " + webinar.AccessCodeOrganizer + "}]" +
                              System.Environment.NewLine);


                    break;

                default:
                    {
                        System.Console.WriteLine("Other number");
                        break;
                    }
            }
            return sb.ToString();

        }

        public IEnumerable<Webinar> SearchWebinars(string searchTerm)
        {
            var webinars = _webinarManagementService.GetWebinarByPresenterLastName(searchTerm);
            var webinarsByTopic = _webinarManagementService.GetWebinarByDescription(searchTerm);
            var unionOfResultSets = webinars.Union(webinarsByTopic);
            return unionOfResultSets;
        }

        public string UpdateWebinarRecording(WebinarDetailsViewModel webinarDetailsViewModel, out string message)
        {
            int webinarId = 0;
            message = string.Empty;

            try
            {
                Webinar webinar = _webinarManagementService.GetWebinar(webinarDetailsViewModel.Webinar.idWebinar);
                webinarId = webinar.idWebinar;

                var checkThatNewFilesExist = CheckThatFileExists(webinarDetailsViewModel.Webinar.RecordingUrl);

                if (checkThatNewFilesExist != "OK")
                {
                    _logger.Error(string.Format(webinarDetailsViewModel.Webinar.RecordingUrl, " does not exist."));
                    return string.Format(webinarDetailsViewModel.Webinar.RecordingUrl, " does not exist.");
                }

                var ordersForWebinar = _orderManagementService.GetOrdersForWebinar(webinar.idWebinar);

                AddClaimForPostEventMaterials(ordersForWebinar);

                webinar.RecordingUrl = webinarDetailsViewModel.Webinar.RecordingUrl;

                webinar.Status = WebinarStatus.Recorded;

                _webinarManagementService.UpdateWebinar(webinar);

                _orderManagementService.FireSendRecordingIsPostedEvent(ordersForWebinar);
                _logger.Info(string.Format("Recording for {0} is saved to {1}", webinar.idWebinar, webinar.RecordingUrl));
                return WebUiConstants.Success;

            }
            catch (Exception exception)
            {
                _logger.ErrorException(string.Format("UpdateWebinarRecording| UpdateWebinarRecording failed {0} on idWebinar: {1}", exception.Message, webinarId), exception);
                return string.Format("UpdateWebinarRecording| UpdateWebinarRecording failed {0} on idWebinar: {1}",
                    exception.Message, webinarId);
            }
        }

        //public string SetEventToRecorded(int webinarId)
        //{
        //    var webinar = _webinarManagementService.GetWebinar(webinarId);
        //    if (webinar.RecordingUrl != null)
        //    {
        //        String setPostEventClaims = _orderManagementService.SetPostEventClaims(webinarId);
        //        webinar.Status = WebinarStatus.Recorded;

        //        var changeString = PublishStateChange(webinar.Status.ToString() + " to " + "recorded", webinar);

        //        webinar.ConnectionInfo = changeString;
        //    }
        //    return null;
        //}

        public void UpdateWebinar(Webinar webinar)
        {
            _webinarManagementService.UpdateWebinar(webinar);
        }

        public string UpdateWebinarFiles(WebinarFilesEditModel webinarFilesEditModel, out string message)
        {
            /*
                This is a batch update operation. The WebinarFiles collection contains the webinar files to be updated. 
                The three possibilities are updated, deleted and created. (Even if a file was unchanged at the client, it will be treated as updated.)
                    * If the idWebinar in the WebinarFiles is a positive number and does not end with -D, it is updated
                    * If the idWebinar in the WebinarFiles is 0, and does not end with -ND, it has been created (suffix of 'N' appended at client deserializes to 0)
                    * If the fileDesc in the WebinarFile ends with -D, it has been marked for deletion.             
             */
            message = string.Empty;

            try
            {

                var filesForThisEvent = _webinarManagementService.GetWebinarFilesPerWebinar(webinarFilesEditModel.idWebinar);

                var deletedFiles = webinarFilesEditModel.WebinarFiles.Where(f => f.fileDesc.EndsWith("-D")).ToList();
                var newFiles = webinarFilesEditModel.WebinarFiles
                    .Where(f => f.idWebinarFile == 0 && !f.fileDesc.EndsWith("-ND") && f.fileLocation != filesForThisEvent.Select(wf => wf.fileLocation).ToString()).ToList();
                var updatedFiles = webinarFilesEditModel.WebinarFiles.Where(f => f.idWebinarFile > 0 && !f.fileDesc.EndsWith("-D")).ToList();

                foreach (var webinarFile in newFiles)
                {
                    webinarFile.fileLocation = Regex.Replace(webinarFile.fileLocation, @"\s+", "");
                    var checkThatNewFilesExist = CheckThatFileExists(webinarFile.fileLocation.ToString());
                    if (checkThatNewFilesExist != "OK")
                    {
                        _logger.Error(string.Format("{0} does not exist.", webinarFile.fileLocation));
                        return string.Format("{0} does not exist.", webinarFile.fileLocation.ToString());
                    }

                }

                _webinarManagementService.AddWebinarFiles(newFiles);
                _webinarManagementService.DeleteWebinarFiles(deletedFiles);
                _webinarManagementService.UpdateWebinarFiles(updatedFiles);

                return WebUiConstants.Success;
            }
            catch (Exception exception)
            {
                _logger.ErrorException(
                    string.Format("UpdateWebinarFiles| UpdateWebinarFiles failed {0}", exception.Message), exception);
                return string.Format("UpdateWebinarFiles| UpdateWebinarFiles failed {0}", exception.Message);

            }

        }

        public AdhocNotificationViewModel BuildAdhocNotificationViewModel(WebinarType webinarType)
        {
            switch (webinarType)
            {
                case WebinarType.Recorded:
                    return new AdhocNotificationViewModel
                    {
                        Webinars = EventInvokerHelpers.GetRecordedWebinarsAsSelectListItems(_webinarManagementService)
                    };

                case WebinarType.Upcoming:
                    return new AdhocNotificationViewModel
                    {
                        Webinars = EventInvokerHelpers.GetUpcomingWebinarsAsSelectListItems(_webinarManagementService)
                    };
                default: throw new NotSupportedException(string.Format("{0} is not a valid WebinarType", webinarType));
            }
        }

        public WebinarEditModel BuildEditModelForWebinarCreate()
        {
            var upcomingRegTypeGroups = _webinarManagementService.GetUpcomingRegTypesForWebinars().ToList();
            var presenters = _webinarManagementService.GetAllPresenters()
                .Select(presenter =>
                    new SelectListItem { Text = presenter.WebUser.FullName, Value = presenter.idUser.ToString() }
                );
            var statuses = (from object value in Enum.GetValues(typeof(WebinarStatus))
                            select new SelectListItem { Text = value.ToString(), Value = ((int)value).ToString() }).ToList();


            var webinarEditModel = new WebinarEditModel
            {
                Presenters = presenters,
                RegTypeGroups = upcomingRegTypeGroups,
                Statuses = statuses,
                Topics = _webinarManagementService.GetAllTopics()
            };

            return webinarEditModel;
        }

        public WebinarEditModel BuildEditModelForWebinar(int idWebinar)
        {
            var webinar = _webinarManagementService.GetWebinar(idWebinar);

            var topicIdsForWebinar = _webinarManagementService.GetTopicsPerWebinar(idWebinar).ToList();
            var upcomingRegTypeGroups = _webinarManagementService.GetUpcomingRegTypesForWebinars().ToList();
            var regTypeGroupsForWebinars = _webinarManagementService.GetRegTypeGroupsForWebinars(idWebinar).ToList();
            var presenters = _webinarManagementService.GetAllPresenters()
                .Select(presenter => new SelectListItem
                {
                    Text = presenter.WebUser.FullName,
                    Value = presenter.idUser.ToString()
                });
            var topics = _webinarManagementService.GetAllTopics();

            var statuses = (from object value in Enum.GetValues(typeof(WebinarStatus))
                            select new SelectListItem { Text = value.ToString(), Value = ((int)value).ToString() }).ToList();

            // The database does not support the business rule that there will only be a single price for Additional Locations.
            // We ae therefore enforcing this rule here.
            var additionalLocationsPricing =
                _webinarManagementService.GetAdditionalLocationsLookupPricesForWebinar(idWebinar).SingleOrDefault();

            var webinarEditModel = new WebinarEditModel
            {
                AdditionalLocationsPrice = additionalLocationsPricing == null ? 0.00M : additionalLocationsPricing.Cost,
                PostedTopics = new PostedTopics { TopicIds = topicIdsForWebinar.Select(topic => topic.idTopic).ToArray() },
                PostedRegTypeGroups =
                    new PostedRegTypeGroups
                    {
                        RegTypeGroupIds = regTypeGroupsForWebinars.Select(r => r.idRegTypeGroup).ToArray()
                    },
                Presenters = presenters,
                RegTypeGroups = upcomingRegTypeGroups,
                SelectedPresenter = webinar.idPresenter,
                SelectedRegTypeGroups = regTypeGroupsForWebinars,
                SelectedTopics = topicIdsForWebinar,
                SelectedStatus = (int)webinar.Status,
                Statuses = statuses,
                Topics = topics
            };

            // copy across remaining properties not dealt with above.
            webinarEditModel = _universalMapper.Map(webinar, webinarEditModel);

            return webinarEditModel;
        }

        public void CreateWebinarFromViewInput(WebinarEditModel webinarEditModel)
        {
            if (ReferenceEquals(webinarEditModel.PostedRegTypeGroups, null))
                webinarEditModel.PostedRegTypeGroups = new PostedRegTypeGroups { RegTypeGroupIds = new int[0] };
            if (ReferenceEquals(webinarEditModel.PostedTopics, null))
                webinarEditModel.PostedTopics = new PostedTopics { TopicIds = new int[0] };

            var webinar = new Webinar
            {
                Title = webinarEditModel.Title,
                idPresenter = webinarEditModel.SelectedPresenter,
                Date = webinarEditModel.Date,
                DateChanged = TtsConfig.UtcNowAsCts,
                DateCreated = TtsConfig.UtcNowAsCts,
                Status = (WebinarStatus)webinarEditModel.SelectedStatus,
                Duration = webinarEditModel.Duration,
                ceu = webinarEditModel.ceu,
                Description = webinarEditModel.Description,
                DescriptionLong = webinarEditModel.DescriptionLong,
                LearnBody = webinarEditModel.LearnBody,
                LearnCaption = webinarEditModel.LearnCaption,
                ImageUrl = webinarEditModel.ImageUrl,
                RecordingUrl = webinarEditModel.RecordingUrl,
                RegTypesGroupsXref = new List<RegTypesGroupsXref>(),
                SmallImageUrl = webinarEditModel.SmallImageUrl,
                WebinarTopicXrefs = new List<WebinarTopicXref>(),
                WhoAttend = webinarEditModel.WhoAttend
            };


            foreach (var regTypeGroupId in webinarEditModel.PostedRegTypeGroups.RegTypeGroupIds)
            {
                webinar.RegTypesGroupsXref.Add(new RegTypesGroupsXref { idRegTypeGroup = regTypeGroupId });
            }

            foreach (var topicId in webinarEditModel.PostedTopics.TopicIds)
            {
                webinar.WebinarTopicXrefs.Add(new WebinarTopicXref { idTopic = topicId });
            }

            _webinarManagementService.AddWebinar(webinar);

            var additionalLocationsLookupPrice = _webinarManagementService.GetAdditionalLocationsLookupPricesForWebinar(webinar.idWebinar).SingleOrDefault();

            if (ReferenceEquals(null, additionalLocationsLookupPrice))
            {
                additionalLocationsLookupPrice = new AdditionalLocationsLookupPrice
                {
                    Cost = webinarEditModel.AdditionalLocationsPrice,
                    idWebinar = webinar.idWebinar
                };

                _webinarManagementService.AddAdditionalLocationsLookupPrice(additionalLocationsLookupPrice);
            }
            else
            {
                additionalLocationsLookupPrice.Cost = webinarEditModel.AdditionalLocationsPrice;
            }

            _webinarManagementService.AddAdditionalLocationsLookupPrice(additionalLocationsLookupPrice);
            _webinarManagementService.SaveChanges();
        }

        public void UpdateWebinarFromViewInput(WebinarEditModel webinarEditModel)
        {
            var webinar = _webinarManagementService.GetWebinar(webinarEditModel.idWebinar);

            if (ReferenceEquals(webinarEditModel.PostedRegTypeGroups, null))
                webinarEditModel.PostedRegTypeGroups = new PostedRegTypeGroups { RegTypeGroupIds = new int[0] };
            if (ReferenceEquals(webinarEditModel.PostedTopics, null))
                webinarEditModel.PostedTopics = new PostedTopics { TopicIds = new int[0] };

            webinar.Title = webinarEditModel.Title;
            webinar.idPresenter = webinarEditModel.SelectedPresenter;
            webinar.Date = webinarEditModel.Date;
            webinar.Status = (WebinarStatus)webinarEditModel.SelectedStatus;
            webinar.Duration = webinarEditModel.Duration;
            webinar.ceu = webinarEditModel.ceu;
            webinar.Description = webinarEditModel.Description;
            webinar.DescriptionLong = webinarEditModel.DescriptionLong;
            webinar.LearnBody = webinarEditModel.LearnBody;
            webinar.LearnCaption = webinarEditModel.LearnCaption;
            webinar.ImageUrl = webinarEditModel.ImageUrl;
            webinar.SmallImageUrl = webinarEditModel.SmallImageUrl;
            webinar.WhoAttend = webinarEditModel.WhoAttend;
            webinar.RecordingUrl = webinarEditModel.RecordingUrl;

            var existingRegTypeGroupIds =
                webinar.RegTypesGroupsXref.Where(r => r.idWebinar == webinar.idWebinar)
                    .Select(r => r.idRegTypeGroup)
                    .ToArray();

            // This loop add any new RegTypeGroupXrefs which exist in the incoming model, but not in the collection
            // retrieved from the database i.e. new RegTypeGroupXrefs
            foreach (
                var regTypeGroupId in
                    webinarEditModel.PostedRegTypeGroups.RegTypeGroupIds.Where(
                        regTypeGroupId => !existingRegTypeGroupIds.Contains(regTypeGroupId)))
            {
                webinar.RegTypesGroupsXref.Add(new RegTypesGroupsXref
                {
                    idWebinar = webinar.idWebinar,
                    idRegTypeGroup = regTypeGroupId
                });
            }

            foreach (
                var existingRegTypeGroupId in
                    existingRegTypeGroupIds.Where(
                        existingRegTypeGroupId =>
                            !webinarEditModel.PostedRegTypeGroups.RegTypeGroupIds.Contains(existingRegTypeGroupId)))
            {
                _webinarManagementService.DeleteRegTypeGroupXRef(webinar, existingRegTypeGroupId);
            }

            var existingTopicIds =
                webinar.WebinarTopicXrefs.Where(r => r.idWebinar == webinar.idWebinar).Select(r => r.idTopic).ToArray();

            foreach (
                var topicId in
                    webinarEditModel.PostedTopics.TopicIds.Where(topicId => !existingTopicIds.Contains(topicId)))
            {
                webinar.WebinarTopicXrefs.Add(new WebinarTopicXref { idWebinar = webinar.idWebinar, idTopic = topicId });
            }

            foreach (
                var exisingTopicId in
                    existingTopicIds.Where(
                        exisingTopicId => !webinarEditModel.PostedTopics.TopicIds.Contains(exisingTopicId)))
            {
                _webinarManagementService.DeleteWebinarTopicXref(webinar, exisingTopicId);
            }

            var additionalLocationsLookupPrice = _webinarManagementService.GetAdditionalLocationsLookupPricesForWebinar(webinar.idWebinar).SingleOrDefault();

            if (ReferenceEquals(null, additionalLocationsLookupPrice))
            {
                _webinarManagementService.AddAdditionalLocationsLookupPrice(new AdditionalLocationsLookupPrice
                {
                    Cost = webinarEditModel.AdditionalLocationsPrice,
                    idWebinar = webinar.idWebinar
                });
            }
            else
            {
                additionalLocationsLookupPrice.Cost = webinarEditModel.AdditionalLocationsPrice;
            }

            _webinarManagementService.UpdateWebinar(webinar);
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
                    var onDemandCode = RandomHelpers.GetUniqueCode(5);

                    order.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active).OnDemandCode = onDemandCode;

                    _orderManagementService.SaveChanges();

                    var orderIdProperty = new JProperty(JsonPropertyKeys.OrderId, order.idOrder);
                    var expiryDateProperty = new JProperty(JsonPropertyKeys.ExpiryDate, expiryDate.ToString(DomainConstants.ClaimDateFormatText));
                    var OnDemandCodeProperty = new JProperty(JsonPropertyKeys.OnDemandCode, onDemandCode);

                    var claimValue = new JObject(
                        orderIdProperty,
                        expiryDateProperty,
                        OnDemandCodeProperty
                        );

                    _membershipService.AddClaim(
                        userAccountOfOrderer, ClaimTypes.DisplayPostEventMaterials, claimValue.ToString(Formatting.None)
                        );

                    _logger.Info("Claim for onDemand access added for " + order.idOrder + "-" + onDemandCode);

                }
                catch (Exception)
                {
                    _logger.Error(string.Format("AddClaimForPostEventMaterials| MR record not found {0}", order.BillingEmail));
                }
            }
        }


        private string CheckThatFileExists(string newFile)
        {
            var handoutRepo = "http://ttsmedia.ttstrain.com/";

            HttpWebResponse response = null;
            string uri = handoutRepo + newFile;
            var request = (HttpWebRequest)WebRequest.Create(uri);
            request.Method = "HEAD";
            try
            {
                response = (HttpWebResponse)request.GetResponse();
                return response.StatusCode.ToString();
            }
            catch (WebException ex)
            {
                return string.Format("File Requested could not be found: {0}", ex.Message);
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }
            }
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

                _membershipService.Dispose();
                _orderManagementService.Dispose();
                _webinarManagementService.Dispose();

                _disposed = true;
            }
        }
    }
}