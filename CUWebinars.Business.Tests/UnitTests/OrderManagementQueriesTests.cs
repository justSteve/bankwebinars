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
        private Mock<IMembershipService> _membershipServiceMock;
        private Mock<IOrderManagementService> _orderManagementServiceMock;
        private OrderManagementQueries _orderManagementQueries;
        private static TestContext _context;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            _context = context;
        }


        [TestInitialize]
        public void TestSetup()
        {
            _membershipServiceMock = new Mock<IMembershipService>();
            _orderManagementServiceMock = new Mock<IOrderManagementService>();
            _orderManagementQueries = new OrderManagementQueries(_membershipServiceMock.Object, _orderManagementServiceMock.Object);
        }


        [TestMethod]
        public void HandleMethodReturnsQueryResultWithPropertiesPouplated()
        {
            //  Arrange
            int idWebinar = 400;
            int idUserAff = 17;
            string email = "somevalid@emailaddress.com";

            _orderManagementServiceMock.Setup(o => o.GetAffiliateById(idUserAff)).Returns(new Affiliate { idUserAff = idUserAff });
            _orderManagementServiceMock.Setup(o => o.GetWebinar(idWebinar)).Returns(new Webinar {idWebinar = idWebinar});
            _membershipServiceMock.Setup(m => m.GetUserByEmail(email)).Returns(new WebUser {email = email});

            var orderManagementQuery = new OrderManagementQuery
            {
                AffiliateId = idUserAff,
                Email = email,
                WebinarId = idWebinar
            };

            //  Act
            var result = _orderManagementQueries.Handle(orderManagementQuery);
            
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
                () => _orderManagementQueries.Handle(orderManagementQuery)
                );
        }
    }
}

