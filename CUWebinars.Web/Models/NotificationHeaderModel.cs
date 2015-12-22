using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace CUWebinars.Web.Models
{
    public class NotificationHeaderModel
    {
        public string BannerText { get; set; }
        public string HeaderText { get; set; }
        public string TenantLogo { get; set; }
    }
}