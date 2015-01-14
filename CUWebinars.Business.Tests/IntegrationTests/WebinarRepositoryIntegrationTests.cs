using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CUWebinars.Business.Models;
using CUWebinars.Business.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CUWebinars.Business.Tests.IntegrationTests
{
    [TestClass]
    public class WebinarRepositoryIntegrationTests
    {
        private const string WebinarTitle = "10 Lessons Learned When Your Member Dies";
        private const string TopicDescription = "Operations";
        private const string PresenterLastName = "Crawford";
        private const string WebinarTitleOfWebinarWithOperationsDesc = "How to Use Business Resolutions and Authorizations to Protect Your Financial Institution";
        private WebinarRepository webinarRepository;

        [TestInitialize]
        public void SetUp()
        {
            var databaseSetup = new DatabaseSetup { ConnectionString = Globals.LocalDbConnectionString };
            databaseSetup.InstallDatabase(Constants.CreateDbDefault);
        }

        [TestCleanup]
        public void TearDown()
        {
            var databaseSetup = new DatabaseSetup { ConnectionString = Globals.LocalDbConnectionString };
            databaseSetup.UninstallDatabase(Constants.CUWebinarsDb);
        }


        [TestMethod]
        [TestCategory(TestCategories.WebinarRepositoryIntegration)]
        public void FindByPresenterLastNameReturnsWebinarsByPresenter()
        {
            //  Arrange
            var ctx = new TTSWebinarsContext(Constants.LocalDbConnectionStringName);
            webinarRepository = new WebinarRepository(ctx);

            //  Act
            var webinars = webinarRepository.FindByPresenterLastName(PresenterLastName);
            var title = webinars.Where(webinar => webinar.Title == WebinarTitle).Select(webinar => webinar.Title).First();

            //  Assert                 
            Assert.AreEqual(title, WebinarTitle);
            Assert.IsTrue(webinars.Count() == 2);
        }

        [TestMethod]
        [TestCategory(TestCategories.WebinarRepositoryIntegration)]
        public void FindByPresenterLastNameReturnsNoWebinarsByNonExistantPresenter()
        {
            //  Arrange
            var ctx = new TTSWebinarsContext(Constants.LocalDbConnectionStringName);
            webinarRepository = new WebinarRepository(ctx);

            //  Act
            var webinars = webinarRepository.FindByPresenterLastName("InavlidSurname");

            //  Assert                 
            Assert.IsFalse(webinars.Any());
        }

        [TestMethod]
        [TestCategory(TestCategories.WebinarRepositoryIntegration)]
        public void FindByTopicDescriptionReturnsOneWebinar()
        {
            //  Arrange
            var ctx = new TTSWebinarsContext(Constants.LocalDbConnectionStringName);
            webinarRepository = new WebinarRepository(ctx);

            //  Act
            var webinars = webinarRepository.FindByDescription(TopicDescription);
            var title = webinars.Where(webinar => webinar.Title == WebinarTitleOfWebinarWithOperationsDesc).Select(webinar => webinar.Title).First();

            //  Assert                 
            Assert.IsTrue(webinars.Count() == 1);
        }

        [TestMethod]
        [TestCategory(TestCategories.WebinarRepositoryIntegration)]
        public void FindByTopicDescriptionReturnsNoWebinarsWhereDescriptionNotExist()
        {
            //  Arrangeo
            var ctx = new TTSWebinarsContext(Constants.LocalDbConnectionStringName);
            webinarRepository = new WebinarRepository(ctx);

            //  Act
            var webinars = webinarRepository.FindByDescription("InvalidDescription");

            //  Assert                 
            Assert.IsFalse(webinars.Any());
        }

        [TestMethod]
        [TestCategory(TestCategories.WebinarRepositoryIntegration)]
        public void FindByTopicDescriptionReturnsOneWebinarWithCorrectDescription()
        {
            //  Arrange
            var ctx = new TTSWebinarsContext(Constants.LocalDbConnectionStringName);
            webinarRepository = new WebinarRepository(ctx);

            //  Act
            var webinars = webinarRepository.FindByDescription(TopicDescription);
            var title = webinars.Where(webinar => webinar.Title == WebinarTitleOfWebinarWithOperationsDesc).Select(webinar => webinar.Title).First();

            //  Assert                 
            Assert.AreEqual(title, WebinarTitleOfWebinarWithOperationsDesc);
            Assert.IsTrue(webinars.Count() == 1);
        }


        [TestMethod]
        [TestCategory(TestCategories.WebinarRepositoryIntegration)]
        public void FindByTopicDescriptionReturnsOneWebinarWithCorrectDescriptions()
        {
            //  Arrange
            var ctx = new TTSWebinarsContext(Constants.LocalDbConnectionStringName);
            webinarRepository = new WebinarRepository(ctx);

            //  Act
            var webinarsByTopic = webinarRepository.FindByDescription(TopicDescription);
            var webinarsByPresenterLastName = webinarRepository.FindByPresenterLastName(PresenterLastName);

            var unionOfRetrievedWebinars = webinarsByTopic.Union(webinarsByPresenterLastName);

            //  Assert                 
            Assert.IsTrue(unionOfRetrievedWebinars.Count() == 2);
        }


    }
}
