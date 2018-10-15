using System.Collections.Generic;

namespace CUWebinars.Web.Controllers.Admin
{
    public class InvoiceLog
    {
        public string ActiveInvoice { get; set; }
        public int idWebinar { get; set; }
        public string idGTW { get; set; }
        public Dictionary<string, string> User_AffState { get; set; }
        public int idOrder { get; set; }
        public string InvoiceHistory { get; set; }
    }
}