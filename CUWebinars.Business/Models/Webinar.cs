using System;
using System.Collections.Generic;

namespace CUWebinars.Business.Models
{
    public partial class Webinar
    {
        public Webinar()
        {
            this.HostPropertyValues = new List<HostPropertyValue>();
            this.OptionsGroupsXrefs = new List<OptionsGroupsXref>();
            this.OrderRows = new List<OrderRow>();
            this.WebinarFiles = new List<WebinarFile>();
            this.WebinarTopicXrefs = new List<WebinarTopicXref>();
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
        public int idPresenter { get; set; }
        public string AdditionalNotifications { get; set; }
        public string ceu { get; set; }
        public System.DateTime DateCreated { get; set; }
        public System.DateTime DateChanged { get; set; }
        public virtual ICollection<HostPropertyValue> HostPropertyValues { get; set; }
        public virtual ICollection<OptionsGroupsXref> OptionsGroupsXrefs { get; set; }
        public virtual ICollection<OrderRow> OrderRows { get; set; }
        public virtual Presenter Presenter { get; set; }
        public virtual ICollection<WebinarFile> WebinarFiles { get; set; }
        public virtual ICollection<WebinarTopicXref> WebinarTopicXrefs { get; set; }
    }
}
