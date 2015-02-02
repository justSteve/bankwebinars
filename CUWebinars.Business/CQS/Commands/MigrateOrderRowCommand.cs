using System;
using System.Collections.Generic;
using CUWebinars.Business.Models;

namespace CUWebinars.Business.CQS.Commands
{
    public class MigrateOrderRowCommand
    {
        public string AdditionalLocationsString { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public int RegistrationType { get; set; }
        public Webinar Webinar { get; set; }
        public string Discount { get; set; }
        public DateTime OrderDate { get; set; }
        // output property
        public OrderRow OrderRow { get; internal set; }
    }
}