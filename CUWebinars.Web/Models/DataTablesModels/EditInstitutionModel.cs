using System.Collections.Generic;
using CUWebinars.Business.Models;

namespace CUWebinars.Web.Models.DataTablesModels
{
    public class EditInstitutionModel
    {
        public int idInstitution { get; set; }
        public int idOfCurrentUser { get; set; }
        public string InstitutionName { get; set; }
        public string InstitutionType { get; set; }
        public string domainName { get; set; }
        public string RegIdentifier { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public virtual ICollection<WebUser> WebUsers { get; set; }
        
    }
}