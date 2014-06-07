using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using BrockAllen.MembershipReboot;
using BrockAllen.MembershipReboot.Ef;
using BrockAllen.MembershipReboot.WebHost;
using CUWebinars.Business.AccountService;
using CUWebinars.Business.Models;
using CUWebinars.Business.Repository;
using CUWebinars.Tests.Common;
using CUWebinars.Web.App_Start;
using CUWebinars.Web.Controllers;
using CUWebinars.Web.Core;
using CUWebinars.Web.Helpers;
using CUWebinars.Web.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject.Extensions.Logging.Log4net.Infrastructure;

namespace CUWebinars.Web.Tests
{
    [TestClass]
    public class OrderControllerIntegrationTests
    {
        private readonly GlobalConfig globalConfig = GlobalConfig.GlobalConfigSingleton;

        //[TestMethod]
        //public void CreateOrder()
        //{
        //    var orderController = new OrderController(TestHelper.CreateMembershipService(),
        //        TestHelper.CreateOrderManagemetService(),
        //        new StateService(),
        //        new Log4NetLogger(typeof (OrderController))
        //        );

        //    orderController.CreateOrder()
        //}

    }
}
