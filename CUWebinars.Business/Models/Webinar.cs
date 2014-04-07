using System;
using System.Collections.Generic;

namespace CUWebinars.Business.Models
{
    public partial class Webinar
    {
        public Webinar()
        {
            //this.HostPropertyValues = new List<HostPropertyValue>();
            //RegTypes = new List<RegType>();
            OrderRows = new List<OrderRow>();
            WebinarFiles = new List<WebinarFile>();
            WebinarTopicXrefs = new List<WebinarTopicXref>();
        }

        public int idWebinar { get; set; }
        public string Description { get; set; }
        public string DescriptionLong { get; set; }
        public string ImageUrl { get; set; }
        public string SmallImageUrl { get; set; }
        public WebinarStatus Status { get; set; }
        public string Title { get; set; }
        public System.DateTime Date { get; set; }
        public string LearnCaption { get; set; }
        public string LearnBody { get; set; }
        public string WhoAttend { get; set; }
        public decimal Duration { get; set; }
        public string RecordingUrl { get; set; }
        //public int idWebinarRegTypeGroup { get; set; }
        public int idPresenter { get; set; }
        public int WebinarKey { get; set; }
        public int OrganizerKey { get; set; }
        public string OrganizerOAuthKey { get; set; }
        public string ceu { get; set; }
        public string ConnectionInfo { get; set; }
        public System.DateTime DateCreated { get; set; }
        public System.DateTime DateChanged { get; set; }
        public virtual ICollection<RegTypesGroupsXref> RegTypesGroupsXref { get; set; }
        //public virtual ICollection<RegType> RegTypes { get; set; }
        public virtual ICollection<OrderRow> OrderRows { get; set; }
        public virtual Presenter Presenter { get; set; }
        public virtual ICollection<WebinarFile> WebinarFiles { get; set; }
        public virtual ICollection<WebinarTopicXref> WebinarTopicXrefs { get; set; }
    }
}
