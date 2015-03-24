using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CUWebinars.Business.Services;

namespace CUWebinars.Business.Notification.Email
{
    public class LocalOrderNotifierMessageDelivery : IOrderConfirmedNotificationDelivery
    {
        public LocalOrderNotifierMessageDelivery()
        {
            
        }

        public void Notify(IConfirmOrderMessage confirmOrderMessage)
        {
            //OrderManagementService oms = (IOrderManagementService)GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(IErrorResponseCommand));
        }
    }
}
