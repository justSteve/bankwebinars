using System.Web.Mvc;
using CUWebinars.Web.Core.DataTables;

namespace CUWebinars.Web.Core.Browsers.Webinars
{
    public class WebinarsBrowserRequestModelBinderAttribute : DataTablesRequestModelBinderAttribute
    {
        public override IModelBinder GetBinder()
        {
            return new WebinarsBrowserRequestModelBinder();
        }
    }
}