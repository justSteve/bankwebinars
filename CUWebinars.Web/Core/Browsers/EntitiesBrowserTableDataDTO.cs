using System.Collections.Generic;

namespace CUWebinars.Web.Core.Browsers
{
    /// <summary>
    /// Common DTO to deliver data to DataTables.
    /// It will be serialized into JSON and will be recognized by DataTables as a response to search/paging request.
    /// Fields are named based on DataTables requirements: http://www.datatables.net/usage/server-side
    /// </summary>
    public class EntitiesBrowserTableDataDTO
    {
        /// <summary>
        /// Total records, after filtering (i.e. the number of records after filtering has been applied)
        /// </summary>
        public int iTotalDisplayRecords { get; set; }

        /// <summary>
        /// Total records, before filtering (i.e. the number of records in the database)
        /// </summary>
        public int iTotalRecords { get; set; }

        /// <summary>
        /// The data in a 2D array
        /// </summary>
        public IList<object> aaData { get; set; }

        /// <summary>
        /// An unaltered copy of sEcho sent from the client side
        /// </summary>
        public string sEcho { get; set; }
    }
}