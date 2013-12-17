using CUWebinars.Web.Core.DataTables;

namespace CUWebinars.Web.Core.Browsers.Webinars
{
    public class WebinarsBrowserRequestModel : DataTablesRequestModel
    {
        public WebinarsBrowserRequestModel()
        {
        }

        public WebinarsBrowserRequestModel(DataTablesRequestModel dataTablesRequestModel)
            : base(dataTablesRequestModel)
        {
        }

        public bool IncludeUpcoming { get; set; }
        public bool IncludeRecorded { get; set; }
        public bool IncludeArchived { get; set; }
    }
}