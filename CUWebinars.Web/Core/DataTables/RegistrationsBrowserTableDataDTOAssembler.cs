using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CUWebinars.Web.Core.DataTables
{
    public class RegistrationsBrowserTableDataDTOAssembler : AbstractDTOAssembler<RegistrationsBrowserTableDataDTO, RegistrationsBrowserSearchResultDTO>
    {
        private readonly string _echoId;

        public RegistrationsBrowserTableDataDTOAssembler(string echoId)
        {
            _echoId = echoId;
        }

        public override RegistrationsBrowserTableDataDTO Entity2DTO(RegistrationsBrowserSearchResultDTO entity)
        {
            var rowsData = new List<object>();

            foreach (var order in entity.Orders)
            {
                rowsData.Add(new List<object>
                {
                    order.idOrder,
                    new
                        {
                            order.OrderDate,
                            order.WebUser,
                            Institution = order.Institution== null ? String.Empty : order.Institution
                        },
                    order.Origin,
                    order.OrderDate.ToString("MM/dd/yy"),
                    string.Empty
                });
            }

            return new RegistrationsBrowserTableDataDTO
            {
                iTotalDisplayRecords = entity.FoundCount,
                iTotalRecords = entity.TotalCount,
                aaData = rowsData,
                sEcho = _echoId
            };
        }

        public override RegistrationsBrowserSearchResultDTO DTO2Entity(RegistrationsBrowserTableDataDTO dto)
        {
            throw new NotImplementedException();
        }
    }
}