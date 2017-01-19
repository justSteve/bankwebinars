using System;
using System.Collections.Generic;
using CUWebinars.Business.Models;
using CUWebinars.Web.Infrastructure.Extensions;

namespace CUWebinars.Web.Core
{
    public class CalendarRssFullDTOAssembler : IDTOAssembler<CalendarRssDTO, Webinar>
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

            var dto = new CalendarRssDTO
            {
                id = entity.idWebinar,
                Title = entity.Title,
                start = ToUnixTimespan(entity.Date),
                end = ToUnixTimespan(entity.Date.AddHours((double)entity.Duration)),
                Url = _globalConfig.TenantURL + "/" + entity.idWebinar + "/" + seoTitle,
                Description = "BWRss",
                EventDate = entity.Date,
                PublishedDate = entity.Date.ToShortDateString(),
                Content = "<div id=\"date\">" + entity.Date.ToShortDateString() + " - " + entity.Date.ToShortTimeString() + "</div><div id=\"presenter\">"
                + entity.Presenter.BiographyLong + "</div><div id=WebinarDec>" + entity.Description + "</div>"

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
            var dtoAssembler = new CalendarRssFullDTOAssembler();

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
