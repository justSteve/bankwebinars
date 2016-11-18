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
//         123456
// 62279788
// TAS456
// ++NO++MATCH++++++%2D+Approved+and+completed
// No+Match
// Match
// test%40test%2Ecom
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
    }
}