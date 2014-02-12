using System.Collections.Generic;
using BrockAllen.MembershipReboot;
using CUWebinars.Business.Models;

namespace CUWebinars.Business.Services
{
    public interface IOrderManagementService
    {
        string BuildConnectionInfo(OrderRow orderRow);
        IList<Option> GetOptionsByWebinarId(int id);

        void CreateOrderEvent(Order order, UserAccount userAccount);
        void DispatchDummyOrder();
    }
}
