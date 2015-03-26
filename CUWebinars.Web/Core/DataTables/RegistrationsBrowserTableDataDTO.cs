using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CUWebinars.Web.Core.DataTables
{
    public class RegistrationsBrowserTableDataDTO
    {
        public int iTotalDisplayRecords { get; set; }
        public int iTotalRecords { get; set; }
        public IList<object> aaData { get; set; }
        public string sEcho { get; set; }
    }
}