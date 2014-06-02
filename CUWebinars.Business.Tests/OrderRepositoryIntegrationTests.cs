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
            var databaseSetup = new DatabaseSetup {ConnectionString = Globals.LocalDbConnectionString};
            databaseSetup.InstallDatabase(Constants.CreateDbDefault);
        }

        [TestCleanup]
        public void TearDown()
        {
            var databaseSetup = new DatabaseSetup { ConnectionString = Globals.LocalDbConnectionString };
            databaseSetup.UninstallDatabase(Constants.DbName);
        }


        [TestMethod]
        [TestCategory(TestCategories.OrderRepositoryIntegration )]
        public void CreateOrderCreatesNewOrder()
        {
            //  Arrange
            var ctx = new TTSWebinarsContext(Constants.LocalDbConnectionStringName);
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
        [TestCategory(TestCategories.OrderRepositoryIntegration )]
        public void SaveOrderChangesAddsDetailsToNewOrder()
        {
            //  Arrange
            var ctx = new TTSWebinarsContext(Constants.LocalDbConnectionStringName);
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

            BusinessTestHelper.PopulateOrder(newOrder, webUser);

            //  Act
            newOrder = orderRepository.SaveOrderChanges(newOrder);

            var savedOrder = ctx.Orders.FirstOrDefault(o => o.idOrder == newOrder.idOrder); //  Check Save succeeded

            //  Assert                        
            Assert.IsNotNull(savedOrder.OrderStatus == OrderStatus.Submitted);
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
