using CUWebinars.Business.Models;
using CUWebinars.Web.Models;
using CUWebinars.Web.ViewModel;

namespace CUWebinars.Web.Core.Orchestrators
{
    public interface IWebinarControllerOrchestrator
    {
        WebinarEditModel BuildEditModelForWebinarToBeCloned(int idWebinar);
        void CreateWebinarFromViewInput(WebinarEditModel webinarEditModel);
        Webinar GetWebinar(int idWebinar);
        Webinar PopulateWebinarFromViewModel(ConnectionInfoEditModel connectionInfoEditModel, out bool detailsValid);
        string PublishStateChange(string stateFromTo, Webinar webinar);
        string SetEventToRecorded(int webinarId);
        void UpdateWebinar(Webinar webinar);
        void UpdateWebinarFromViewInput(WebinarEditModel webinarEditModel);
    }
}