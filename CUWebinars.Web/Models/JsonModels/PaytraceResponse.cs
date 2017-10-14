using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CUWebinars.Web.Models.JsonModels
{
    public class PaytraceResponse
    {
        public class TxResponse
        {
            public string Orderid { get; set; }
            public string Transactionid { get; set; }
            public string Appcode { get; set; }
            public string Appmsg { get; set; }
            public object Avsresponse { get; set; }
            public object Cscresponse { get; set; }
            public object Email { get; set; }
            public string Amount { get; set; }
            public string Bname { get; set; }
            public object Order { get; set; }
            public string CartType { get; set; }
        }

        public class PaidByCc
        {
            public int idOrder { get; set; }
            public int idUser { get; set; }
            public object OrderList { get; set; }
            public int idAffiliate { get; set; }
            public string TimeStamp { get; set; }
            public string AuditInfo { get; set; }
            public string SingleOrMulti { get; set; }
            public string ValidateResponse { get; set; }
            public string ValidateRequest { get; set; }
            public TxResponse TxResponse { get; set; }
            public string TxRequest { get; set; }
        }
    }
}