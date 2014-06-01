using CUWebinars.Business.Models;

namespace CUWebinars.Business.Tests
{
    public class BusinessTestHelper
    {
        public static void PopulateOrder(Order newOrder, WebUser webUser)
        {
            newOrder.AdminComments = "incomingOrderModel.AdminComments";
            newOrder.AffiliateComments = "AffiliateComments";
            newOrder.UserComments = "incomingOrderModel.UserComments";
            newOrder.Origin = "incomingOrderModel.Origin";
            newOrder.FirstName = webUser.FirstName;
            newOrder.LastName = webUser.LastName;
            newOrder.Institution = webUser.Institution.InstitutionName;
            newOrder.BillingEmail = webUser.email;

            newOrder.BillingAddress = "968 Wildcat Dr";
            newOrder.BillingAddress2 = null;
            newOrder.BillingPhone = "555-555-5555";
            newOrder.BillingCity = "Del Rio";
            newOrder.BillingState = "Tx";
            newOrder.BillingZip = "5000";

            newOrder.ShippingAddress = "968 Wildcat Dr";
            newOrder.ShippingAddress2 = null;
            newOrder.ShippingPhone = "555-555-5555";
            newOrder.ShippingCity = "Del Rio";
            newOrder.ShippingState = "Tx";
            newOrder.ShippingZip = "5000";
            newOrder.ShippingFirstName = "Alan";
            newOrder.ShippingLastName = "Turing";
        }
    }
}