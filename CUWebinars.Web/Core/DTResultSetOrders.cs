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
                                (p.Affiliate_ttsDomain != null && p.Affiliate_ttsDomain.ToLower().Contains(search.ToLower()))
                            )
                        ));

                        // we're likely NOT going to have any column specific filters
                        //&& (columnFilters[0] == null || (p.Name != null && p.Name.ToLower().Contains(columnFilters[0].ToLower())))
                        //&& (columnFilters[1] == null || (p.City != null && p.City.ToLower().Contains(columnFilters[1].ToLower())))
                        //&& (columnFilters[2] == null || (p.Postal != null && p.Postal.ToLower().Contains(columnFilters[2].ToLower())))
                        //&& (columnFilters[3] == null || (p.Email != null && p.Email.ToLower().Contains(columnFilters[3].ToLower())))
                        //&& (columnFilters[4] == null || (p.Company != null && p.Company.ToLower().Contains(columnFilters[4].ToLower())))
                        //&& (columnFilters[5] == null || (p.Account != null && p.Account.ToLower().Contains(columnFilters[5].ToLower())))
                        //&& (columnFilters[6] == null || (p.CreditCard != null && p.CreditCard.ToLower().Contains(columnFilters[6].ToLower())))

            return results;
        }

    }
}
