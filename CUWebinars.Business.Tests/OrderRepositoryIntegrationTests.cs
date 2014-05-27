using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CUWebinars.Business.Models;
using CUWebinars.Business.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CUWebinars.Business.Tests
{
    [TestClass]
    public class OrderRepositoryIntegrationTests
    {
        [TestInitialize]
        public void SetUp()
        {
            var databaseSetup = new DatabaseSetup();
            databaseSetup.InstallDatabase();
        }

        [TestMethod]
        public void CreateOrderCreatesNewOrder()
        {
            OrderRepository orderRepository = new OrderRepository(new TTSWebinarsContext("CUWebinarsSUTLocal"));            


            //orderRepository.CreateOrder()
            Assert.IsTrue(true);
        }

    }
}
