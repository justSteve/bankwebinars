using System.Web.Mvc;

namespace CUWebinars.Web.Core.DataTables
{
    public class DataTablesRequestModelBinder : IModelBinder
    {
        public virtual object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var dataTablesRequest = new DataTablesRequestModel();
            var request = controllerContext.HttpContext.Request;

            int tmp;

            if (int.TryParse(request["iDisplayStart"], out tmp))
                dataTablesRequest.DisplayStart = tmp;

            if (int.TryParse(request["iDisplayLength"], out tmp))
                dataTablesRequest.DisplayLength = tmp;

            dataTablesRequest.Search = request["sSearch"];
            dataTablesRequest.EchoId = request["sEcho"];

            if (!string.IsNullOrEmpty(request["iSortCol_0"]))
            {
                dataTablesRequest.SortColumn = int.Parse(request["iSortCol_0"]);
                dataTablesRequest.SortDirection = request["sSortDir_0"];
            }

            return dataTablesRequest;
        }
    }
}