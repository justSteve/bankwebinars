using System.Collections.Generic;
using CUWebinars.Business.Models;

namespace CUWebinars.Web.ViewModel
{
    public class AddAdditionalLocationViewModel
    {
        public IList<string> Emails { get; set; }
        public AdditionalLocation AdditionalLocation { get; set; }
    }
}