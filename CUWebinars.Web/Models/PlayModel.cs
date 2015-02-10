using CUWebinars.Business.Models;
using System.Collections.Generic;

namespace CUWebinars.Web.Models
{
    public class PlayModel
    {
        public Presenter Presenter { get; set; }
        public Webinar Webinar { get; set; }
        public IEnumerable<string> WebinarFiles { get; set; }
    }
}