using System;
using System.Collections.ObjectModel;
using System.IO;
using CUWebinars.Business.Core;
using CUWebinars.Business.Models;
using CUWebinars.Business.Repository;
using CUWebinars.Business.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject.Extensions.Logging.Log4net.Infrastructure;
using System.Data.Entity;
using System.Linq;

namespace CUWebinars.Business.Tests
{
    [TestClass]
    public class OrderManagementServiceIntegrationTests
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
            databaseSetup.UninstallDatabase(Constants.DbName);
        }


        [TestMethod]
        public void SaveOrderChangesWhenUserAlreadyExistsDoesNotIncludeWelcomeLinkInEmail()
        {
            //  Arrange
            var ctx = new TTSWebinarsContext(Constants.LocalDbConnectionStringName);
            var orderManagementService = new OrderManagementService(
                new AffiliateRepository(ctx),
                new RegTypeRepository(ctx), 
                new OrderRepository(ctx),
                new RefDataRepository(),
                new WebUserRepository(ctx), 
                new WebinarRepository(ctx),
                new Log4NetLogger(typeof(OrderManagementService)),
                TtsConfig.Create(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\.."))
                    );

            var webUser = ctx.WebUsers.Include(w => w.Institution).First(w => w.idUser == 26368);
            var webinar = ctx.Webinars.First(w => w.idWebinar == 404);
            var affiliate = ctx.Affiliates.First(w => w.idUserAff == 19);

            var orderRow = new OrderRow
            {
                AdditionalLocation = new Collection<AdditionalLocation>(),
                idOrder = 2,
                idRegType = 88,
                UnitPrice = 395.00M,
                RegistrationType = new RegType{ idRegType = 79, OptionExplain = "Admission to all five events in this series.", OptionLabel = "Live Only - 5 Part Series", Price = 1095D },
                RowPrice = 395.00M,
                Royalty = 0.00M,
                RowStatus = OrderRowStatus.Active,
                Webinar = webinar
            };


            var orderRepository = new OrderRepository(ctx);

            //  Act
            var newOrder = orderRepository.CreateOrder(affiliate, webUser, webinar, orderRow);

            BusinessTestHelper.PopulateOrder(newOrder, webUser);

            newOrder = orderManagementService.SaveOrderChanges(newOrder, "bfghsdjkfds", string.Empty);


        }
    }
}
