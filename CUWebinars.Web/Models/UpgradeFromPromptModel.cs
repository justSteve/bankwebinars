using System;

namespace CUWebinars.Web.Models
{
    internal class UpgradeFromPromptModel
    {
        public int idOrder { get; set; }
        public int idRegOriginal { get; set; }
        public int idRegUpgrade { get; set; }
        public int idUser { get; set; }
        public int idAffiliate { get; set; }

        public String TimeStamp { get; set; }
        public string AuditInfo { get; set; }
    }
}