using System.Collections.Generic;
using CUWebinars.Business.Models;
using CUWebinars.Business.ModelsV4;

namespace CUWebinars.Business.Notification.ViewModel
{
    public class ShowMcCampaignViewModel
    {
        public IEnumerable<McCampaign> McCampaigns { get; set; }
        public Webinar Webinar { get; set; }
        public IEnumerable<Affiliate> Affiliates { get; set; }
    }
}