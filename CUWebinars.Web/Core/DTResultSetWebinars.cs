using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using CUWebinars.Business.Models;

namespace CUWebinars.Web.Core
{
    // based heavily on https://www.echosteg.com/jquery-datatables-asp.net-mvc5-server-side
    public class DTResultSetWebinars
    {
        public List<SearchDTO> GetResult(string search, string sortOrder, int start, int length, List<SearchDTO> dtResult,
            List<string> columnFilters)
        {
            return FilterResult(search, dtResult, columnFilters).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }

        public int Count(string search, List<SearchDTO> dtResult, List<string> columnFilters)
        {
            return FilterResult(search, dtResult, columnFilters).Count();
        }

        private IQueryable<SearchDTO> FilterResult(string search, List<SearchDTO> dtResult, List<string> columnFilters)
        {
            IQueryable<SearchDTO> results = dtResult.AsQueryable();

            results =
                results.Where(
                    p => (search == null ||
                            (
                                (p.WhoAttend != null && p.WhoAttend.ToLower().Contains(search.ToLower())) ||
                                (p.Title != null && p.Title.ToLower().Contains(search.ToLower())) ||
                                (p.DescriptionLong != null && p.DescriptionLong.ToLower().Contains(search.ToLower())) ||
                                (p.PresenterName != null && p.PresenterName.ToLower().Contains(search.ToLower())) ||
                                (p.RelatedTopicsString != null && p.RelatedTopicsString.ToLower().Contains(search.ToLower()))
                            )
                        ));

            return results;
        }

    }
}
