using CUWebinars.Business.Models;
using CUWebinars.Web.Models;

namespace CUWebinars.Web.Core.Orchestrators
{
    public interface IWebinarControllerOrchestrator
    {
        Webinar PopulateWebinarFromViewModel(ConnectionInfoEditModel connectionInfoEditModel, out bool detailsValid);
        void UpdateWebinar(Webinar webinar);
        string PublishStateChange(string stateFromTo, Webinar webinar);
        string SetEventToRecorded(int webinarId);
    }
}