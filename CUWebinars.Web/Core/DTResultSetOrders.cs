using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using CUWebinars.Business.Models;

namespace CUWebinars.Web.Core
{
    // based heavily on https://www.echosteg.com/jquery-datatables-asp.net-mvc5-server-side
    public class DTResultSetOrders
    {
        public List<OrderDTO> GetResult(string search, string sortOrder, int start, int length, List<OrderDTO> dtResult,
            List<string> columnFilters)
        {
            return FilterResult(search, dtResult, columnFilters).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }

        public int Count(string search, List<OrderDTO> dtResult, List<string> columnFilters)
        {
            return FilterResult(search, dtResult, columnFilters).Count();
        }

        private IQueryable<OrderDTO> FilterResult(string search, List<OrderDTO> dtResult, List<string> columnFilters)
        {
            IQueryable<OrderDTO> results = dtResult.AsQueryable();

            results =
                results.Where(
                    p => (search == null || 
                            (
                                (p.LastName != null && p.LastName.ToLower().Contains(search.ToLower())) ||
                                (p.FirstName != null && p.FirstName.ToLower().Contains(search.ToLower())) ||
                                (p.Institution != null && p.Institution.ToLower().Contains(search.ToLower())) ||
                                (p.idOrder.ToString() == search) ||
                                (p.BillingEmail != null && p.BillingEmail.ToLower().Contains(search.ToLower())) ||
                                (p.Affiliate_ttsDomain != null && p.Affiliate_ttsDomain.ToLower().Contains(search.ToLower()))
                            )
                        ));

            return results;
        }

    }
}
