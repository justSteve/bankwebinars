using System.Web.Mvc;
using CUWebinars.Web.Core.Browsers.Webinars;

namespace CUWebinars.Web.Core.DataTables
{
    public class WebinarsBrowserRequestModelBinderAttribute : DataTablesRequestModelBinderAttribute
    {
        public override IModelBinder GetBinder()
        {
            return new WebinarsBrowserRequestModelBinder();
        }
    }
}