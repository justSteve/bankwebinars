using System.Collections.Generic;
using System.Linq;
using CUWebinars.Business.Models;
using CUWebinars.Business.Repository;

namespace CUWebinars.Business.Services
{
    public class OrderManagementService : IOrderManagementService
    {
        private readonly IOptionRepository _optionRepository;
        private IRefDataRepository _refDataRepository;

        public OrderManagementService(IOptionRepository optionRepository, IRefDataRepository refDataRepository)
        {
            _optionRepository = optionRepository;
            _refDataRepository = refDataRepository;
        }

        public string BuildConnectionInfo(OrderRow orderRow)
        {
            // add code here to build string

            var option = _optionRepository.FindOption((int) orderRow.RegistrationType);

            return string.Empty;
        }

        public IList<Option> GetOptionsByWebinarId(int id)
        {
            return _refDataRepository.FindOptionsByWebinarId(id);
        }
    }
}