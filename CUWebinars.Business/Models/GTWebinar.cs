using System;
using System.Collections.Generic;

namespace CUWebinars.Business.Models
{
    public partial class GTWebinar
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public int DescriptionMaxCharLimit { get; set; }
        public string EndHour { get; set; }
        public string EndMeridian { get; set; }
        //public DateTime EndTime { get; set; }
        public string PresenterFirstName { get; set; }
        public string PresenterLastName { get; set; }
        public bool Pstn { get; set; }
        public bool PstnTF { get; set; }
        //  public ICollection<Presenter> Presenters { get; set; }
        public bool RequirePassword { get; set; }
        //public DateTime StartDate { get; set; } 
        public string StartHour { get; set; }
        public string StartMeridian { get; set; }
        public DateTime StartTime { get; set; }
        public string TemplateTitle { get; set; }
        public int TimeZoneKey { get; set; }
    }
}
