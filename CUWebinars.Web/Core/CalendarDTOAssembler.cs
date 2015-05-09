using System;
using System.Collections.Generic;
using CUWebinars.Business.Models;
using CUWebinars.Web.Infrastructure.Extensions;

namespace CUWebinars.Web.Core
{
    public class CalendarDTOAssembler : IDTOAssembler<CalendarDTO, Webinar>
    {
        public CalendarDTO Entity2DTO(Webinar entity)
        {

            string seoTitle =
                entity.Title.RemoveIllegalCharacters()
                    .ReplaceSpacesWithHyphens()
                    .ReplaceAmpersandsWithAnd()
                    .ToLower()
                    .TrimEnd('.');

            var dto = new CalendarDTO
            {
                id = entity.idWebinar,
                title = entity.Title,
                start = ToUnixTimespan(entity.Date),
                end = ToUnixTimespan(entity.Date.AddHours((double)entity.Duration)),
                url =  "/" + entity.idWebinar + "/" + seoTitle
                //url = "/Webinar/Details/" + entity.idWebinar
            };

            return dto;
        }

        private long ToUnixTimespan(DateTime date)
        {
            //DateTime timeUtc = date;
            //TimeZoneInfo myZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
            //DateTime theTime = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, myZone);
            TimeZoneInfo tzInfo = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
            var convertedTimeToUtc = TimeZoneInfo.ConvertTimeToUtc(date, tzInfo);
 
            //return (long)Math.Truncate(tspan.TotalSeconds);


            TimeSpan tspan = convertedTimeToUtc.Subtract(new DateTime(1970, 1, 1, 0, 0, 0));
            return (long)Math.Truncate(tspan.TotalSeconds);
        }

        public Webinar DTO2Entity(CalendarDTO dto)
        {
            throw new NotImplementedException();
        }

        //
        public IList<CalendarDTO> Entities2DTOs(IList<Webinar> entities)
        {
            var dtos = new List<CalendarDTO>();
            var dtoAssembler = new CalendarDTOAssembler();

            foreach (var webinar in entities)
            {
                dtos.Add(dtoAssembler.Entity2DTO(webinar));
            }

            return dtos;
        }

        public IList<Webinar> DTOs2Entities(IList<CalendarDTO> dtos)
        {
            throw new NotImplementedException();
        }
    }
}
