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
        private Mock<IWebinarManagementService> _webinarManagementServiceMock;
        private OrderManagementQueryHandlers _orderManagementQueryHandlers;
        private static TestContext _context;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            /* ********************* Note: Can use context to find various directories and test info *********************/
            _context = context;
        }


        [TestInitialize]
        public void TestSetup()
        {
            _membershipServiceMock = new Mock<IMembershipService>();
            _orderManagementServiceMock = new Mock<IOrderManagementService>();
            _webinarManagementServiceMock = new Mock<IWebinarManagementService>();
            _orderManagementQueryHandlers = new OrderManagementQueryHandlers(_membershipServiceMock.Object, _orderManagementServiceMock.Object, _webinarManagementServiceMock.Object);
        }


        [TestMethod]
        [TestCategory(TestCategories.OrderManagementQueries)]
        public void HandleMethodInvokesGetAffiliateById()
        {
            //  Arrange
            int idWebinar = 400;
            int idUserAff = 17;
            string email = "somevalid@emailaddress.com";

            _orderManagementServiceMock.Setup(o => o.GetAffiliateById(idUserAff))
                .Returns(new Affiliate { idUserAff = idUserAff })
                .Verifiable();

            _webinarManagementServiceMock.Setup(o => o.GetWebinar(idWebinar)).Returns(new Webinar {idWebinar = idWebinar});
            _membershipServiceMock.Setup(m => m.GetUserByEmail(email)).Returns(new WebUser {email = email});

            var orderManagementQuery = new OrderManagementQuery
            {
                AffiliateId = idUserAff,
                Email = email,
                WebinarId = idWebinar
            };

            //  Act
            var result = _orderManagementQueryHandlers.Handle(orderManagementQuery);
            
            //  Assert                        
            _orderManagementServiceMock.Verify();

        }

        [TestMethod]
        [TestCategory(TestCategories.OrderManagementQueries)]
        public void HandleMethodInvokesGetUserByEmail()
        {
            //  Arrange
            int idWebinar = 400;
            int idUserAff = 17;
            string email = "somevalid@emailaddress.com";

            _orderManagementServiceMock.Setup(o => o.GetAffiliateById(idUserAff))
                .Returns(new Affiliate { idUserAff = idUserAff });

            _webinarManagementServiceMock.Setup(o => o.GetWebinar(idWebinar))
                .Returns(new Webinar {idWebinar = idWebinar})
                .Verifiable();

            _membershipServiceMock.Setup(m => m.GetUserByEmail(email)).Returns(new WebUser {email = email});

            var orderManagementQuery = new OrderManagementQuery
            {
                AffiliateId = idUserAff,
                Email = email,
                WebinarId = idWebinar
            };

            //  Act
            var result = _orderManagementQueryHandlers.Handle(orderManagementQuery);
            
            //  Assert                        
            _orderManagementServiceMock.Verify();

        }

        [TestMethod]
        [TestCategory(TestCategories.OrderManagementQueries)]
        public void HandleMethodInvokesGetWebinar()
        {
            //  Arrange
            int idWebinar = 400;
            int idUserAff = 17;
            string email = "somevalid@emailaddress.com";

            _orderManagementServiceMock.Setup(o => o.GetAffiliateById(idUserAff))
                .Returns(new Affiliate { idUserAff = idUserAff });

            _webinarManagementServiceMock.Setup(o => o.GetWebinar(idWebinar))
                .Returns(new Webinar {idWebinar = idWebinar});

            _membershipServiceMock.Setup(m => m.GetUserByEmail(email))
                .Returns(new WebUser {email = email})
                .Verifiable();

            var orderManagementQuery = new OrderManagementQuery
            {
                AffiliateId = idUserAff,
                Email = email,
                WebinarId = idWebinar
            };

            //  Act
            var result = _orderManagementQueryHandlers.Handle(orderManagementQuery);
            
            //  Assert                        
            _orderManagementServiceMock.Verify();

        }

        [TestMethod]
        [TestCategory(TestCategories.OrderManagementQueries)]
        public void HandleMethodReturnsQueryResultWithPropertiesPouplated()
        {
            //  Arrange
            int idWebinar = 400;
            int idUserAff = 17;
            string email = "somevalid@emailaddress.com";

            _orderManagementServiceMock.Setup(o => o.GetAffiliateById(idUserAff)).Returns(new Affiliate { idUserAff = idUserAff });
            _webinarManagementServiceMock.Setup(o => o.GetWebinar(idWebinar)).Returns(new Webinar { idWebinar = idWebinar });
            _membershipServiceMock.Setup(m => m.GetUserByEmail(email)).Returns(new WebUser {email = email});

            var orderManagementQuery = new OrderManagementQuery
            {
                AffiliateId = idUserAff,
                Email = email,
                WebinarId = idWebinar
            };

            //  Act
            var result = _orderManagementQueryHandlers.Handle(orderManagementQuery);
            
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
        [TestCategory(TestCategories.OrderManagementQueries)]
        public void HandleMethodThrowsArgumentExceptionWhenPassedNullValue()
        {
            //  Arrange
            OrderManagementQuery orderManagementQuery = null;

            //  Act
            //  Assert                        
            ExceptionAssert.Throws<ArgumentNullException>(
                () => _orderManagementQueryHandlers.Handle(orderManagementQuery)
                );
        }
    }
}

