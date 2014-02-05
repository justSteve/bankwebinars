using System.Collections.Generic;
using CUWebinars.Business.Models;

namespace CUWebinars.Business.Services
{
    public interface IOrderManagementService
    {
        string BuildConnectionInfo(OrderRow orderRow);
        IList<Option> GetOptionsByWebinarId(int id);
    }
}
