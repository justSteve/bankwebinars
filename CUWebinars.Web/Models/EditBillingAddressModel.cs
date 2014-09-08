using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using CUWebinars.Business.Models;

namespace CUWebinars.Web.Models
{
    public class EditBillingAddressModel
    {

        [UIHint("Address")]
        public AddressModel BillingAddress { get; set; }

        [HiddenInput]
        public string AccountDetailsTitle { get; set; }

    }
}