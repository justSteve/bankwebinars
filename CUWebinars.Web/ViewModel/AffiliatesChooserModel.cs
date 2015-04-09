using System.Collections.Generic;
using System.Web.Mvc;

namespace CUWebinars.Web.ViewModel
{
    public class AffiliatesChooserModel
    {
        public int SelectedAffiliate { get; set; }
        public IEnumerable<SelectListItem> AffiliatesList { get; set; }
    }
}