using System.Web.Mvc;
using CUWebinars.Web.Core.DataTables;

namespace CUWebinars.Web.Core.Browsers.Webinars
{
    public class WebinarsBrowserRequestModelBinder : DataTablesRequestModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var httpRequest = controllerContext.HttpContext.Request;
            var dataTablesRequest = base.BindModel(controllerContext, bindingContext) as DataTablesRequestModel;
            var webinarsBrowserRequest = new WebinarsBrowserRequestModel(dataTablesRequest);

            if (controllerContext.HttpContext.Session != null)
            {
                if (controllerContext.HttpContext.Session["IncludeRecorded"] != null &&
                    controllerContext.HttpContext.Session["IncludeRecorded"].ToString() == "True")
                {
                    webinarsBrowserRequest.IncludeRecorded = true;
                }

                if (controllerContext.HttpContext.Session["IncludeUpcoming"] != null &&
                    controllerContext.HttpContext.Session["IncludeUpcoming"].ToString() == "True")
                {
                    webinarsBrowserRequest.IncludeUpcoming = true;
                }
            }

            //support for legacy admin search components
            //if (httpRequest.FilePath == "/admin/webinars/webinarstabledata")
            //{
            bool tmp;

            if (bool.TryParse(httpRequest["bIncludeUpcoming"], out tmp))
                webinarsBrowserRequest.IncludeUpcoming = tmp;

            if (bool.TryParse(httpRequest["bIncludeRecorded"], out tmp))
                webinarsBrowserRequest.IncludeRecorded = tmp;

            if (bool.TryParse(httpRequest["bIncludeArchived"], out tmp))
                webinarsBrowserRequest.IncludeArchived = tmp;

            //}

            //bool tmp;
            if (httpRequest["searchFor"] != null)
            {
                //if (httpRequest["searchFor"].Split('=')[] )
                webinarsBrowserRequest.Search = httpRequest["searchFor"].ToString();
            }
            if (httpRequest["includeInSearch"] != null)
            {

                switch (httpRequest["includeInSearch"])
                {
                    case "Upcoming":
                        webinarsBrowserRequest.IncludeUpcoming = true;
                        webinarsBrowserRequest.IncludeRecorded = false;
                        webinarsBrowserRequest.IncludeArchived = false;
                        break;
                    case "On-Demand":
                        webinarsBrowserRequest.IncludeUpcoming = false;
                        webinarsBrowserRequest.IncludeRecorded = true;
                        webinarsBrowserRequest.IncludeArchived = false;
                        break;
                    default:
                        webinarsBrowserRequest.IncludeUpcoming = true;
                        webinarsBrowserRequest.IncludeRecorded = true;
                        webinarsBrowserRequest.IncludeArchived = false;
                        break;
                }

            }

            return webinarsBrowserRequest;
        }
    }
}