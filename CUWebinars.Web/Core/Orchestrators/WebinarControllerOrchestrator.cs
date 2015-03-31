using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using CUWebinars.Business.AccountService;
using CUWebinars.Business.Models;
using CUWebinars.Business.Services;
using CUWebinars.Web.Helpers;
using CUWebinars.Web.Mapping.Mappers;
using CUWebinars.Web.Models;
using CUWebinars.Web.Services;
using CUWebinars.Web.ViewModel;
using Newtonsoft.Json;
using Ninject.Extensions.Logging;
using WebGrease.Css.Extensions;

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

        public string SetEventToRecorded(int webinarId)
        {
            var webinar = _webinarManagementService.GetWebinar(webinarId);
            if (webinar.RecordingUrl != null)
            {
                String setPostEventClaims = _orderManagementService.SetPostEventClaims(webinarId);
                webinar.Status = WebinarStatus.Recorded;

                var changeString = PublishStateChange(webinar.Status.ToString() + " to " + "recorded", webinar);

                webinar.ConnectionInfo = changeString;
            }
            return null;
        }

        public void UpdateWebinar(Webinar webinar)
        {
            _webinarManagementService.UpdateWebinar(webinar);
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
                    new SelectListItem {Text = presenter.WebUser.FullName, Value = presenter.idUser.ToString()}
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

            var statuses = (from object value in Enum.GetValues(typeof (WebinarStatus))
                select new SelectListItem {Text = value.ToString(), Value = ((int) value).ToString()}).ToList();

            // The database does not support the business rule that there will only be a single price for Additional Locations.
            // We ae therefore enforcing this rule here.
            var additionalLocationsPricing =
                _webinarManagementService.GetAdditionalLocationsLookupPricesForWebinar(idWebinar).SingleOrDefault();

            var webinarEditModel = new WebinarEditModel
            {
                AdditionalLocationsPrice = additionalLocationsPricing == null ? 0.00M : additionalLocationsPricing.Cost,
                PostedTopics = new PostedTopics {TopicIds = topicIdsForWebinar.Select(topic => topic.idTopic).ToArray()},
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
                SelectedStatus = (int) webinar.Status,
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
                webinarEditModel.PostedRegTypeGroups = new PostedRegTypeGroups {RegTypeGroupIds = new int[0]};
            if (ReferenceEquals(webinarEditModel.PostedTopics, null))
                webinarEditModel.PostedTopics = new PostedTopics {TopicIds = new int[0]};

            var webinar = new Webinar
            {
                Title = webinarEditModel.Title,
                idPresenter = webinarEditModel.SelectedPresenter,
                Date = webinarEditModel.Date,
                DateChanged = DateTime.Now,
                DateCreated = DateTime.Now,
                Status = (WebinarStatus) webinarEditModel.SelectedStatus,
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
                webinar.RegTypesGroupsXref.Add(new RegTypesGroupsXref {idRegTypeGroup = regTypeGroupId});
            }

            foreach (var topicId in webinarEditModel.PostedTopics.TopicIds)
            {
                webinar.WebinarTopicXrefs.Add(new WebinarTopicXref {idTopic = topicId});
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
                webinarEditModel.PostedRegTypeGroups = new PostedRegTypeGroups {RegTypeGroupIds = new int[0]};
            if (ReferenceEquals(webinarEditModel.PostedTopics, null))
                webinarEditModel.PostedTopics = new PostedTopics {TopicIds = new int[0]};

            webinar.Title = webinarEditModel.Title;
            webinar.idPresenter = webinarEditModel.SelectedPresenter;
            webinar.Date = webinarEditModel.Date;
            webinar.Status = (WebinarStatus) webinarEditModel.SelectedStatus;
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
                webinar.WebinarTopicXrefs.Add(new WebinarTopicXref {idWebinar = webinar.idWebinar, idTopic = topicId});
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