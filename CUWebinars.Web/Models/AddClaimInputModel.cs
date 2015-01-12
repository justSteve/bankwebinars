using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace CUWebinars.Web.Models
{
    public class AddClaimInputModel
    {
        [DisplayName(".NET ClaimTypes")]
        public IEnumerable<SelectListItem> ClaimTypes { get; set; }
        [DisplayName("Custom ClaimType")]
        public string CustomClaimType { get; set; }
        [Required]
        [HiddenInput]
        public string NewClaimType { get; set; }
        [Required]
        [DisplayName("Value of New Claim")]
        public string NewClaimValue { get; set; }
        public string SelectedClaimType { get; set; }
    }
}