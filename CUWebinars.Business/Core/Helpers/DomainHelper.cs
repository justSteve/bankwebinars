using CUWebinars.Business.Models;
using CUWebinars.Business.Repository;
using System;
using System.Linq;

namespace CUWebinars.Business.Core.Helpers
{
    public class DomainHelper : IDomainHelper
    {
        private readonly RegTypeRepository _regTypeRepository;

        public DomainHelper(RegTypeRepository regTypeRepository)
        {
            _regTypeRepository = regTypeRepository;
        }

        public DateTime GetPostEventMaterialsAccessExpiry(Order order)
        {
            if (order == null) throw new ArgumentNullException("order");

            var orderRow = order.OrderRows.Single(or => or.RowStatus == OrderRowStatus.Active);
            // let exception be thrown if there is not a single 

            var regType = _regTypeRepository.FindRegType(orderRow.idRegType);

            if (regType.ShowRecordingNotifications.Equals("yes", StringComparison.OrdinalIgnoreCase))
                return DateTime.Today.AddMonths(6);

            return DateTime.Today.AddDays(5);

        }
    }
}
