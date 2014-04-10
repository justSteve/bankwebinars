using System;
using System.Collections.Generic;

namespace CUWebinars.Business.Models
{
    public partial class Address : IObjectWithState
    {
        public int Id { get; set; }
        public string AddressType { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string StreetAddress { get; set; }
        public string StreetAddress2 { get; set; }
        public string City { get; set; }
        public string Zip { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        //public Nullable<int> WebUser_Id { get; set; }
        public int idUser { get; set; }
        public virtual WebUser WebUser { get; set; }
        public State DomainEntityState { get; set; }
        public Dictionary<string, object> OriginalValues { get; set; }
    }
}
