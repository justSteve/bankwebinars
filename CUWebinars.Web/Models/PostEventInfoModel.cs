using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using CUWebinars.Business.Models;

namespace CUWebinars.Web.Models
{
    public class PostEventInfoModel
    {

        public int idWebinar { get; set; }

        [Display(Name = "Citrix Key")]
        public string WebinarKey { get; set; }
        public string OrganizerKey { get; set; }
        public string OrganizerOAuthKey { get; set; }
        public string ManageURL { get; set; }


        [Display(Name = "Webinar Recording")]
        public string RecordingURL { get; set; }
        [Display(Name = "Webinar Quiz")]
        public TTSQuiz Assessment { get; set; }
        [Display(Name = "Presenter Code")]
        public TTSAttendance CertificateOfAttendance { get; set; }
        [Display(Name = "Organizer Code")]
        public string AccessCodeOrganizer { get; set; }

        public ICollection<WebinarFile> WebinarFiles { get; set; }

    }
}