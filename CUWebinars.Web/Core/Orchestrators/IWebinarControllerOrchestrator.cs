using System;
using System.Collections.Generic;
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
        void FireSendConnectionInfoNotificationEvent(int idWebinar);
        Webinar GetWebinar(int idWebinar);

        Webinar PopulateWebinarFromViewModel(ConnectionInfoEditModel connectionInfoEditModel, out bool detailsValid);
        string PublishStateChange(string stateFromTo, Webinar webinar);
        IEnumerable<Webinar> SearchWebinars(string lastName);
        //string SetEventToRecorded(int webinarId);
        void UpdateWebinar(Webinar webinar);
        void UpdateWebinarFromViewInput(WebinarEditModel webinarEditModel);
        string SetEventToRecorded(int webinarId);
    }
}