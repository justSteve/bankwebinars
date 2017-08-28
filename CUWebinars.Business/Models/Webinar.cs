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


        public string OpeningMessage { get; set; }
        public int idPresenter { get; set; }
        public int idWebinar { get; set; }

        public string AccessCodeAttendee { get; set; }
        public string AccessCodeOrganizer { get; set; }
        public string AccessCodePresenter { get; set; }
        public string AccessPhone { get; set; }
        public decimal AdditionalLocationPrice { get; set; }
        public string ceu { get; set; }
        public string CitrixRegisterUrl { get; set; }
        public string ConnectionInfo { get; set; }

        public System.DateTime Date { get; set; }
        public System.DateTime DateChanged { get; set; }
        public System.DateTime DateCreated { get; set; }
        public string Description { get; set; }
        public string DescriptionLong { get; set; }
        public decimal Duration { get; set; }
        public string ImageUrl { get; set; }
        public string LearnBody { get; set; }
        public string LearnCaption { get; set; }
        public DateTime LivePlusFiveValue { get; set; }
        public string OrganizerKey { get; set; }
        public string OrganizerOAuthKey { get; set; }
        public string RecordingUrl { get; set; }
        public string SeriesInfo { get; set; }
        public string SmallImageUrl { get; set; }
        public WebinarStatus Status { get; set; }
        public string Title { get; set; }
        public string TitleAnnouncement { get; set; }
        public string WebinarKey { get; set; }
        public string WhoAttend { get; set; }

        public virtual ICollection<OrderRow> OrderRows { get; set; }
        public virtual ICollection<RegTypesGroupsXref> RegTypesGroupsXref { get; set; }
        public virtual ICollection<WebinarFile> WebinarFiles { get; set; }
        public virtual ICollection<WebinarTopicXref> WebinarTopicXrefs { get; set; }
        public virtual Presenter Presenter { get; set; }
        public string Campaigns { get; set; }
        public string Comments { get; set; }
    }
}
