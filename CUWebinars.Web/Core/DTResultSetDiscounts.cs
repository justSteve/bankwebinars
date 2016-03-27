using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using CUWebinars.Business.Models;
using CUWebinars.Web.Models;

namespace CUWebinars.Web.Core
{
    public class DTResultSetDiscounts
    {
        public List<DiscountDTO> GetResult(string search, string sortOrder, int start, int length, List<DiscountDTO> dtResult,
            List<string> columnFilters)
        {
            return FilterResult(search, dtResult, columnFilters).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }

        public int Count(string search, List<DiscountDTO> dtResult, List<string> columnFilters)
        {
            return FilterResult(search, dtResult, columnFilters).Count();
        }

        private IQueryable<DiscountDTO> FilterResult(string search, List<DiscountDTO> dtResult, List<string> columnFilters)
        {
            IQueryable<DiscountDTO> results = dtResult.AsQueryable();

            results =
                results.Where
                (p => (search == null ||
                    (p.DiscountCode!= null && p.DiscountCode.ToLower().Contains(search.ToLower())) 
                    
                        ));

            return results;
        }

    }
}
