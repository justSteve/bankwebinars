using System;

namespace CUWebinars.Web.Models.JsonModels
{
    public class OrderChangedComments
    {
        public int OrderId { get; set; }
        public int UserIdOfEditor { get; set; }
        public DateTime ChangeDate { get; set; }
        public string Comments { get; set; }
    }
}