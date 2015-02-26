using CUWebinars.Business.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace CUWebinars.Web.ViewModel
{
    public class WebinarEditModel
    {
        public Webinar Webinar { get; set; }
        public IEnumerable<SelectListItem> Presenters { get; set; }
        public int SelectedPresenter { get; set; }
        public int SelectedStatus { get; set; }
        public PostedTopics PostedTopics { get; set; }
        public WebinarStatus WebinarStatus { get; set; }
        public IEnumerable<Topic> Topics { get; set; }
        public IEnumerable<Topic> SelectedTopics { get; set; }
    }

    public class PostedTopics
    {
        public string[] TopicIds { get; set; }
    }
}