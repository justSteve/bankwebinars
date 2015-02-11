using CUWebinars.Business.Models;
using System.Collections.Generic;

namespace CUWebinars.Web.Models
{
    public class ConnectionInfoEditModel
    {
        public int idWebinar { get; set; }
        public string ManageURL { get; set; }
        public string OrganizerKey { get; set; }
        public string OrganizerOAuthKey { get; set; }

        public ICollection<WebinarFile> WebinarFiles { get; set; }
    }
}