using System;
using System.Security.Cryptography.X509Certificates;

namespace CUWebinars.Web.Models
{
    internal class PaytraceRequestToQueueModel
    {
        public int idOrder { get; set; }
        public int idUser { get; set; }
        public string[] OrderList { get; set; }
        public int idAffiliate { get; set; }

        public String TimeStamp { get; set; }
        public string AuditInfo { get; set; }
        public string SingleOrMulti { get; set; }
        public string ValidateResponse { get; set; }
        public string ValidateRequest { get; set; }
        public string TxResponse { get; set; }
        public string TxRequest { get; set; }
    }
}