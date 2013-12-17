using System;

namespace CUWebinars.Web.Core.DataTables
{
    public class DataTablesRequestModel
    {
        public int DisplayStart { get; set; }
        public int DisplayLength { get; set; }
        public string Search { get; set; }
        public string EchoId { get; set; }

        public int SortColumn { get; set; }
        public String SortDirection { get; set; }

        public DataTablesRequestModel()
        {

        }

        public DataTablesRequestModel(DataTablesRequestModel dataTablesRequestModel)
        {
            DisplayLength = dataTablesRequestModel.DisplayLength;
            DisplayStart = dataTablesRequestModel.DisplayStart;
            EchoId = dataTablesRequestModel.EchoId;
            Search = dataTablesRequestModel.Search;
            SortColumn = dataTablesRequestModel.SortColumn;
            SortDirection = dataTablesRequestModel.SortDirection;
        }
    }
}