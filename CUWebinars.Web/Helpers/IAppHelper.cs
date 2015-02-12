using System.Collections.Generic;

namespace CUWebinars.Web.Helpers
{
    public interface IAppHelper
    {
        /// <summary>
        /// Retrieves the city state zip via URL parameter and stored in the ASP.NET Session
        /// </summary>
        /// <returns>City | State based on input zipcode</returns>
        string GetCityStateFromZip(int zipCode);

        SessionStartInfo GetSessionStartInfo();
        string GetUserAuditInfo();
        List<string> InstitutionAutoComplete(string name, string zip);
    }
}