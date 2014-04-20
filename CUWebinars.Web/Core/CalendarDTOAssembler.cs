using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CUWebinars.Business.Models;
using CUWebinars.Web.Core;

namespace CUWebinars.Web.Core
{
    public class CalendarDTOAssembler : IDTOAssembler<CalendarDTO, Webinar>
    {
        public CalendarDTO Entity2DTO(Webinar entity)
        {
            //if (idAffiliate != null)
            //{


            //}
            var dto = new CalendarDTO
            {
                id = entity.idWebinar,
                title = entity.Title,
                start = ToUnixTimespan(entity.Date),
                end = ToUnixTimespan(entity.Date.AddHours((double)entity.Duration)),
                url = "/Webinar/Details/" + entity.idWebinar
            };

            return dto;
        }

        private long ToUnixTimespan(DateTime date)
        {
            TimeSpan tspan = date.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0));

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
