using CUWebinars.Business.Repository;
using CUWebinars.Web.Infrastructure.Extensions;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace CUWebinars.Web.Controllers
{
    public class NavigationController : Controller
    {
        private const string YadaYadaYada = "...";
        private readonly IWebinarRepository _webinarRepository;

        public NavigationController(IWebinarRepository webinarRepository)
        {
            _webinarRepository = webinarRepository;
        }

        [OutputCache(Duration = 3600)]
        public string UpComingWebinarMenu()
        {
            var upcomingWebinars = _webinarRepository.GetUpcoming().OrderBy(w => w.Date).Take(8).ToList();
            var upComingPresentationListItems = new StringBuilder();
            upComingPresentationListItems.Append(
                "<li role='presentation'><a  role=\"menuitem\" tabindex=\"-1\"  href='/Webinar/allActive/?eventsToShow=upcoming'>View <b>All</b> Upcoming Events</a></li>"
                );

            if (upcomingWebinars.Count > 0)
            {
                for (int i = 0; i < upcomingWebinars.Count; i++)
                {
                    var upcomingWebinar = upcomingWebinars[i];

                    string shortTitle = upcomingWebinar.Title.Length > 35
                        ? upcomingWebinar.Title.Substring(0, 35) + YadaYadaYada
                        : upcomingWebinar.Title;

                    string seoTitle =
                        upcomingWebinar.Title.RemoveIllegalCharacters()
                            .ReplaceSpacesWithHyphens()
                            .ReplaceAmpersandsWithAnd()
                            .ToLower()
                            .TrimEnd('.');

                    upComingPresentationListItems.Append(
                        string.Concat(string.Format(
                            "<li role=\"presentation\"><a role=\"menuitem\" tabindex=\"-1\" href='/{0}/{1}'",
                            upcomingWebinar.idWebinar,
                            seoTitle
                            ),
                            upcomingWebinars[i].idWebinar + "'>" + Server.HtmlEncode(shortTitle) + "</a></li>")
                        );
                }

            }
            return upComingPresentationListItems.ToString();
        }

        [OutputCache(Duration = 3600)]
        public string GetRecordedWebinarMenu()
        {
            var recordedWebinars = _webinarRepository.GetRecorded().OrderBy(w => w.Date).Take(8).ToList();

            var recordedWebinarsListItems = new StringBuilder();

            recordedWebinarsListItems.Append(
                "<li role='presentation'><a  role=\"menuitem\" tabindex=\"-1\"  href='/Webinar/allActive/?eventsToShow=recorded'>View <b>All</b> Recordings</a></li>"
                );

            if (recordedWebinars.Count > 0)
            {
                for (int i = 0; i < 8; i++)
                {
                    var recordedWebinar = recordedWebinars[i];

                    string shortTitle =
                        recordedWebinar.Title.Length > 45
                            ? recordedWebinar.Title.Substring(0, 45) + YadaYadaYada
                            : recordedWebinar.Title;

                    string seoTitle =
                        recordedWebinar.Title.RemoveIllegalCharacters()
                            .ReplaceSpacesWithHyphens()
                            .ReplaceAmpersandsWithAnd()
                            .ToLower()
                            .TrimEnd('.');

                    recordedWebinarsListItems.Append(
                        string.Concat(string.Format(
                            "<li role=\"presentation\"><a role=\"menuitem\" tabindex=\"-1\" href='/{0}/{1}'",
                            recordedWebinar.idWebinar,
                            seoTitle
                            ),
                            recordedWebinars[i].idWebinar + "'>" + Server.HtmlEncode(shortTitle) + "</a></li>"));

                    //recordedWebinarsListItems.Append(
                    //    "<li role='presentation'><a role=\"menuitem\" tabindex=\"-1\" href='/Webinar/Details/" +
                    //    recordedWebinar.idWebinar + "'>" + Server.HtmlEncode(ShortTitle) + "</a></li>"
                    //    );
                }
            }

            return recordedWebinarsListItems.ToString();
        }
    }
}