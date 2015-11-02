using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CUWebinars.Business.Models
{
    public class SearchDTO
    {
        public int idWebinar { get; set; }
        public string Title { get; set; }
        public string DescriptionLong { get; set; }
        public WebinarStatus Status { get; set; }
        public String WebinarStatusString { get { return Status.ToString(); } }
        public System.DateTime Date { get; set; }
        public string WebinarDateString { get { return Date.ToShortDateString(); } } // to ease consuption in the JS / DataTables caller
        //        
        public string LearnCaption { get; set; }
        public string LearnBody { get; set; }
        public string WhoAttend { get; set; }
        public decimal Duration { get; set; }

        public int idPresenter { get; set; }
        public string PresenterName { get; set; }
        public string PresenterPhotoFull { get; set; }


        //public ICollection<WebinarTopicXref> WebinarTopicXrefs { get; set; }

        //public string Affiliate_ttsDomain { get; set; } // flattened via Automapper to avoid circular reference during JSON serialization

    }
}