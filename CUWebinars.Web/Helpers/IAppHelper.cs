using System.Collections.Generic;
using System.Web.Mvc;
using CUWebinars.Business.Models;

namespace CUWebinars.Web.Helpers
{
    public interface IAppHelper
    {
        /// <summary>
        /// Retrieves the city state zip via URL parameter and stored in the ASP.NET Session
        /// </summary>
        /// <returns>City | State based on input zipcode</returns>
        string GetCityStateFromZip(int zipCode);

        SelectList  GetListOfAffiliates(int selectedValue);
        SessionStartInfo GetSessionStartInfo();
        string GetUserAuditInfo();
        List<string> InstitutionAutoComplete(string name, string zip);
        string GetAffiliateName(int idAffiliate);
        IList<string> ServerSideEmailCheck(IList<string> emails);
        IEnumerable<AdditionalLocation> CheckAdditionalLocationsForValidEmail(IEnumerable<AdditionalLocation> additionalLocations);
        bool CheckIsEmailValid(string email);
    }
}