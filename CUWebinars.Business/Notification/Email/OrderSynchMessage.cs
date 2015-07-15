using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CUWebinars.Business.Core;
using CUWebinars.Business.Models;

namespace CUWebinars.Business.Notification.Email
{
    public class OrderSynchMessage : IOrderSynchMessage
    {
        public string AddPasswordUrl { get; set; }
        public string BaseUrl { get; set; }
        public string ConfirmChangeEmailUrl { get; set; }
        public string Details { get; set; }
        public int idOrder { get; set; }
        public Order Order { get; set; }
        public OrderGenesis OrderGenesis { get; set; }
        public string PersistedName { get; set; }
        public bool UserCreatedInCart { get; set; }
        public bool UserCreatedOnImport { get; set; }
    }

    public interface IOrderSynchMessage
    {
        string AddPasswordUrl { get; set; }
        string BaseUrl { get; set; }
        string ConfirmChangeEmailUrl { get; set; }
        string Details { get; set; }
        int idOrder { get; set; }
        Order Order { get; set; }
        OrderGenesis OrderGenesis { get; set; }
        string PersistedName { get; set; }
        bool UserCreatedInCart { get; set; }
        bool UserCreatedOnImport { get; set; }
    }
}
