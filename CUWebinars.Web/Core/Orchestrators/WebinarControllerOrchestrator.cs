using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using CUWebinars.Business.AccountService;
using CUWebinars.Business.Models;
using CUWebinars.Business.Services;
using CUWebinars.Web.Helpers;
using CUWebinars.Web.Models;
using CUWebinars.Web.Services;
using Ninject.Extensions.Logging;

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

        public WebinarControllerOrchestrator(
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
                throw new Exception("Audio node was null"); // TODO: [dar] handle error condition. Better message? Right Exception-type?
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
                    sb.Append("       CitrixRegisterUrl   : " + webinar.CitrixRegisterUrl + "," + System.Environment.NewLine);
                    sb.Append("       WebinarKey          : " + webinar.WebinarKey + "," + System.Environment.NewLine);
                    sb.Append("       AccessPhone         : " + webinar.AccessPhone + "," + System.Environment.NewLine);
                    sb.Append("       AccessCodeAttendee  : " + webinar.AccessCodeAttendee + "," + System.Environment.NewLine);
                    sb.Append("       AccessCodePresenter : " + webinar.AccessCodePresenter + "," + System.Environment.NewLine);
                    sb.Append("       AccessCodeOrganizer : " + webinar.AccessCodeOrganizer + "}]" + System.Environment.NewLine);


                    break;

                default:
                    {
                        System.Console.WriteLine("Other number");
                        break;
                    }
            }
            return sb.ToString();

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
    }
}