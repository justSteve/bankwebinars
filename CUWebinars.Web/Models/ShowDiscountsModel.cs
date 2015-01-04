using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using CUWebinars.Business.Models;
using Microsoft.VisualBasic.ApplicationServices;

namespace CUWebinars.Web.Models
{
    public class ShowDiscountsModel
    {
        public User User { get; set; }
        public Discount Discount { get; set; }
    }
}