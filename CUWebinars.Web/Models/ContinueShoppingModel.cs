using System.Collections.Generic;
using CUWebinars.Business.Models;

namespace CUWebinars.Web.Models
{
    public class ContinueShoppingModel
    {
        public IEnumerable<Webinar> SelectedWebinars { get; set; }
        public IEnumerable<Webinar> SelectedRelated { get; set; }
        public IEnumerable<Webinar> SelectedPresenter { get; set; }
        public IEnumerable<Webinar> SelectedTopics { get; set; }
    }
}