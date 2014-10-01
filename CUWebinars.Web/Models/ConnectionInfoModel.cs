using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using CUWebinars.Business.Models;

namespace CUWebinars.Web.Models
{
    public class ConnectionInfoModel
    {

        public int idWebinar { get; set; }

        [Display(Name = "Citrix Key")]
        public string WebinarKey { get; set; }
        public string OrganizerKey { get; set; }
        public string OrganizerOAuthKey { get; set; }

        [Display(Name = "Attendee Phone")]
        public string AccessPhoneAttendee { get; set; }
        [Display(Name = "Attendee Code")]
        public string AccessCodeAttendee { get; set; }
        [Display(Name = "Presenter Phone")]
        public string AccessPhonePresenter { get; set; }
        [Display(Name = "Presenter Code")]
        public string AccessCodePresenter { get; set; }
        [Display(Name = "Organizer Phone")]
        public string AccessPhoneOrganizer { get; set; }
        [Display(Name = "Organizer Code")]
        public string AccessCodeOrganizer { get; set; }

        public ICollection<WebinarFile> WebinarFiles { get; set; }

    }
}