using System.ComponentModel.DataAnnotations;

namespace CUWebinars.Web.Models
{
    public class ConnectionInfoModel : ConnectionInfoEditModel
    {
        [Display(Name = "Citrix Key")]
        public string WebinarKey { get; set; }
        public string CitrixRegisterURL { get; set; }

        [Display(Name = "Webinar Phone Number")]
        public string AccessPhone { get; set; }
        [Display(Name = "Attendee Code")]
        public string AccessCodeAttendee { get; set; }
        [Display(Name = "Presenter Code")]
        public string AccessCodePresenter { get; set; }
        [Display(Name = "Organizer Code")]
        public string AccessCodeOrganizer { get; set; }
    }
}