using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CUWebinars.Business.Models;

namespace CUWebinars.Web.Models
{
    public class DiscountModel
    {
        [Required]
        [Display(Name = "Discount Code")]
        public string DiscountCode { get; set; }

        [Required]
        [Display(Name = "Reduction Percentage")]
        public decimal PercentOff { get; set; }

        [Required]
        [Display(Name = "Reduction Amount")]
        public decimal FlatOff { get; set; }

        [Required]
        [Display(Name = "Credits Used")]
        public decimal CreditsUsed { get; set; }

        [Required]
        [Display(Name = "Credits Remaining")]
        public decimal CreditsRemain { get; set; }
        
        [Required]
        [Display(Name = "Date Verified")]
        public DateTime DateVerified { get; set; }

        [Required]
        [Display(Name = "Date Started")]
        public DateTime DateValidFrom { get; set; }

        [Required]
        [Display(Name = "Expiration Date")]
        public DateTime DateValidTo { get; set; }

        [Required]
        public string Status { get; set; }

        public DateTime? DateBilled { get; set; }

        public decimal? Cost { get; set; }

        [Required]
        public string Notes { get; set; }

        [Required]
        public int RenewalTerm { get; set; }

    
        [Required]
        public decimal TotalCount { get; set; }

        [HiddenInput]
        public int idDiscount { get; set; }
        
        [HiddenInput]
        public DiscountType TypeOfDiscount { get; set; }
        
        [HiddenInput]
        public WebUserDiscountXref WebUserDiscountXref { get; set; }

        //[ScaffoldColumn(false)]
        //public string ClientScriptActionHint { get; set; }
    }


}
