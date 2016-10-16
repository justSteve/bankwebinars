using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using CUWebinars.Business.Models;

namespace CUWebinars.Web.Models
{
    public class ContinueShoppingModel
    {
        public int idWebinar { get; set; }
        public IEnumerable<Webinar> SelectedWebinars { get; set; }
        public IEnumerable<Webinar> SelectedRelated { get; set; }
        public String SelectedPresenter { get; set; }
        public ICollection<WebinarTopicXref> SelectedTopics { get; set; }
        public string SearchTerm { get; set; }
    }
}