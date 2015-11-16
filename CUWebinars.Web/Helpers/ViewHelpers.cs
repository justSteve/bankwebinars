using CUWebinars.Business.Models;
using System.Text;
using System.Web.Mvc;

// from here: http://www.codemag.com/article/1312081
namespace CUWebinars.Web.Helpers
{
    public class ViewHelpers
    {
        public static string RenderViewToString(ControllerContext context, string viewPath, object model = null, bool partial = false)
        {
            // first find the ViewEngine for this view
            ViewEngineResult viewEngineResult = null;
            if (partial)
                viewEngineResult = ViewEngines.Engines.FindPartialView(context, viewPath);
            else
                viewEngineResult = ViewEngines.Engines.FindView(context, viewPath, null);

            if (viewEngineResult == null ||
                viewEngineResult.View == null) // a.s. fixed bug here, viewEngineResult was not null but viewEngineResult.View was
                throw new System.IO.FileNotFoundException("View cannot be found.");

            // get the view and attach the model to view data
            var view = viewEngineResult.View;
            context.Controller.ViewData.Model = model;

            string result = null;

            using (var sw = new System.IO.StringWriter())
            {
                var ctx = new ViewContext(context, view,
                                            context.Controller.ViewData,
                                            context.Controller.TempData,
                                            sw);
                view.Render(ctx, sw);
                result = sw.ToString();
            }

            return result;
        }
    }
}