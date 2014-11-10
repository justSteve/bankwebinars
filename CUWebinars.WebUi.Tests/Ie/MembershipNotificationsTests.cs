using System.Globalization;
using CUWebinars.Business.Core.Tracing;
using CUWebinars.Tests.Common;
using CUWebinars.WebUi.Tests.Infrastructure;
using CUWebinars.WebUi.Tests.Page.Firefox;
using HtmlAgilityPack;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace CUWebinars.WebUi.Tests.Ie
{
    [TestClass]
    public class MembershipNotificationsTests : IeBaseTest
    {
        private int notificationFileCreationTimeout = 10;
        private string notificationsDirectory = string.Empty;
        DataOperations dataOperations = new DataOperations();

        [TestMethod]
        [TestCategory(TestCategories.Gui)]
        [TestCategory(TestCategories.IE)]
        public void CreateNewOrderForNewUser()
        {
            var firstName = WebUiTestHelpers.RandomString(4);
            var lastName = WebUiTestHelpers.RandomString(4);

            var membershipNotificationsPage = NavigateToMembershipNotificationsPage();

            membershipNotificationsPage.ClickImportSingleOrderButton();
            var json = membershipNotificationsPage.GetValueFromJsonOrderTextArea();

            JObject parsedJsonObject = JObject.Parse(json);
            var emailForUser = string.Concat(firstName, "_", lastName, Constants.SitTestEmailAddressDomain);

            parsedJsonObject["Email"] = emailForUser;
            membershipNotificationsPage.SetValueFromJsonOrderTextArea(parsedJsonObject.ToString());

            membershipNotificationsPage.ClickImportOrderButton();

            WaitOnNotificationFileCreation(notificationFileCreationTimeout);

            var orderSuccessString = membershipNotificationsPage.GetOrderNumberCreated();

            var orderId = GetOrderIdOfNewOrder(orderSuccessString);

            if (ReferenceEquals(null, orderId))
                Assert.Fail("Unable to obtain order number of the new order.");

            membershipNotificationsPage.Close();

            Assert.IsTrue(VerifyLinkIncludedInSentEmail());

            dataOperations.ConnectionString = WebUiTestGlobalsTestConfig.DefaultConnection;

            dataOperations.DeleteOrder(orderId.Value);
        }

        [TestMethod]
        [TestCategory(TestCategories.Gui)]
        [TestCategory(TestCategories.IE)]
        public void CreateNewOrderForExistingUserScenario1()
        {
            var idOrder = PerformTest(Constants.TestQueryString1);

            Assert.IsTrue(VerifyLinkNotIncludedInSentEmail());

            dataOperations.ConnectionString = WebUiTestGlobalsTestConfig.DefaultConnection;

            dataOperations.DeleteOrder(idOrder);

        }

        [TestMethod]
        [TestCategory(TestCategories.Gui)]
        [TestCategory(TestCategories.IE)]
        public void CreateNewOrderForExistingUserScenario2()
        {
            var idOrder = PerformTest(Constants.TestQueryString2);

            Assert.IsTrue(VerifyLinkNotIncludedInSentEmail());

            dataOperations.ConnectionString = WebUiTestGlobalsTestConfig.DefaultConnection;

            dataOperations.DeleteOrder(idOrder);

        }

        [TestMethod]
        [TestCategory(TestCategories.Gui)]
        [TestCategory(TestCategories.IE)]
        public void CreateNewOrderForExistingUserScenario3()
        {
            var idOrder = PerformTest(Constants.TestQueryString3);

            Assert.IsTrue(VerifyLinkNotIncludedInSentEmail());

            dataOperations.ConnectionString = WebUiTestGlobalsTestConfig.DefaultConnection;

            dataOperations.DeleteOrder(idOrder);

        }

        [TestMethod]
        [TestCategory(TestCategories.Gui)]
        [TestCategory(TestCategories.IE)]
        public void CreateNewOrderForExistingUserScenario4()
        {
            var email = TestHelper.ExtractEmailAddress(Constants.TestQueryString4);
            var idOrder = PerformTest(Constants.TestQueryString4);

            Assert.IsTrue(VerifyLinkIncludedInSentEmail());

            dataOperations.ConnectionString = WebUiTestGlobalsTestConfig.DefaultConnection;

            dataOperations.DeleteOrder(idOrder);
            dataOperations.DeleteWebUser(email);
            dataOperations.ConnectionString = WebUiTestGlobalsTestConfig.MembershipRebootConnection;
            dataOperations.DeleteUserAccountAndClaims(email);
        }

        [TestMethod]
        [TestCategory(TestCategories.Gui)]
        [TestCategory(TestCategories.IE)]
        public void CreateNewOrderForExistingUserScenario5()
        {
            var idOrder = PerformTest(Constants.TestQueryString5);

            Assert.IsTrue(VerifyLinkNotIncludedInSentEmail());

            dataOperations.ConnectionString = WebUiTestGlobalsTestConfig.DefaultConnection;

            dataOperations.DeleteOrder(idOrder);

        }

        [TestMethod]
        [TestCategory(TestCategories.Gui)]
        [TestCategory(TestCategories.IE)]
        public void CreateNewOrderForExistingUserScenario6()
        {
            var idOrder = PerformTest(Constants.TestQueryString6);

            Assert.IsTrue(VerifyLinkNotIncludedInSentEmail());

            dataOperations.ConnectionString = WebUiTestGlobalsTestConfig.DefaultConnection;

            dataOperations.DeleteOrder(idOrder);

        }

        [TestMethod]
        [TestCategory(TestCategories.Gui)]
        [TestCategory(TestCategories.IE)]
        public void CreateNewOrderForExistingUserScenario7()
        {
            var idOrder = PerformTest(Constants.TestQueryString7);

            Assert.IsTrue(VerifyLinkNotIncludedInSentEmail());

            dataOperations.ConnectionString = WebUiTestGlobalsTestConfig.DefaultConnection;

            dataOperations.DeleteOrder(idOrder);

        }

        [TestMethod]
        [TestCategory(TestCategories.Gui)]
        [TestCategory(TestCategories.IE)]
        public void CreateNewOrderForExistingUserScenario8()
        {
            var idOrder = PerformTest(Constants.TestQueryString8);

            Assert.IsTrue(VerifyLinkNotIncludedInSentEmail());

            dataOperations.ConnectionString = WebUiTestGlobalsTestConfig.DefaultConnection;

            dataOperations.DeleteOrder(idOrder);

        }

        [TestMethod]
        [TestCategory(TestCategories.Gui)]
        [TestCategory(TestCategories.IE)]
        public void CreateNewOrderForExistingUserScenario9()
        {
            var idOrder = PerformTest(Constants.TestQueryString9);

            Assert.IsTrue(VerifyLinkNotIncludedInSentEmail());

            dataOperations.ConnectionString = WebUiTestGlobalsTestConfig.DefaultConnection;

            dataOperations.DeleteOrder(idOrder);

        }


        [TestMethod]
        [TestCategory(TestCategories.Gui)]
        [TestCategory(TestCategories.IE)]
        public void CreateNewOrderForExistingUserScenario10()
        {
            var idOrder = PerformTest(Constants.TestQueryString10);

            Assert.IsTrue(VerifyLinkNotIncludedInSentEmail());

            dataOperations.ConnectionString = WebUiTestGlobalsTestConfig.DefaultConnection;

            dataOperations.DeleteOrder(idOrder);

        }

        [TestMethod]
        [TestCategory(TestCategories.Gui)]
        [TestCategory(TestCategories.IE)]
        public void CreateNewOrderForExistingUserScenario11()
        {
            var idOrder = PerformTest(Constants.TestQueryString11);
            var email = TestHelper.ExtractEmailAddress(Constants.TestQueryString11);

            Assert.IsTrue(VerifyLinkIncludedInSentEmail());

            dataOperations.ConnectionString = WebUiTestGlobalsTestConfig.DefaultConnection;
            dataOperations.DeleteOrder(idOrder);
            dataOperations.DeleteWebUser(email);

            dataOperations.ConnectionString = WebUiTestGlobalsTestConfig.MembershipRebootConnection;
            dataOperations.DeleteUserAccountAndClaims(email);
        }

        [TestMethod]
        [TestCategory(TestCategories.Gui)]
        [TestCategory(TestCategories.IE)]
        public void CreateNewOrderForExistingUserScenario12()
        {
            var idOrder = PerformTest(Constants.TestQueryString12);

            Assert.IsTrue(VerifyLinkNotIncludedInSentEmail());

            dataOperations.ConnectionString = WebUiTestGlobalsTestConfig.DefaultConnection;

            dataOperations.DeleteOrder(idOrder);

        }


        private int PerformTest(string queryString)
        {
            var membershipNotificationsPage = NavigateToMembershipNotificationsPage();
            membershipNotificationsPage.ClickImportSingleOrderButton();

            var json = TestHelper.TransformQueryStringToJsonCompliantString(queryString);

            membershipNotificationsPage.SetValueFromJsonOrderTextArea(json.ToString());

            membershipNotificationsPage.ClickImportOrderButton();

            WaitOnNotificationFileCreation(notificationFileCreationTimeout);

            var orderSuccessString = membershipNotificationsPage.GetOrderNumberCreated();

            membershipNotificationsPage.Close();

            var orderId = GetOrderIdOfNewOrder(orderSuccessString);

            if (ReferenceEquals(null, orderId))
                Assert.Fail("Unable to obtain order number of the new order.");

            return orderId.Value;
        }

        private void WaitOnNotificationFileCreation(int timeout)
        {
            dataOperations.ConnectionString = WebUiTestGlobalsTestConfig.TtsDatabaseConnectionString;

            var notificationsDirectory = GetNotificationsDirectory();

            string persistedFileName = dataOperations.GetNameOfLatestPersistedNotification().SubstringFrom("OrderNotification");

            var directoryInfo = new DirectoryInfo(notificationsDirectory);

            FileInfo[] files;

            var secs = 0;
            var timeoutAsQuarterSeconds = timeout * 4;

            while (secs < timeoutAsQuarterSeconds)
            {
                files = directoryInfo.GetFiles();

                if (files.Any(f => f.Name == persistedFileName))
                {
                    break;
                }

                Thread.Sleep(250);
                secs++;
            }

            Trace.WriteLine(persistedFileName);


        }


        private bool VerifyLinkIncludedInSentEmail()
        {
            var directory = GetNotificationsDirectory();
            var notificationFiles = new DirectoryInfo(directory).GetFiles();

            Tracer.Information(string.Format("{0} files", notificationFiles.Length));
            var mostRecentNotification =
                notificationFiles.Where(f => f.Extension == ".htm").OrderByDescending(f => f.LastWriteTime).First();

            var populatedEmail = new HtmlDocument();
            populatedEmail.Load(new FileStream(mostRecentNotification.FullName, FileMode.Open));

            var verifyLink = populatedEmail.DocumentNode.SelectNodes("//i");

            if (ReferenceEquals(null, verifyLink))
                return false;

            return verifyLink.Count == 1;
        }

        private bool VerifyLinkNotIncludedInSentEmail()
        {
            return !VerifyLinkIncludedInSentEmail();
        }

        private int? GetOrderIdOfNewOrder(string orderSuccessString)
        {
            var orderIdAsString = orderSuccessString.SubstringFrom(":").Remove(0, 1);
            int id;

            if (int.TryParse(orderIdAsString, NumberStyles.AllowLeadingWhite, CultureInfo.CurrentCulture, out id)) 
                ;
            return id;

            return null;
        }

        private string GetNotificationsDirectory()
        {
            if (string.IsNullOrEmpty(notificationsDirectory))
                notificationsDirectory = Path.Combine(string.Concat(AppDomain.CurrentDomain.BaseDirectory, TestConstants.UpThreeFolders), TestConstants.NotificationsDirectory);
            return notificationsDirectory;
        }

        private MembershipNotificationsPage NavigateToMembershipNotificationsPage()
        {
            var membershipNotificationsPage = new MembershipNotificationsPage(TestDriver);

            membershipNotificationsPage.Open();

            return membershipNotificationsPage;
        }


    }

}
