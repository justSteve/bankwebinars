using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CUWebinars.Business.Models.Mapping
{

    public partial class WebUserLegacy
    {
        public WebUserLegacy()
        {
            Addresses = new List<Address>();
            Orders = new List<Order>();
        }

        public int idUser { get; set; }
        public int idUserLegacy { get; set; }
        public UserType UserType { get; set; }
        public string AcctStatus { get; set; }
        public System.DateTime DateCreated { get; set; }
        public string FirstName { get; set; }
        public string FullName
        {
            get { return FirstName + " " + LastName; }
        }
        public string LastName { get; set; }
        public string Initial { get; set; }
        public int idUserInstitution { get; set; }
        public string email { get; set; }
        public string futureMail { get; set; }
        public string generalComments { get; set; }
        public Nullable<bool> taxExempt { get; set; }
        public Nullable<int> idSubscriptionDiscount { get; set; }
        public USTimeZone timeZone { get; set; }
        public string Title { get; set; }
        public virtual ICollection<Address> Addresses { get; set; }
        public virtual Affiliate Affiliate { get; set; }
        public virtual Institution Institution { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual Presenter Presenter { get; set; }
        public WebUserDiscountXref WebUserDiscountXref { get; set; }
    }

}
