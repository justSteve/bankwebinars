using System.Data.Entity;
using System.Linq;
using CUWebinars.Business.Models;
using CUWebinars.Business.Repository;
using CUWebinars.Business.Tests.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CUWebinars.Business.Tests.IntegrationTests
{
    [TestClass]
    public class OrderRepositoryIntegrationTests
    {
        [TestMethod]
        [TestCategory(TestCategories.OrderRepositoryIntegration )]
        public void CreateOrderCreatesNewOrder()
        {
            var databaseResources = new DatabaseResources();
            databaseResources.PrimeMembershipTestsDatabases();
            databaseResources.CreateCuWebinarsDb();

            //  Arrange
            var ctx = new TTSWebinarsContext(Constants.LocalDbConnectionStringName);
            var webUser = ctx.WebUsers.First(w => w.idUser == 10563);
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
    }
}
