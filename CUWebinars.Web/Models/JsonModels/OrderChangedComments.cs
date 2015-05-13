using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CUWebinars.Web.Models.JsonModels
{
    public class OrderChangedComments
    {
        public int OrderId { get; set; }
        public int UserIdOfEditor { get; set; }
        public DateTime ChangeDate { get; set; }
        public String Comments { get; set; }
    }
}