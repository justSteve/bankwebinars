using CUWebinars.Web.Infrastructure.Attributes;
using CUWebinars.Web.ViewModel;
using System.Linq;
using System.Web.Mvc;

namespace CUWebinars.Web.Infrastructure.Extensions
{
    public static class ControllerExtensions
    {
        public static ActionResult ModelStateJson(this Controller controller, ModelStateDictionary state)
        {
            var errorsDictionary = state.Where(s => s.Value.Errors.Count > 0)
                .ToDictionary(s => s.Key, s => s.Value.Errors.Select(e => e.ErrorMessage)
                    .JoinAsString()
                );

            return controller.ModelStateJson(false, "There have been some errors.", errorsDictionary);
        }

        public static ActionResult ModelStateJson(this IController controller, bool isSuccessful, string message = "", object data = null)
        {
            return new JsonNetResult { Data = new JsonResultViewModel<object>(isSuccessful, message) { Data = data } };
        }
    }
}