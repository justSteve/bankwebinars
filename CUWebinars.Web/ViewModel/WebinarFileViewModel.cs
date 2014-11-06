using CUWebinars.Business.Models;
using System.Collections.Generic;

namespace CUWebinars.Web.ViewModel
{
    public class WebinarFileViewModel
    {
        public int idWwebinar { get; set; }
        public IEnumerable<WebinarFile> WebinarFiles { get; set; }
    }
}