using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CUWebinars.Business.Core;
using CUWebinars.Business.Models;

namespace CUWebinars.Business.Notification
{
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
