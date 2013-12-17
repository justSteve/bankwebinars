using System.Collections.Generic;

namespace CUWebinars.Business.Models
{
    public partial class Institution
    {
        public Institution()
        {
            this.WebUsers = new List<WebUser>();
        }

        public int idInstitution { get; set; }
        public string InstitutionName { get; set; }
        public string InstitutionType { get; set; }
        public string domainName { get; set; }
        public string RegIdentifier { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public virtual ICollection<WebUser> WebUsers { get; set; }
    }
}
