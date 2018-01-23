using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Web.Mvc;
using CUWebinars.Business.Models;
using CUWebinars.Web.Models;
using CUWebinars.Web.ViewModel;

namespace CUWebinars.Web.Core.Orchestrators
{
    public interface IWebinarControllerOrchestrator : IDisposable
    {
        AdhocNotificationViewModel BuildAdhocNotificationViewModel(WebinarType webinarType);
        WebinarEditModel BuildEditModelForWebinarCreate();
        WebinarEditModel BuildEditModelForWebinar(int idWebinar);
        void CreateWebinarFromViewInput(WebinarEditModel webinarEditModel);
        void FireSendConnectionInfoNotificationEvent(int idOrder, bool reminder);
        Webinar GetWebinar(int idWebinar);
        ActionResult Identify(IdentifyModel identifyModel, int id);
        ActionResult OnDemand(int id, string onDemandCode, IIdentity userIdentity);
        string OpenMeeting(string joinCode, IIdentity userIdentity);
        Webinar PopulateWebinarFromViewModel(ConnectionInfoEditModel connectionInfoEditModel, out bool detailsValid);
        string PublishStateChange(string stateFromTo, Webinar webinar);
        IEnumerable<Webinar> SearchWebinars(string lastName);
        //string SetEventToRecorded(int webinarId);
        void UpdateWebinar(Webinar webinar);
        bool UpdateWebinarFiles(WebinarFilesEditModel webinarFilesEditModel, out string message);
        void UpdateWebinarFromViewInput(WebinarEditModel webinarEditModel);
        bool UpdateWebinarRecording(WebinarDetailsViewModel webinarDetailsViewModel, out string message);
        ActionResult OnDemandLegacy(int idWebinar, int idUser);
        void SendRecordingIsPostedBatch(int idWebinar);
        void SendRecordingIsPostedPerOrder(int idWebinar, string note);
        //string CreateICS(WebinarDetailsViewModel model);
        //List<string> GetCitrixRegsPerWebinar(Webinar webinar);
        ConnectionInfoEditModel GetConnectionInfo(string webinarKey);
        string BuildRecordingIsPostedMessage(Order order);

        DisplayRowPriceViewModel BuildDisplayRowPriceViewModel(OrderRow orderRow, int? idOrderRow, decimal? optionsCost = null);
    }


}