using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CUWebinars.Business.Models;
using CUWebinars.Web.Helpers;
using CUWebinars.Web.Infrastructure.Extensions;
using HtmlAgilityPack;

namespace CUWebinars.Web.Core
{
    public class CalendarRssFullDTOAssembler2 : IDTOAssembler<CalendarRssDTO, Webinar>
    {

        GlobalConfig _globalConfig = GlobalConfig.GlobalConfigSingleton;
        public CalendarRssDTO Entity2DTO(Webinar entity)
        {

            string seoTitle =
                entity.Title.RemoveIllegalCharacters()
                    .ReplaceSpacesWithHyphens()
                    .ReplaceAmpersandsWithAnd()
                    .ToLower()
                    .TrimEnd('.');
            var sb = new StringBuilder();
            HtmlAgilityPack.HtmlDocument doc = new HtmlDocument();

            doc.LoadHtml(entity.Presenter.BiographyLong);

            var node = doc.DocumentNode.FirstChild;
            foreach (var _node in node.ChildNodes)
            {

                if (_node.Name != "img" && _node.Name != "a")
                {
                    sb.Append(_node.InnerHtml);
                }
            }

            var ceuShort = string.Empty;
            var ceuStatement = string.Empty;

            if (!string.IsNullOrEmpty(entity.ceu))
            {
                string[] ceu = entity.ceu.Split('|');
                ceuShort = "" + ceu[0];
                ceuStatement = ceu[1];
            }

            var pricing = "";
            // to update pricing values run sproc
            // EXEC BuildRegTypesForJson @RegTypeGroup = 51


            if (entity.SeriesInfo.Contains("children"))
            {
                var numInSeries = entity.SeriesInfo.Split(',').Length;
                switch (numInSeries)
                {
                    case 1:
                        pricing =
                            "[{\"label\":\"PreEvent Series2\",\"id\":\"249_" + entity.idWebinar + "\",\"RegTypeLabel\":\"Live Plus Five\",\"RegTypeExplain\":\"Attend the live events and receive five business days of unlimited access to the OnDemand Playback of each session and links to presenter materials and supplementary handouts.\",\"Price\":\"485.0\"},{\"label\":\"PreEvent Series2\",\"id\":\"250_" + entity.idWebinar + "\",\"RegTypeLabel\":\"OnDemand Recording\",\"RegTypeExplain\":\"Receive unlimited access to each OnDemand playback for 6 months and links to presenter materials and supplementary handouts. This option does not include live session attendance.\",\"Price\":\"535.0\"},{\"label\":\"PreEvent Series2\",\"id\":\"253_" + entity.idWebinar + "\",\"RegTypeLabel\":\"CD-ROM\",\"RegTypeExplain\":\"Receive each webinar recording on a CD-ROM 7-10 business days after the Live event or your registration date (whichever is the latter). Receive unlimited access to the OnDemand Playback for 6 months and links to presenter materials and supplementary handouts. This option does not include live session attendance.\",\"Price\":\"625.0\"},{\"label\":\"PreEvent Series2\",\"id\":\"251_" + entity.idWebinar + "\",\"RegTypeLabel\":\"Live Plus Six\",\"RegTypeExplain\":\"Attend the live events and receive six months of unlimited access to each OnDemand Playback and links to presenter materials and supplementary handouts.\",\"Price\":\"665.0\"},{\"label\":\"PreEvent Series2\",\"id\":\"252_" + entity.idWebinar + "\",\"RegTypeLabel\":\"Premier Package\",\"RegTypeExplain\":\"Includes the three base options:  Live attendance, OnDemand Playback for six months, and the CD-ROM of each session.\",\"Price\":\"715.0\"}]";
                        break;
                    case 2:
                        pricing =
                            "[{\"label\":\"PreEvent Series3\",\"id\":\"214_" + entity.idWebinar + "\",\"RegTypeLabel\":\"Premier Package\",\"RegTypeExplain\":\"Includes the three base options:  Live attendance, OnDemand Playback for six months, and the CD-ROM of each session.\",\"Price\":\" 1075\"},{\"label\":\"PreEvent Series3\",\"id\":\"210_" + entity.idWebinar + "\",\"RegTypeLabel\":\"Live Plus Five\",\"RegTypeExplain\":\"Attend the live events and receive five business days of unlimited access to the OnDemand Playback of each session and links to presenter materials and supplementary handouts.\",\"Price\":\"725.0\"},{\"label\":\"PreEvent Series3\",\"id\":\"211_" + entity.idWebinar + "\",\"RegTypeLabel\":\"OnDemand Recording\",\"RegTypeExplain\":\"Receive unlimited access to each OnDemand playback for 6 months and links to presenter materials and supplementary handouts. This option does not include live session attendance.\",\"Price\":\"805.0\"},{\"label\":\"PreEvent Series3\",\"id\":\"213_" + entity.idWebinar + "\",\"RegTypeLabel\":\"CD-ROM\",\"RegTypeExplain\":\"Receive each webinar recording on a CD-ROM 7-10 business days after the Live event or your registration date (whichever is the latter). Receive unlimited access to the OnDemand Playback for 6 months and links to presenter materials and supplementary handouts. This option does not include live session attendance.\",\"Price\":\"945.0\"},{\"label\":\"PreEvent Series3\",\"id\":\"212_" + entity.idWebinar + "\",\"RegTypeLabel\":\"Live Plus Six\",\"RegTypeExplain\":\"Attend the live events and receive six months of unlimited access to each OnDemand Playback and links to presenter materials and supplementary handouts.\",\"Price\":\"995.0\"}]";
                        break;
                    case 3:
                        pricing =
                            "[{\"label\":\"PreEvent Series4\",\"id\":\"217_" + entity.idWebinar + "\",\"RegTypeLabel\":\"OnDemand Recording\",\"RegTypeExplain\":\"Receive unlimited access to each OnDemand playback for 6 months and links to presenter materials and supplementary handouts. This option does not include live session attendance.\",\"Price\":\" 1065\"},{\"label\":\"PreEvent Series4\",\"id\":\"219_" + entity.idWebinar + "\",\"RegTypeLabel\":\"CD-ROM\",\"RegTypeExplain\":\"Receive each webinar recording on a CD-ROM 7-10 business days after the Live event or your registration date (whichever is the latter). Receive unlimited access to the OnDemand Playback for 6 months and links to presenter materials and supplementary handouts. This option does not include live session attendance.\",\"Price\":\" 1255\"},{\"label\":\"PreEvent Series4\",\"id\":\"218_" + entity.idWebinar + "\",\"RegTypeLabel\":\"Live Plus Six\",\"RegTypeExplain\":\"Attend the live events and receive six months of unlimited access to each OnDemand Playback and links to presenter materials and supplementary handouts.\",\"Price\":\" 1315\"},{\"label\":\"PreEvent Series4\",\"id\":\"220_" + entity.idWebinar + "\",\"RegTypeLabel\":\"Premier Package\",\"RegTypeExplain\":\"Includes the three base options:  Live attendance, OnDemand Playback for six months, and the CD-ROM of each session.\",\"Price\":\" 1425\"},{\"label\":\"PreEvent Series4\",\"id\":\"216_" + entity.idWebinar + "\",\"RegTypeLabel\":\"Live Plus Five\",\"RegTypeExplain\":\"Attend the live events and receive five business days of unlimited access to the OnDemand Playback of each session and links to presenter materials and supplementary handouts.\",\"Price\":\"995.0\"}]";
                        break;

                    case 4:
                        pricing =
                            "[{\"label\":\"PreEvent Series5\",\"id\":\"221_" + entity.idWebinar + "\",\"RegTypeLabel\":\"Live Plus Five\",\"RegTypeExplain\":\"Attend the live events and receive five business days of unlimited access to the OnDemand Playback of each session and links to presenter materials and supplementary handouts.\",\"Price\":\" 1145\"},{\"label\":\"PreEvent Series5\",\"id\":\"222_" + entity.idWebinar + "\",\"RegTypeLabel\":\"OnDemand Recording\",\"RegTypeExplain\":\"Receive unlimited access to each OnDemand playback for 6 months and links to presenter materials and supplementary handouts. This option does not include live session attendance.\",\"Price\":\" 1245\"},{\"label\":\"PreEvent Series5\",\"id\":\"224_" + entity.idWebinar + "\",\"RegTypeLabel\":\"CD-ROM\",\"RegTypeExplain\":\"Receive each webinar recording on a CD-ROM 7-10 business days after the Live event or your registration date (whichever is the latter). Receive unlimited access to the OnDemand Playback for 6 months and links to presenter materials and supplementary handouts. This option does not include live session attendance.\",\"Price\":\" 1485\"},{\"label\":\"PreEvent Series5\",\"id\":\"223_" + entity.idWebinar + "\",\"RegTypeLabel\":\"Live Plus Six\",\"RegTypeExplain\":\"Attend the live events and receive six months of unlimited access to each OnDemand Playback and links to presenter materials and supplementary handouts.\",\"Price\":\" 1555\"},{\"label\":\"PreEvent Series5\",\"id\":\"225_" + entity.idWebinar + "\",\"RegTypeLabel\":\"Premier Package\",\"RegTypeExplain\":\"Includes the three base options:  Live attendance, OnDemand Playback for six months, and the CD-ROM of each session.\",\"Price\":\" 1745\"}]";
                        break;

                }
            }
            else
            {
                if (entity.Duration == 1)
                    pricing =
                        "[{\"label\":\"PreEvent 1hr_17\",\"id\":\"200_" + entity.idWebinar + "\",\"RegTypeLabel\":\"Live Plus Five\",\"RegTypeExplain\":\"Attend the live event and  receive five business days of unlimited access to the OnDemand Playback and links to presenter materials and supplementary handouts.\",\"Price\":\"165.0\"},{\"label\":\"PreEvent 1hr_17\",\"id\":\"201_" + entity.idWebinar + "\",\"RegTypeLabel\":\"OnDemand Recording\",\"RegTypeExplain\":\"Receive unlimited access to the OnDemand Playback for 6 months and links to presenter materials and supplementary handouts. This option does not include live session attendance.\",\"Price\":\"185.0\"},{\"label\":\"PreEvent 1hr_17\",\"id\":\"203_" + entity.idWebinar + "\",\"RegTypeLabel\":\"Live Plus Six\",\"RegTypeExplain\":\"Attend the live event and receive six months of unlimited access to the OnDemand Playback and links to presenter materials and supplementary handouts.\",\"Price\":\"235.0\"},{\"label\":\"PreEvent 1hr_17\",\"id\":\"265_" + entity.idWebinar + "\",\"RegTypeLabel\":\"CD-ROM\",\"RegTypeExplain\":\"Receive the webinar recording on a CD-ROM 7-10 business days after the Live event or your registration date (whichever is the latter). Receive unlimited access to the OnDemand Playback for 6 months and links to presenter materials and supplementary handouts. This option does not include live session attendance.\",\"Price\":\"235.0\"},{\"label\":\"PreEvent 1hr_17\",\"id\":\"204_" + entity.idWebinar + "\",\"RegTypeLabel\":\"Premier Package\",\"RegTypeExplain\":\"Includes all three base options.  Live attendance, OnDemand Playback for six months, and the CD-ROM.\",\"Price\":\"265.0\"}]";
                else
                {
                    pricing =
                        "[{\"label\":\"PreEvent 2hr_17\",\"id\":\"260_" + entity.idWebinar + "\",\"RegTypeLabel\":\"Live Plus Five\",\"RegTypeExplain\":\"Attend the live event and  receive five business days of unlimited access to the OnDemand Playback and links to presenter materials and supplementary handouts.\",\"Price\":\"265.0\"},{\"label\":\"PreEvent 2hr_17\",\"id\":\"261_" + entity.idWebinar + "\",\"RegTypeLabel\":\"OnDemand Recording\",\"RegTypeExplain\":\"Receive unlimited access to the OnDemand Playback for 6 months and links to presenter materials and supplementary handouts. This option does not include live session attendance.\",\"Price\":\"295.0\"},{\"label\":\"PreEvent 2hr_17\",\"id\":\"263_" + entity.idWebinar + "\",\"RegTypeLabel\":\"CD-ROM \",\"RegTypeExplain\":\"Receive the webinar recording on a CD-ROM 7-10 business days after the Live event or your registration date (whichever is the latter). Receive unlimited access to the OnDemand Playback for 6 months and links to presenter materials and supplementary handouts. This option does not include live session attendance.\",\"Price\":\"345.0\"},{\"label\":\"PreEvent 2hr_17\",\"id\":\"262_" + entity.idWebinar + "\",\"RegTypeLabel\":\"Live Plus Six\",\"RegTypeExplain\":\"Attend the live event and receive six months of unlimited access to the OnDemand Playback and links to presenter materials and supplementary handouts.\",\"Price\":\"365.0\"},{\"label\":\"PreEvent 2hr_17\",\"id\":\"264_" + entity.idWebinar + "\",\"RegTypeLabel\":\"Premier Package\",\"RegTypeExplain\":\"Includes all three base options.  Live attendance, OnDemand Playback for six months, and the CD-ROM.\",\"Price\":\"395.0\"}]";
                }
            }

            var dto = new CalendarRssDTO
            {
                id = entity.idWebinar,
                Title = entity.Title,
                start = ToUnixTimespan(entity.Date),
                end = ToUnixTimespan(entity.Date.AddHours((double)entity.Duration)),
                Url = _globalConfig.TenantURL + "/" + entity.idWebinar + "/" + seoTitle,
                Description = entity.Title,
                EventDate = entity.Date,
                PublishedDate = entity.Date.ToShortDateString(),
                Content = "<div id=WebinarDesc>" + entity.DescriptionLong + "</div>" +
                          "<div id=\"presenter\"><b>Presenter: </b>" + sb.ToString() + "</div>" +
                          "<div id=\"learn\"><b>" + entity.LearnCaption + "</b>" + entity.LearnBody + "</div>" +
                          "<div id=\"whoattend\"><b>Who Should Attend: </b>" + entity.WhoAttend + "</div>" +
                          "<div id=\"ceu\">" + ceuStatement + "</div>" +
                          "<div style=\"display: none;\" id=\"pricing\">" + pricing.Replace(".0", "") + "</div>"

            };

            return dto;
        }

        private long ToUnixTimespan(DateTime date)
        {
            TimeZoneInfo tzInfo = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
            var convertedTimeToUtc = TimeZoneInfo.ConvertTimeToUtc(date, tzInfo);

            TimeSpan tspan = convertedTimeToUtc.Subtract(new DateTime(1970, 1, 1, 0, 0, 0));
            return (long)Math.Truncate(tspan.TotalSeconds);
        }

        public Webinar DTO2Entity(CalendarRssDTO dto)
        {
            throw new NotImplementedException();
        }

        //
        public IList<CalendarRssDTO> Entities2DTOs(IList<Webinar> entities)
        {
            var dtos = new List<CalendarRssDTO>();
            var dtoAssembler = new CalendarRssFullDTOAssembler2();

            foreach (var webinar in entities)
            {
                dtos.Add(dtoAssembler.Entity2DTO(webinar));
            }

            return dtos;
        }

        public IList<Webinar> DTOs2Entities(IList<CalendarRssDTO> dtos)
        {
            throw new NotImplementedException();
        }
    }
}
