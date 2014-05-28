using CUWebinars.Business.Models;
using CUWebinars.Business.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Entity;
using System.Linq;

namespace CUWebinars.Business.Tests
{
    [TestClass]
    public class OrderRepositoryIntegrationTests
    {
        [TestInitialize]
        public void SetUp()
        {
            var databaseSetup = new DatabaseSetup();
            databaseSetup.InstallDatabase(Constants.CreateDbDefault);
        }

        [TestCleanup]
        public void TearDown()
        {
            var databaseSetup = new DatabaseSetup();
            databaseSetup.UninstallDatabase();
        }


        [TestMethod]
        [TestCategory("Integration Tests")]
        public void CreateOrderCreatesNewOrder()
        {
            //  Arrange
            var ctx = new TTSWebinarsContext("CUWebinarsSUTLocal");
            var webUser = ctx.WebUsers.First(w => w.idUser == 26368);
            var webinar = ctx.Webinars.First(w => w.idWebinar == 404);
            var affiliate = ctx.Affiliates.First(w => w.idUserAff == 19);

            var orderRow = new OrderRow
            {
                idOrder = 2,
                idRegType = 88,
                UnitPrice = 395.00M,
                RowPrice = 395.00M,
                Royalty = 0.00M,
                RowStatus = OrderRowStatus.Active,
                Webinar = webinar
            };


            var orderRepository = new OrderRepository(ctx);

            //  Act
            var newOrder = orderRepository.CreateOrder(affiliate, webUser, webinar, orderRow);

            var savedOrder = ctx.Orders.FirstOrDefault(o => o.idOrder == newOrder.idOrder); //  Check Save succeeded

            //  Assert                        
            Assert.IsNotNull(savedOrder);
        }

        [TestMethod]
        [TestCategory("Integration Tests")]
        public void SaveOrderChangesAddsDetailsToNewOrder()
        {
            //  Arrange
            var ctx = new TTSWebinarsContext("CUWebinarsSUTLocal");
            var webUser = ctx.WebUsers.Include(w => w.Institution).First(w => w.idUser == 26368);
            var webinar = ctx.Webinars.First(w => w.idWebinar == 404);
            var affiliate = ctx.Affiliates.First(w => w.idUserAff == 19);

            var orderRow = new OrderRow
            {
                idOrder = 2,
                idRegType = 88,
                UnitPrice = 395.00M,
                RowPrice = 395.00M,
                Royalty = 0.00M,
                RowStatus = OrderRowStatus.Active,
                Webinar = webinar
            };


            var orderRepository = new OrderRepository(ctx);
            
            var newOrder = orderRepository.CreateOrder(affiliate, webUser, webinar, orderRow);

            PopulateOrder(newOrder, webUser);

            //  Act
            newOrder = orderRepository.SaveOrderChanges(newOrder);

            var savedOrder = ctx.Orders.FirstOrDefault(o => o.idOrder == newOrder.idOrder); //  Check Save succeeded

            //  Assert                        
            Assert.IsNotNull(savedOrder.OrderStatus == OrderStatus.Submitted);
        }

        private static void PopulateOrder(Order newOrder, WebUser webUser)
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

        //var order = new Order
        //{
        //    AdminComments = "incomingOrderModel.AdminComments",
        //    AffiliateComments = "Affiliate comments",
        //    FirstName = "Alan",
        //    LastName = "Turing",
        //    idAffiliate = 19,
        //    idUser = 26368,
        //    OrderDate = DateTime.UtcNow,
        //    Institution = "Some Institution",
        //    BillingZip = "5000",
        //    BillingState = "Tx",
        //    BillingCity = "Del Rio",
        //    BillingAddress = "968 Wildcat Dr",
        //    BillingPhone = "555-555-5555",
        //    ShippingZip = "5000",
        //    ShippingState = "Tx",
        //    ShippingCity = "Del Rio",
        //    ShippingAddress = "968 Wildcat Dr",
        //    ShippingPhone = "555-555-5555",
        //    TaxExempt = false,
        //    Total = 395.00M,
        //    Origin = "incomingOrderModel.Origin",
        //    UserComments = "incomingOrderModel.UserComments"
        //};

    }
}
