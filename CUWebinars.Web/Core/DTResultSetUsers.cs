using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using CUWebinars.Business.Models;

namespace CUWebinars.Web.Core
{
    public class DTResultSetUsers
    {
        public List<UserDTO> GetResult(string search, string sortOrder, int start, int length, List<UserDTO> dtResult,
            List<string> columnFilters)
        {
            return FilterResult(search, dtResult, columnFilters).SortBy(sortOrder).Skip(start).Take(length).ToList();
        }

        public int Count(string search, List<UserDTO> dtResult, List<string> columnFilters)
        {
            return FilterResult(search, dtResult, columnFilters).Count();
        }

        private IQueryable<UserDTO> FilterResult(string search, List<UserDTO> dtResult, List<string> columnFilters)
        {
            IQueryable<UserDTO> results = dtResult.AsQueryable();

            results =
                results.Where
                (p => (search == null ||
                    (p.LastName != null && p.LastName.ToLower().Contains(search.ToLower()))||
                    (p.email != null && p.email.ToLower().Contains(search.ToLower()))
                    //(p.Institution != null && p.BillingEmail.ToLower().Contains(search.ToLower()))

                        ));

            return results;
        }

    }
}
