using System;

namespace CUWebinars.Web.Models
{
    public class ConnInfoSendModel
    {
        public string Sender { get; internal set; }
        public string URL { get; internal set; }
        public DateTime SendDate { get; set; }
    }
}