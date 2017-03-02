using System.Web.Management;
using CUWebinars.Business.Models;

namespace CUWebinars.Web.ViewModel
{
    public class ShareNotisViewModel
    {
        public string addresses { get; set; }
        public int idOrder { get; set; }
        public NotiType NotiType { get; set; }
    }

    public enum NotiType
    {
        Billing = 0,
        CC = 1,
        Cert = 2,
        PostEvent = 3,
        Promo = 4,
        Unknown = 255
    }
}