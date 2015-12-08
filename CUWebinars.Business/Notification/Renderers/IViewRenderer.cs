using System.IO;
using System.Web.Mvc;
using System.Web.Routing;

namespace CUWebinars.Business.Notification.Renderers
{
    public interface IViewRenderer
    {
        string RenderViewToString(string viewPath, object model = null);
        void RenderView(string viewPath, object model, TextWriter writer);
        string RenderPartialViewToString(string viewPath, object model = null);
        void RenderPartialView(string viewPath, object model, TextWriter writer);
    }
}
