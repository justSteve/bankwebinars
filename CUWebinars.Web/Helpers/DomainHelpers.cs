using CUWebinars.Business.Models;
using System.Text;

namespace CUWebinars.Web.Helpers
{
    public class DomainHelpers
    {
        public static string BuildAdditionalLocationsCaption(OrderRow orderRow)
        {
            var caption = new StringBuilder(20);

            foreach (var additionalLocation in orderRow.AdditionalLocation)
            {
                caption.AppendFormat("{0},", additionalLocation.Email);
            }

            return caption.Length > 0 ? caption.ToString().Substring(0, caption.Length - 1) : string.Empty;
        }
    }
}