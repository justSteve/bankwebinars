using CUWebinars.Business.AccountService;
using CUWebinars.Business.CQS.Queries;
using CUWebinars.Business.CQS.QueryHandlers;
using CUWebinars.Business.Models;
using CUWebinars.Business.Services;
using CUWebinars.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;

namespace CUWebinars.Business.Tests.UnitTests
{
    [TestClass]
    public class OrderManagementQueriesTests
    {
        private Mock<IMembershipService> membershipService;
        private Mock<IOrderManagementService> orderManagementService;
        private OrderManagementQueries orderManagementQueries;
        private static TestContext _context;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            _context = context;
        }


        [TestInitialize]
        public void TestSetup()
        {
            membershipService = new Mock<IMembershipService>();
            orderManagementService = new Mock<IOrderManagementService>();
            orderManagementQueries = new OrderManagementQueries(membershipService.Object, orderManagementService.Object);
        }


        [TestMethod]
        public void HandleMethodReturnsQueryResultWithPropertiesPouplated()
        {
            //  Arrange
            int idWebinar = 400;
            int idUserAff = 17;
            string daveDaveCom = "dave@dave.com";

            orderManagementService.Setup(o => o.GetAffiliateById(idUserAff)).Returns(new Affiliate { idUserAff = idUserAff });
            orderManagementService.Setup(o => o.GetWebinar(idWebinar)).Returns(new Webinar {idWebinar = idWebinar});
            membershipService.Setup(m => m.GetUserByEmail(daveDaveCom)).Returns(new WebUser {email = daveDaveCom});

            var orderManagementQuery = new OrderManagementQuery
            {
                AffiliateId = idUserAff,
                Email = daveDaveCom,
                WebinarId = idWebinar
            };

            //  Act
            var result = orderManagementQueries.Handle(orderManagementQuery);
            
            //  Assert                        
            Assert.IsNotNull(result.Affiliate);
            Assert.IsNotNull(result.Webinar);
            Assert.IsNotNull(result.WebUser);

            /* ********************* Can use context to find various directories and test info *********************
            Trace.WriteLine(_context.TestName);
            Trace.WriteLine(_context.TestDir);
            Trace.WriteLine(_context.TestResultsDirectory);
            Trace.WriteLine(_context.TestRunDirectory);
            Trace.WriteLine(_context.TestRunResultsDirectory);
            */
        }

        [TestMethod]
        public void HandleMethodThrowsArgumentExceptionWhenPassedNullValue()
        {
            //  Arrange
            OrderManagementQuery orderManagementQuery = null;

            //  Act
            //  Assert                        
            ExceptionAssert.Throws<ArgumentNullException>(
                () => orderManagementQueries.Handle(orderManagementQuery)
                );

        }
    }
}

