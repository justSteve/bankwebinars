using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CUWebinars.Business.Core;
using CUWebinars.Business.Models;
using CUWebinars.Business.Repository;
using CUWebinars.Business.Services;
using CUWebinars.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CUWebinars.Business.Tests.IntegrationTests
{
    [TestClass]
    public class OrderManagementServiceIntegrationTests
    {

        [TestMethod]
        public void TestMethod()
        {
            var ctx = new TTSWebinarsContext();

            //  Arrange
            var orderManagementService = new OrderManagementService(
                new AffiliateRepository(ctx),
                new RegTypeRepository(ctx),
                new OrderRepository(ctx),
                new RefDataRepository(),
                new WebUserRepository(ctx),
                new WebinarRepository(ctx),
                new AdditionalLocationRepository(ctx),
                new Ninject.Extensions.Logging.Log4net.Infrastructure.Log4NetLogger(typeof (OrderManagementService)),
                TtsConfig.Create(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, TestConstants.UpTwoFolders),
                    Globals.UseAzureWebjobs, Globals.StorageAccountName, Globals.StorageAccessKey));


            //  Act
            orderManagementService.CreateRegistrantKey("John", "Hancock", "showmesomeandroid-test1@yahoo.com.au", 1722,
                "127193450");

            //  Assert                        

        }

    }
}
