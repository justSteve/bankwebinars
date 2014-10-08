
using System.Collections.Generic;
using CUWebinars.Business.Models;

namespace CUWebinars.Web.ViewModel
{
    public class DisplayOptionsInDropDownViewModel
    {
        public IEnumerable<RegType> Options { get; set; }
        public int OrderRowId { get; set; }
        public RegType OrderRowRegistrationType { get; set; }
    }
}