using CUWebinars.Business.Models;

namespace CUWebinars.Business.Notification.ViewModel
{
    public class PostEventPublishModel
    {
        public Order Order { get; set; }
        public decimal CostFor6month { get; set; }
        public decimal CostForCD { get; set; }
        //public OrderGenesis OrderGenesis { get; set; }
        //public bool UserCreatedInCart { get; set; }
        //public bool UserCreatedOnImport { get; set; }
    }
}