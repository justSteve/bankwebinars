using CUWebinars.Business.Services;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace CUWebinars.Web.Helpers
{
    public static class EventInvokerHelpers
    {
        public static IEnumerable<SelectListItem> GetRegTypesForWebinarAsSelectListItems(int idWebinar, IOrderManagementService orderManagementService)
        {
            return orderManagementService.FindRegTypesByWebinarId(idWebinar).Select(r => new SelectListItem { Text = r.OptionLabel, Value = r.idRegType.ToString() });
        }


        public static IEnumerable<SelectListItem> GetUpcomingWebinarsAsSelectListItems(IOrderManagementService orderManagementService)
        {
            return orderManagementService.GetUpcomingWebinars()
                .OrderBy(w => w.Date)
                .Select(w => new SelectListItem { Text = w.Title, Value = w.idWebinar.ToString() });
        }

        public static IEnumerable<SelectListItem> GetRecordedWebinarsAsSelectListItems(IOrderManagementService orderManagementService)
        {
            return orderManagementService.GetRecordedWebinars().Select(w => new SelectListItem { Text = w.Title, Value = w.idWebinar.ToString() });
        }

        public static IEnumerable<SelectListItem> GetShippedWebinarsAsSelectListItems(IOrderManagementService orderManagementService)
        {
            return orderManagementService.GetOrdersForShippedNotification().Select(w => new SelectListItem { Text = w.idOrder.ToString(), Value = w.idOrder.ToString() });
        }

    }
}