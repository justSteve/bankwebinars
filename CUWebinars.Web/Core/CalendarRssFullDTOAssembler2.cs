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
                ceuShort = "" +  ceu[0];
                ceuStatement = ceu[1];
            }

            var pricing = "";

            if (entity.SeriesInfo == "")
            {
                
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
                          ceuStatement + pricing

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
