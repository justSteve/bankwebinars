using CUWebinars.Business.CQS.Queries;
using CUWebinars.Business.Models;
using CUWebinars.Web.Models;

namespace CUWebinars.Web.Core.Orchestrators
{
    public interface IOrderControllerOrchestrator
    {
        int CreateNewOrder(IncomingOrderModel incomingOrderModel, 
            string email, 
            OrderManagementQueryResult orderManagementQueryResult, 
            string verificationKey, 
            string confirmChangeEmailUrl
            );
        void FinalizeNewRegistration(IncomingOrderModel incomingOrderModel, string verificationKey);
        OrderManagementQueryResult GetData(IncomingOrderModel incomingOrderModel, string email);
        WebUser ProcessNewUser(IncomingOrderModel incomingOrderModel, string email);

    }
}
