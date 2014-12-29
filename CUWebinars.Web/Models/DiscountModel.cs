using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CUWebinars.Web.Models
{
    public class DiscountModel
    {
        [Required]
        [Display(Name = "Full Name")]
        public string DiscountCode { get; set; }

        [Required]
        [Display(Name = "Reduction Percentage")]
        public decimal PercentOff { get; set; }

        [Required]
        [Display(Name = "Reduction Amount")]
        public decimal FlatOff { get; set; }

        [Required]
        [Display(Name = "Times Used")]
        public int UsesCount { get; set; }

        [Required]
        [Display(Name = "Uses Remaining")]
        public int UsesRemain { get; set; }

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

        [HiddenInput]
        public DiscountType TypeOfDiscount { get; set; }

        [ScaffoldColumn(false)]
        public string ClientScriptActionHint { get; set; }
    }

    public enum DiscountType
    {
        Promotional = 0,
        Compensatory = 1,
        Bronze =2,
        Silver = 3,
        Gold = 4,
        Subscription = 5,
        Annual = 6
    }
}
