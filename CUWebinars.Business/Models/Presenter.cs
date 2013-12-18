using System;
using System.Collections.Generic;

namespace CUWebinars.Business.Models
{
    public partial class Presenter
    {
        public Presenter()
        {
            this.Webinars = new List<Webinar>();
        }

        public int idUser { get; set; }
        public string Biography { get; set; }
        public string BiographyLong { get; set; }
        public string PhotoFull { get; set; }
        public string PhotoThumb { get; set; }
        public virtual WebUser WebUser { get; set; }
        public virtual ICollection<Webinar> Webinars { get; set; }
    }
}
