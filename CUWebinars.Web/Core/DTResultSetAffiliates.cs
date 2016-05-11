using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using CUWebinars.Business.Models;
using CUWebinars.Web.Models;

namespace CUWebinars.Web.Core
{
    // based heavily on https://www.echosteg.com/jquery-datatables-asp.net-mvc5-server-side
    public class DTResultSetAffiliates
    {
        public List<AffiliateReportDTO> GetResult(string search, string sortOrder, int start, int length, List<AffiliateReportDTO> dtResult,
            List<string> columnFilters)
        {
            return FilterResult(search, dtResult, columnFilters).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }

        public int Count(string search, List<AffiliateReportDTO> dtResult, List<string> columnFilters)
        {
            return FilterResult(search, dtResult, columnFilters).Count();
        }

        private IQueryable<AffiliateReportDTO> FilterResult(string search, List<AffiliateReportDTO> dtResult, List<string> columnFilters)
        {
            IQueryable<AffiliateReportDTO> results = dtResult.AsQueryable();

            results =
                results.Where(
                    p => (search == null ||
                            (
                                (p.Affiliate.ttsDomain != null && p.Affiliate.ttsDomain.ToLower().Contains(search.ToLower()))
                            )
                        ));

            return results;
        }

    }
}
