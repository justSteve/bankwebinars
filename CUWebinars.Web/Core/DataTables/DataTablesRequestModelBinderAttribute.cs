using System.Web.Mvc;

namespace CUWebinars.Web.Core.DataTables
{
    public class DataTablesRequestModelBinderAttribute : CustomModelBinderAttribute
    {
        public override IModelBinder GetBinder()
        {
            return new DataTablesRequestModelBinder();
        }
    }
}