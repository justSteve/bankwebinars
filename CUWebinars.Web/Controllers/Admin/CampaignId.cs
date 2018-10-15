using Newtonsoft.Json;

namespace CUWebinars.Web.Controllers.Admin
{
    public class CampaignId
    {
        [JsonProperty("id")]
        public string id { get; set; }
    }
}