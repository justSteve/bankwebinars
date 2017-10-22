using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CUWebinars.Business.Models;

namespace CUWebinars.Web.Models
{
    public class PayTraceResponse
    {

        public string Orderid { get; set; }
        public string Transactionid { get; set; }
        public string Appcode { get; set; }
        public string Appmsg { get; set; }
        public string Avsresponse { get; set; }
        public string Cscresponse { get; set; }
        public string Email { get; set; }
        public string Amount { get; set; }
        public string Bname { get; set; }
        public Order Order { get; set; }
        public string CartType { get; set; }
    }
}