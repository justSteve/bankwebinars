using CUWebinars.Business.Core;
using CUWebinars.Business.Models;

namespace CUWebinars.Web.ViewModel
{
    public class PayTraceViewModel
    {

        public PricesAndDiscounts PricesAndDiscounts { get; set; }
        //, , , , , , , EMAIL, , INVOICE, and  
        public decimal TaxAmount { get; set; }
        public string DESCRIPTION { get; set; }
        public int idOrder { get; set; }
        public string EMAIL { get; set; }
        public string BNAME { get; set; }
        public string Institution { get; set; }
        public string BADDRESS { get; set; }
        public string BADDRESS2 { get; set; }
        public string BCITY { get; set; }
        public string BSTATE { get; set; }
        public string BZIP { get; set; }
        public string BCOUNTRY { get; set; }
        public string PHONE { get; set; }
    }
}