using System.Collections.Generic;
using CUWebinars.Business.Models;

namespace CUWebinars.Business.CQS.Commands
{
    public class MigrateOrderRowCommand
    {
        public IList<IncomingAdditionalLocation> AdditionalLocations { get; set; }
        public string Email { get; set; }
        public int RegistrationType { get; set; }
        public Webinar Webinar { get; set; }

        // output property
        public OrderRow OrderRow { get; internal set; }
    }
}