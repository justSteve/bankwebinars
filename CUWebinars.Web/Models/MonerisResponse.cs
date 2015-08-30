using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CUWebinars.Web.Models
{
    public class MonerisResponse
    {

        public string FormId { get; set; }
        public string order_no { get; set; }
        public string ref_num { get; set; }
        public string message { get; set; }
        public string result { get; set; }
        public string auth_code { get; set; }
        public string txn_time { get; set; }
        public string txn_date { get; set; }
        public string response_code { get; set; }
        public string note { get; set; }
    }
}