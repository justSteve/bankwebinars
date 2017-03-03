using Citrix.GoToWebinar.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Citrix.GoToWebinar.Api.Model;
using CUWebinars.Business.AccountService;
using CUWebinars.Business.Constants;

using CUWebinars.Business.Core;
using CUWebinars.Business.Core.Helpers;
using CUWebinars.Business.Models;
using CUWebinars.Business.Services;
using CUWebinars.Business.Validation.Json;
using CUWebinars.Web.Helpers;
using CUWebinars.Web.Infrastructure;
using CUWebinars.Web.Mapping.Mappers;
using CUWebinars.Web.Models;
using CUWebinars.Web.Models.JsonModels;
using CUWebinars.Web.Services;
using CUWebinars.Web.ViewModel;
using FluentValidation;
using GemBox.Document;
using GemBox.Document.MailMerging;
using Glimpse.AspNet.Tab;
using Microsoft.VisualBasic.ApplicationServices;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Ninject.Extensions.Logging;
using WebGrease.Css.Extensions;
using ClaimTypes = CUWebinars.Business.Constants.ClaimTypes;
using PostEventClaim = CUWebinars.Business.Services.PostEventClaim;
using Webinar = CUWebinars.Business.Models.Webinar;

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
        private readonly IValidator<ValidationString> _jsonValidator;
        private bool _disposed;

        public WebinarControllerOrchestrator(
            IMembershipService membershipService,
            IOrderManagementService orderManagementService,
            IWebinarManagementService webinarManagementService,
            ILogger logger,
            IStateService stateService,
            IAppHelper appHelper,
            IUniversalMapper universalMapper,
            IValidator<ValidationString> jsonValidator)
        {
            _membershipService = membershipService;
            _orderManagementService = orderManagementService;
            _webinarManagementService = webinarManagementService;
            _logger = logger;
            _stateService = stateService;
            _appHelper = appHelper;
            _universalMapper = universalMapper;
            _jsonValidator = jsonValidator;
        }

        public void FireSendConnectionInfoNotificationEvent(int idWebinar)
        {
            var webinar = _webinarManagementService.GetWebinar(idWebinar);
            var orgKey = _globalConfig.ConvertToCitrixOrgKey(webinar.OrganizerKey);
            var cWebinarKey = _globalConfig.ConvertToCitrixWebinarKey(webinar.WebinarKey);
            var api = new RegistrantsApi();

            var apiResponse = api.getAllRegistrantsForWebinar(webinar.OrganizerOAuthKey, orgKey, cWebinarKey); // {};

            var orders = _orderManagementService.GetOrdersForLiveNotifications(idWebinar);
            object test = null;
            var sb = new StringBuilder();
            sb.Append("ListSentConnectionInfo: ");

            orders.ToList().ForEach((order) =>
            {
                var row = order.OrderRows.SingleOrDefault(r => r.RowStatus == OrderRowStatus.Active);
                sb.Append(order.BillingEmail + ", ");
                var notificationStorage = new NotificationStorage
                {
                    idOrder = order.idOrder,
                    SessionStartInfo = _appHelper.GetSessionStartInfo()
                };

                order.NotificationStorage = JsonConvert.SerializeObject(notificationStorage);
                if (row.RegistrantKey == null && row.RegistrationType.ShowLiveNotifications == "Yes"
                    && (row.Webinar.Status == WebinarStatus.Active || row.Webinar.Status == WebinarStatus.InProgress))
                    order = _orderManagementService.GenerateRegistrantKey(order);

                if (order.OrderRows.SingleOrDefault(r => r.RowStatus == OrderRowStatus.Active).AdditionalLocation != null)
                    SendConnectionInfoToAddLoc(order, idWebinar);

                _orderManagementService.SaveChanges();
                var body = BuildConnectionInfoMessage(order);
                body = _appHelper.CleanHtmlCodesAndLogo(body, _globalConfig.TenantLogo);

                _orderManagementService.FireMandrillNotificationEvent(
                    ConfigurationManager.AppSettings["TestEmailAddress"],
                    "Connection Checklist for " + GetWebinar(idWebinar).Title, body);


            });

            _logger.Info(sb.ToString());
            _orderManagementService.FireSendConnectionInfoNotificationEvent(orders, false);
        }

        private void SendConnectionInfoToAddLoc(Order order, int idWebinar)
        {
            var addLocs = order.OrderRows.SingleOrDefault(r => r.RowStatus == OrderRowStatus.Active).AdditionalLocation;

            foreach (var loc in addLocs)
            {
                if (string.IsNullOrEmpty(loc.RegistrantKey))
                {
                    try
                    {
                        var key = _orderManagementService.GenerateRegistrantKey(order);
                        _orderManagementService.SaveOrderChanges(key, null, null);

                    }
                    catch (Exception ex)
                    {

                        _logger.FatalException("SendConnInfoAddLoc", ex);
                    }

                }

                var body = BuildConnectionInfoMessage(order, loc.Email);
                body = _appHelper.CleanHtmlCodesAndLogo(body, _globalConfig.TenantLogo);

                _orderManagementService.FireMandrillNotificationEvent(ConfigurationManager.AppSettings["TestEmailAddress"], "Connection Checklist for " + GetWebinar(idWebinar).Title, body);

            }
        }

        public Webinar GetWebinar(int idWebinar)
        {
            return _webinarManagementService.GetWebinar(idWebinar);
        }

        public ActionResult Identify(IdentifyModel identifyModel, int id)
        {
            // do something with name and email address
            var order = _orderManagementService.GetOrderById(id);
            if (order == null) throw new NullReferenceException("order");

            var fieldsToComments = new PostEventMaterialsWereAccessed
            {
                DateAdded = TtsConfig.UtcNowAsCts,
                OnDemandCode = identifyModel.OnDemandCode,
                UserEmail = identifyModel.Email,
                UserName = identifyModel.FullName,
                UserAudit = _appHelper.GetUserAuditInfo()
            };

            try
            {

                string updatedUserComments = JsonHelpers.AddObjectToJsonArray(
                   order.UserComments,
                   JsonPropertyKeys.PostEventMaterialsWereAccessedKey,
                   fieldsToComments
                   );
                order.UserComments = updatedUserComments;

            }
            catch (Exception ex)
            {
                _logger.FatalException("UserComments for OnDemand access failed to save and threw: " + order.UserComments, ex);
                throw;
            }

            try
            {
                _orderManagementService.SaveChanges();
            }
            catch (DbEntityValidationException dbEntityValidationException)
            {
                StringBuilder stringBuilder = new StringBuilder();
                foreach (var error in dbEntityValidationException.EntityValidationErrors.SelectMany(s => s.ValidationErrors))
                {
                    stringBuilder.AppendFormat("Property: {0} Error: {1} ", error.PropertyName, error.ErrorMessage);
                }
                _logger.FatalException("Identify | OnDemand claim processing: " + stringBuilder.ToString(), dbEntityValidationException);
                Trace.TraceInformation(stringBuilder.ToString());
            }

            // now that we've updated UserComments, add claim for PostEvent Access
            var newJson =
                new JProperty(
                    string.Concat("PostEventAccessByAnonUser-",
                        DomainConstants.BuildUtcNowAsCts.ToString(DomainConstants.DateTimeLongFormat)),
                    new JObject(
                        new JProperty("Name", identifyModel.FullName),
                        new JProperty("Email", identifyModel.Email)
                        ));

            var validationResultAdmin = _jsonValidator.Validate(new ValidationString(order.AdminComments));

            if (validationResultAdmin.IsValid || string.IsNullOrWhiteSpace(order.AdminComments))
            {
                order.AdminComments = JsonHelpers.MergeJsonWithStoredField(order.AdminComments, newJson);
            }
            else
            {
                _logger.Warn("UserComments for OnDemand access failed to save: {0}", order.UserComments);
            }

            _orderManagementService.SaveChanges();

            _stateService.SetValue(WebUiConstants.AnonUserIdentified, true);

            return
                new RedirectToRouteResult(
                    new RouteValueDictionary(
                        new { action = "OnDemand", controller = "Webinar", onDemandCode = identifyModel.OnDemandCode })
                        );
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


        public ActionResult OnDemandLegacy(int idWebinar, int idUser)
        {


            //var claimsIdentityOfAuthenticatedUser = (ClaimsIdentity)userIdentity;

            //var orders = _orderManagementService.GetOrdersByUserId(idUser);

            //var order = orders.Where(o => o.OrderRows.SingleOrDefault
            //    (s => s.RowStatus == OrderRowStatus.Active)
            //        .idWebinar);
            var webinar = GetWebinar(idWebinar);

            var playModel = new OnDemandPlaybackModel
            {
                Presenter = webinar.Presenter,
                Webinar = webinar,
                AuthorizedToAccessMaterials = "until"
                // magic string == 'true'
            };

            var viewResult = new ViewResult { ViewName = "OnDemand" };
            viewResult.ViewData.Model = playModel;

            return viewResult;
            //if (claimsIdentityOfAuthenticatedUser.IsAuthenticated)
            //{
            //    //permits admins to preview onDemand pages
            //    //if (claimsIdentityOfAuthenticatedUser.HasClaim((claim) => claim.Type == ClaimTypes.Admin))
            //    //{
            //    //    return new RedirectToRouteResult(new RouteValueDictionary(new { action = "Index", controller = "Admin" }));
            //    //}

            //    ProcessAccessPermissionsForMaterials(order, playModel);

            //   
            //}

            //if (_stateService.HasValue(WebUiConstants.AnonUserIdentified) && _stateService.GetValue<bool>(WebUiConstants.AnonUserIdentified))
            //{
            //    ProcessAccessPermissionsForMaterials(order, playModel);

            //    return viewResult;
            //}

            //return new RedirectToRouteResult(new RouteValueDictionary(new { action = "Identify", controller = "Webinar", onDemandCode = id.ToString() + "-" + onDemandCode }));
        }

        private string ProcessAccessPermissionsForMaterials(Order order, OnDemandPlaybackModel playModel)
        {
            var webUser = _membershipService.GetWebUserById(order.idUser);

            if (webUser != null)
            {

                var myClaim = _membershipService.GetDisplayPostEventMaterialsClaimValue(
                    _globalConfig.Tenant,
                    webUser.email, playModel.idOrder, playModel.OnDemandCode);
                //"{\"OrderId\":92941,\"ExpiryDate\":\"2016-01-08\",\"OnDemandCode\":\"pixi\"}"
                //var checkExpired = GetPostEventMaterialsAccessExpiry(order);
                var thisClaim = JsonConvert.DeserializeObject<PostEventClaim>(myClaim);

                if (myClaim == null)
                {
                    playModel.AuthorizedToAccessMaterials = "Invalid code: " + playModel.OnDemandCode;
                }
                else
                {
                    if (thisClaim.ExpiryDate.AddDays(1) > DateTime.Today.AddHours(23))
                    {
                        playModel.AuthorizedToAccessMaterials = "This code is valid through " +
                                                                thisClaim.ExpiryDate.ToShortDateString() + ".";

                        _logger.Info("AuthorizedToAccessMaterials granted to: " + webUser.email);

                        return myClaim;
                    }
                    playModel.AuthorizedToAccessMaterials = "Code expired on " + thisClaim.ExpiryDate.ToShortDateString();

                    _logger.Warn("AuthorizedToAccessMaterials found expired code: " + webUser.email + " " +
                                 playModel.OnDemandCode);
                    return "Code is Expired.";
                }
                return "Not Authorized";
            }

            _logger.Error("No WebUser exists with the Id {0}", order.idUser);

            return "User Not Found";
            //ModelState.AddModelError(string.Empty,
            //    string.Format("No WebUser exists with the Id {0}", order.idUser));
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

            var webinar = _webinarManagementService.GetWebinar(connectionInfoModel.idWebinar);
            var doc = new HtmlAgilityPack.HtmlDocument();
            //doc.Load(filename);
            doc.LoadHtml(connectionInfoModel.ManageURL);

            var root = doc.DocumentNode;
            var audioNode = root.SelectNodes("//span[contains(@class, 'audio-role-bold')]/parent::p");

            var citrixRegisterUrl = "";
            var accessCodeAttendee = "";
            var accessCodePresenter = "";
            var accessCodeOrganizer = "";
            var webinarKey = "";
            var accessPhone = "";

            webinarKey = root.SelectSingleNode("//span[contains(@id,'WebinarInfoID')]").InnerHtml;
            webinarKey = webinarKey.Replace("-", "").Trim();
            accessPhone = root.SelectSingleNode("//p[contains(@id, 'audioInstructions')]/following::p[1]//span[1]").InnerHtml;
            //accessPhone = root.SelectSingleNode("//p[text()[starts-with(., 'Toll-')]]").InnerHtml;
            //accessPhone = Regex.Split(accessPhone, @"\<br\>")[1].Replace("Toll-free: 1 ", "").Trim().Replace(" ", "-");
            // element at [1] not present starting 7/6/2015
            accessPhone = Regex.Split(accessPhone, @":")[1].Replace("Toll-free: 1 ", "").Trim().Replace(" ", "-");


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
                    accessCodeAttendee = aNode.ParentNode.OuterHtml.Split(':')[1].Trim().Split(' ')[0].Replace("</span>", "");
                }
            }

            webinar.AccessCodeAttendee = accessCodeAttendee;
            webinar.AccessCodeOrganizer = accessCodeOrganizer;
            webinar.AccessCodePresenter = accessCodePresenter;
            webinar.AccessPhone = accessPhone;
            webinar.CitrixRegisterUrl = citrixRegisterUrl;
            webinar.OrganizerOAuthKey = connectionInfoModel.OrganizerOAuthKey;
            webinar.OrganizerKey = connectionInfoModel.OrganizerKey;
            webinar.WebinarKey = webinarKey.Trim();

            if (webinar.WebinarFiles != null || webinar.WebinarFiles.Count < 1)
            {
                webinar.Status = WebinarStatus.Active;

                var changeString = PublishStateChange(webinar.Status.ToString() + " to " + "Active", webinar);

                webinar.ConnectionInfo = changeString;
            }


            UpdateWebinar(webinar);

            detailsValid = true;


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

        public IEnumerable<Webinar> SearchWebinars(string searchTerm)
        {
            var webinars = _webinarManagementService.GetWebinarByPresenterLastName(searchTerm);
            if (!ReferenceEquals(webinars, null) && webinars.Count() > 0)
            {
                return webinars.Where(w => w.Status != WebinarStatus.Deleted && w.Status != WebinarStatus.Pending && w.Status != WebinarStatus.Archived).OrderByDescending(w => w.Date);
            }

            webinars = _webinarManagementService.GetWebinarsByPresenterFullName(searchTerm);
            if (!ReferenceEquals(webinars, null) && webinars.Count() > 0)
            {
                return webinars.Where(w => w.Status != WebinarStatus.Deleted && w.Status != WebinarStatus.Pending && w.Status != WebinarStatus.Archived).OrderByDescending(w => w.Date);
            }
            var webinarsByTopic = _webinarManagementService.GetWebinarByDescription(searchTerm);

            return webinarsByTopic.Where(w => w.Status != WebinarStatus.Deleted && w.Status != WebinarStatus.Pending && w.Status != WebinarStatus.Archived).OrderByDescending(w => w.Date);

        }

        public bool UpdateWebinarRecording(WebinarDetailsViewModel webinarDetailsViewModel, out string message)
        {
            int webinarId = 0;
            message = string.Empty;

            try
            {
                Webinar webinar = _webinarManagementService.GetWebinar(webinarDetailsViewModel.Webinar.idWebinar);

                var checkThatNewFilesExist = CheckThatFileExists(webinarDetailsViewModel.Webinar.RecordingUrl);

                if (checkThatNewFilesExist != "OK")
                {
                    message = string.Concat(webinarDetailsViewModel.Webinar.RecordingUrl, " does not exist.");
                    _logger.Error(string.Format(webinarDetailsViewModel.Webinar.RecordingUrl, " does not exist."));
                    return false;
                }
                webinar.RecordingUrl = webinarDetailsViewModel.Webinar.RecordingUrl;

                webinar.Status = WebinarStatus.Recorded;
                webinar.LivePlusFiveValue = Convert.ToDateTime(webinarDetailsViewModel.Webinar.LivePlusFiveValue.ToShortDateString()).AddHours(23).AddMinutes(59);
                _webinarManagementService.UpdateWebinar(webinar);

                _logger.Info(string.Format("Recordings posted for {0} is saved to {1}", webinar.idWebinar + " - " + webinar.Title, webinar.RecordingUrl));
                SendRecordingIsPostedNotifications(webinarDetailsViewModel, webinar);
                return true;
            }
            catch (Exception exception)
            {
                _logger.ErrorException(string.Format("UpdateWebinarRecording| UpdateWebinarRecording failed {0} on idWebinar: {1}", exception.Message, webinarId), exception);
                message = string.Format("UpdateWebinarRecording| UpdateWebinarRecording failed {0} on idWebinar: {1}", exception.Message, webinarId);
            }

            return false;
        }

        public bool UpdateWebinarRecordingBatch(int idWebinar, string recordingURL, out string message)
        {
            int webinarId = 0;
            message = string.Empty;

            try
            {
                Webinar webinar = _webinarManagementService.GetWebinar(idWebinar); //_webinarManagementService.GetWebinar(webinarDetailsViewModel.Webinar.idWebinar);

                var checkThatNewFilesExist = CheckThatFileExists(webinar.RecordingUrl);

                if (checkThatNewFilesExist != "OK")
                {
                    message = string.Concat(webinar.RecordingUrl, " does not exist.");
                    _logger.Error(string.Format(webinar.RecordingUrl, " does not exist."));
                    return false;
                }
                webinar.RecordingUrl = webinar.RecordingUrl;

                webinar.Status = WebinarStatus.Recorded;

                _webinarManagementService.UpdateWebinar(webinar);

                _logger.Info(string.Format("Recordings posted by UpdateWebinarRecordingBatch for {0} is saved to {1}", webinar.idWebinar + " - " + webinar.Title, webinar.RecordingUrl));
                SendRecordingIsPostedBatch(idWebinar);
                return true;

            }
            catch (Exception exception)
            {
                _logger.ErrorException(string.Format("UpdateWebinarRecording| UpdateWebinarRecording failed {0} on idWebinar: {1}", exception.Message, webinarId), exception);
                message = string.Format("UpdateWebinarRecording| UpdateWebinarRecording failed {0} on idWebinar: {1}", exception.Message, webinarId);
            }

            return false;
        }

        private void SendRecordingIsPostedNotifications(WebinarDetailsViewModel webinarDetailsViewModel, Webinar webinar)
        {
            var ordersForWebinar = _orderManagementService.GetV3OrdersByWebinar(webinar.idWebinar);
            IList<int> orderIDsForWebinar = _orderManagementService.GetV3OrdersIdsByWebinar(webinar.idWebinar);
            // 95336


            try
            {
                foreach (var orderId in orderIDsForWebinar)
                {
                    var order = _orderManagementService.GetOrderById(orderId);
                    _membershipService.AddClaimForPostEventMaterials(order.BillingEmail
                        , order.OrderRows.Single(r => r.RowStatus == OrderRowStatus.Active)
                        , _orderManagementService.CalculatePostEventMaterialsAccessExpiry(order.OrderRows.Single(r => r.RowStatus == OrderRowStatus.Active))
                        , _globalConfig.Tenant);

                    //Mandrill-specific handling
                    var toEmail = order.BillingEmail;
                    var subject = "[" + _globalConfig.Tenant + "] tester for  " +
                    //var subject = "[" + _globalConfig.Tenant + "] OnDemand recording posted for  " +
                                  order.OrderRows.SingleOrDefault(r => r.RowStatus == OrderRowStatus.Active)
                                      .Webinar.Title;

                    if (order.OrderStatus != OrderStatus.Paid)
                    {
                        order.OrderStatus = OrderStatus.Billed;
                        _orderManagementService.SaveChanges();
                    }

                    var body = BuildRecordingIsPostedMessage(order);
                    body = _appHelper.CleanHtmlCodesAndLogo(body, _globalConfig.TenantLogo);

                    //_orderManagementService.FireMandrillNotificationEvent(toEmail, subject, body);
                    _orderManagementService.FireMandrillNotificationEvent(ConfigurationManager.AppSettings["TestEmailAddress"], subject, body);
                }
            }
            catch (Exception ex)
            {
                _logger.ErrorException(string.Format("SendRecordingIsPostedNotifications| AddClaimForPostEventMaterials failed {0} on idWebinar: {1}", ex.Message, webinar.idWebinar), ex);
            }

            _orderManagementService.FireSendRecordingIsPostedEvent(ordersForWebinar);

        }

        public string BuildConnectionInfoMessage(Order order, string addLoc = null)
        {
            OrderRow row = order.OrderRows.SingleOrDefault(r => r.RowStatus == OrderRowStatus.Active);
            Webinar webinar = row.Webinar;
                DocumentModel document =
                    DocumentModel.Load(
                        System.Web.HttpContext.Current.Server.MapPath(
                            @"~/App_Data/mergeTemplates/ConnectionInfo2.docx"));

                var fields = _appHelper.BuildNotiFields(order);

                document.MailMerge.Execute(fields);
            if (addLoc != null)
            {
                document =
                    DocumentModel.Load(
                        System.Web.HttpContext.Current.Server.MapPath(
                            @"~/App_Data/mergeTemplates/ConnectionInfo2AddLoc.docx"));

                order.BillingEmail = addLoc;
                fields = _appHelper.BuildNotiFields(order);

                document.MailMerge.Execute(fields);
                _logger.Info("BuildConnectionInfoMessage AddLoc: " + addLoc + " fields:" + JsonConvert.SerializeObject(fields));
            }

            bool noError = true;
            try
            {
                if (noError)
                {

                    //// SAVE LOCALLY if needed for easier testing
                    //document.Save(System.Web.HttpContext.Current.Server.MapPath(@"~/App_Data/mergeTemplates/" + order.idOrder + ".pdf"), SaveOptions.PdfDefault);

                    var storageCredentials = new StorageCredentials(_globalConfig.StorageAccountName,
                        _globalConfig.StorageAccessKey);

                    var cloudStorageAccount = new CloudStorageAccount(storageCredentials, false);
                    CloudBlobClient blobClient = cloudStorageAccount.CreateCloudBlobClient();

                    // Retrieve reference to a previously created container.
                    CloudBlobContainer container = blobClient.GetContainerReference("connectionchecklist");
                    container.CreateIfNotExists();

                    CloudBlockBlob blob = container.GetBlockBlobReference(webinar.idWebinar + "/" + order.idOrder + ".pdf");

                    using (MemoryStream output = new MemoryStream())
                    {
                        document.Save(output, SaveOptions.PdfDefault);
                        output.Position = 0; // reset to beginning so Upload operation can work correctly
                        blob.UploadFromStream(output);
                    }

                    blob = container.GetBlockBlobReference(webinar.idWebinar + "/" + order.idOrder + ".htm");
                    using (MemoryStream output = new MemoryStream())
                    {
                        document.Save(output, SaveOptions.HtmlDefault);
                        output.Position = 0; // reset to beginning so Upload operation can work correctly
                        blob.UploadFromStream(output);
                    }

                    var blobDoc = container.GetBlockBlobReference(webinar.idWebinar + "/" + order.idOrder + ".docx");
                    using (MemoryStream output = new MemoryStream())
                    {
                        document.Save(output, SaveOptions.DocxDefault);
                        output.Position = 0; // reset to beginning so Upload operation can work correctly
                        blobDoc.UploadFromStream(output);
                    }

                    byte[] fileContents;
                    using (MemoryStream output = new MemoryStream())
                    {
                        document.Save(output, SaveOptions.HtmlDefault);
                        output.Position = 0; // reset to beginning so Upload operation can work correctly

                        fileContents = output.ToArray();
                    }

                    return System.Text.Encoding.UTF8.GetString(fileContents);
                }
                else
                {
                    https://ci4.googleusercontent.com/proxy/AOF0zatzFSovHlWus8P1dHxNNFo0tLbt-mot0d9e-Of2y7-y9OixCjE7b48XZyxMDreHdAqWirQiZ5bnZNro7z99YsBEbeXmAtMPk4wXt_4cag5u=s0-d-e1-ft#http://devholmen15:3538/Content/images/vrLocal/left_shadow.jpg
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.ErrorException("buildingRecordingIsPosted for: " + order.idOrder, ex);
                return "Error: " + ex.Message;
            }
        }


        public string BuildRecordingIsPostedMessage(Order order)
        {
            OrderRow row = order.OrderRows.SingleOrDefault(r => r.RowStatus == OrderRowStatus.Active);
            Webinar webinar = row.Webinar;
            DocumentModel document =
                DocumentModel.Load(
                    System.Web.HttpContext.Current.Server.MapPath(
                        @"~/App_Data/mergeTemplates/RecordingIsPostedToExistingUserBW.docx"));

            if (_globalConfig.Tenant == "CUWebinars")
            {
                document =
                   DocumentModel.Load(
                       System.Web.HttpContext.Current.Server.MapPath(
                           @"~/App_Data/mergeTemplates/RecordingIsPostedToExistingUserCU.docx"));
            }

            var fields = _appHelper.BuildNotiFields(order);

            var oDClaim = _orderManagementService.GetOnDemandClaimById(order.idOrder);
            var thisClaim = JsonConvert.DeserializeObject<Models.JsonModels.PostEventClaim>(oDClaim.ToString());
            //

            fields.Expires = thisClaim.ExpiryDate.ToShortDateString();

            document.MailMerge.Execute(fields);
            
            bool noError = true;
            try
            {
                if (noError)
                {
                    _logger.Info("begins write to file: " + order.idOrder);

                    //// SAVE LOCALLY if needed for easier testing
                    //document.Save(System.Web.HttpContext.Current.Server.MapPath(@"~/App_Data/mergeTemplates/" + order.idOrder + ".pdf"), SaveOptions.PdfDefault);

                    var storageCredentials = new StorageCredentials(_globalConfig.StorageAccountName,
                        _globalConfig.StorageAccessKey);

                    var cloudStorageAccount = new CloudStorageAccount(storageCredentials, false);
                    CloudBlobClient blobClient = cloudStorageAccount.CreateCloudBlobClient();

                    // Retrieve reference to a previously created container.
                    CloudBlobContainer container = blobClient.GetContainerReference("recordingisposted");
                    container.CreateIfNotExists();

                    CloudBlockBlob blob = container.GetBlockBlobReference(webinar.idWebinar + "/" + order.idOrder + ".pdf");

                    using (MemoryStream output = new MemoryStream())
                    {
                        document.Save(output, SaveOptions.PdfDefault);
                        output.Position = 0; // reset to beginning so Upload operation can work correctly
                        blob.UploadFromStream(output);
                    }

                    //blob = container.GetBlockBlobReference(webinar.idWebinar + "/" + order.idOrder + ".gif");
                    //using (MemoryStream output = new MemoryStream())
                    //{
                    //    document.Save(output, new ImageSaveOptions() { Format = ImageSaveFormat.Gif});
                    //    output.Position = 0; // reset to beginning so Upload operation can work correctly
                    //    blob.UploadFromStream(output);
                    //}

                    blob = container.GetBlockBlobReference(webinar.idWebinar + "/" + order.idOrder + ".htm");
                    using (MemoryStream output = new MemoryStream())
                    {
                        document.Save(output, SaveOptions.HtmlDefault);
                        output.Position = 0; // reset to beginning so Upload operation can work correctly
                        blob.UploadFromStream(output);
                    }

                    byte[] fileContents;
                    using (MemoryStream output = new MemoryStream())
                    {
                        document.Save(output, SaveOptions.HtmlDefault);
                        output.Position = 0; // reset to beginning so Upload operation can work correctly

                        fileContents = output.ToArray();
                    }

                    return System.Text.Encoding.UTF8.GetString(fileContents);
                }
                else
                {
                    https://ci4.googleusercontent.com/proxy/AOF0zatzFSovHlWus8P1dHxNNFo0tLbt-mot0d9e-Of2y7-y9OixCjE7b48XZyxMDreHdAqWirQiZ5bnZNro7z99YsBEbeXmAtMPk4wXt_4cag5u=s0-d-e1-ft#http://devholmen15:3538/Content/images/vrLocal/left_shadow.jpg
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.ErrorException("buildingRecordingIsPosted for: " + order.idOrder, ex);
                return "Error: " + ex.Message;
            }
        }


        private void AddNoteClaim(IList<Order> ordersForWebinar)
        {

            foreach (var order in ordersForWebinar)
            {
                var userAccountOfOrderer = _membershipService.GetUserAccountByEmail(
                    _globalConfig.Tenant,
                    order.WebUser.email
                    );

                try
                {
                    // and add logs retrieval
                    // Zopim
                    // email
                    // Grasshopper


                    var addNote = GetLoggedIncidents(order);

                    var note = addNote + " Prior Comments: " + order.UserComments;

                    var orderIdProperty = new JProperty(JsonPropertyKeys.OrderId, order.idOrder);
                    var noteToOrderProperty = new JProperty(JsonPropertyKeys.Note, note);

                    var claimValue = new JObject(
                        orderIdProperty,
                        noteToOrderProperty
                        );

                    _membershipService.AddClaim(
                        userAccountOfOrderer, ClaimTypes.OrderNote, claimValue.ToString(Formatting.None)
                        );

                    _logger.Info("AddClaimForOrderNote: " + order.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active).Webinar.idWebinar + " - " + order.idOrder + ": " + order.UserComments);

                }
                //how to get this ex to detail the tossed error? Inner ex is null.
                catch (Exception ex)
                {
                    _logger.FatalException("AddClaimForOrderNote| MR record not found " + order.BillingEmail + " ", ex);
                }
            }

        }

        private object GetLoggedIncidents(Order order)
        {
            return null;
        }

        public void SendRecordingIsPostedBatch(int idWebinar)
        {
            var ordersForWebinar = _orderManagementService.GetV3OrdersByWebinarForPostEventClaims(idWebinar).ToList();
            AddNoteClaim(ordersForWebinar);

        }

        public void SendRecordingIsPostedPerOrder(int idWebinar, string note)
        {
            throw new NotImplementedException();
        }


        public ConnectionInfoEditModel GetConnectionInfo(string webinarKey)
        {
            throw new NotImplementedException();
        }


        public void UpdateWebinar(Webinar webinar)
        {
            _webinarManagementService.UpdateWebinar(webinar);
        }

        public bool UpdateWebinarFiles(WebinarFilesEditModel webinarFilesEditModel, out string message)
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

                if (ReferenceEquals(null, webinarFilesEditModel.WebinarFiles))
                {
                    message = "No updates, additions or deletions were submitted.";
                    _logger.Error("No updates, additions or deletions were submitted.");
                    return false;
                }


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
                        message = string.Concat(webinarFile.fileLocation, " does not exist.");
                        _logger.Error(string.Format("{0} does not exist.", webinarFile.fileLocation));
                        return false;
                    }

                }

                _webinarManagementService.AddWebinarFiles(newFiles);
                _webinarManagementService.DeleteWebinarFiles(deletedFiles);
                _webinarManagementService.UpdateWebinarFiles(updatedFiles);

                return true;
            }
            catch (Exception exception)
            {
                _logger.ErrorException(
                    string.Format("UpdateWebinarFiles| UpdateWebinarFiles failed {0}", exception.Message),
                    exception
                    );
                message = "Update failed. We have logged an error.";

            }
            return false;
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
            var presenters = _webinarManagementService.GetAllPresenters().OrderBy(p => p.WebUser.LastName)
                .Select(presenter => new SelectListItem
                {
                    Text = presenter.WebUser.FullName,
                    Value = presenter.idUser.ToString()
                });
            var topics = _webinarManagementService.GetAllTopics();

            var statuses = (from object value in Enum.GetValues(typeof(WebinarStatus))
                            select new SelectListItem { Text = value.ToString(), Value = ((int)value).ToString() }).ToList();


            var webinarEditModel = new WebinarEditModel
            {
                AdditionalLocationsPrice = _orderManagementService.GetAdditionalLocationsPricing(idWebinar),
                ceu = webinar.ceu,
                LivePlusFive = webinar.Date.AddDays(7),
                PostedTopics = new PostedTopics { TopicIds = topicIdsForWebinar.Select(topic => topic.idTopic).ToArray() },
                PostedRegTypeGroups =
                    new PostedRegTypeGroups
                    {
                        RegTypeGroupIds = regTypeGroupsForWebinars.Select(r => r.idRegTypeGroup).ToArray()
                    },
                Presenters = presenters,
                RegTypeGroups = upcomingRegTypeGroups.OrderBy(r => r.SortOrder),
                RecordingUrl = webinar.RecordingUrl,
                SelectedPresenter = webinar.idPresenter,
                SelectedRegTypeGroups = regTypeGroupsForWebinars,
                SelectedTopics = topicIdsForWebinar,
                SelectedStatus = (int)webinar.Status,
                SeriesInfo = webinar.SeriesInfo,
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

            //initialize Date value
            webinarEditModel.LivePlusFive = webinarEditModel.Date.AddDays(7);

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
                WhoAttend = webinarEditModel.WhoAttend,
                LivePlusFiveValue = webinarEditModel.LivePlusFive

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

            webinar.AdditionalLocationPrice = _orderManagementService.GetAdditionalLocationsPricing(webinar.idWebinar);

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
            webinar.LivePlusFiveValue = webinarEditModel.LivePlusFive;

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

            webinar.AdditionalLocationPrice = _orderManagementService.GetAdditionalLocationsPricing(webinar.idWebinar);
            //var additionalLocationsLookupPrice = _webinarManagementService.GetAdditionalLocationsLookupPricesForWebinar(webinar.idWebinar).SingleOrDefault();

            //if (ReferenceEquals(null, additionalLocationsLookupPrice))
            //{
            //    _webinarManagementService.AddAdditionalLocationsLookupPrice(new AdditionalLocationsLookupPrice
            //    {
            //        Cost = webinarEditModel.AdditionalLocationsPrice,
            //        idWebinar = webinar.idWebinar
            //    });
            //}
            //else
            //{
            //    additionalLocationsLookupPrice.Cost = webinarEditModel.AdditionalLocationsPrice;
            //}

            _webinarManagementService.UpdateWebinar(webinar);
        }



        private string CheckThatFileExists(string newFile)
        {
            var handoutRepo = _globalConfig.HandoutRepository;

            HttpWebResponse response = null;
            string uri = handoutRepo + newFile;
            var request = (HttpWebRequest)WebRequest.Create(uri);
            request.Method = "HEAD";
            try
            {
                response = (HttpWebResponse)request.GetResponse();
                var result = response.StatusCode.ToString();
                return result;
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