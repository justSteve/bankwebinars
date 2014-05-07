
namespace CUWebinars.Business.Models
{
    public enum WebinarStatus
    {
        Pending = 1, // Not Visible on calendar but will display by direct navigation
        Scheduled = 2, // Visible to calendar - GoToWebinar meeting info is not yet generated
        Active = 3, // 2-3 days prior to the event the Citrix WebinarKey is generated
        Recorded = 4, // PostEvent - remains in this state for 6 months following 
        Archived = 5, // Still visible to calendar but on-demand playback is expired
        Deleted = 6
    }
}
