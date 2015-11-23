
namespace CUWebinars.Business.Models
{
    public enum WebinarStatus
    {
        Pending = 1, // Not Visible on calendar but will display by direct navigation
        Scheduled = 2, // Visible to calendar - GoToWebinar meeting info is not yet generated
        Recorded = 3, // PostEvent - remains in this state for 6 months following 
        Archived = 4, // Still visible to calendar but OnDemand playback is expired
        Active = 7, // 2-3 days prior to the event the Citrix WebinarKey is generated
        InProgress = 8, // Link to event is activated
        Deleted = 6
    }
}
