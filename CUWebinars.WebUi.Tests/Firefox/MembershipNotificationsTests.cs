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

namespace CUWebinars.WebUi.Tests.Firefox
{
    [TestClass]
    public class MembershipNotificationsTests : FirefoxBaseTest
    {
        private int notificationFileCreationTimeout = 10;
        private string notificationsDirectory = string.Empty;

        [TestMethod]
        [TestCategory(TestCategories.Gui)]
        [TestCategory(TestCategories.Firefox)]
        [Ignore]
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

            membershipNotificationsPage.Close();

            Assert.IsTrue(VerifyLinkIncludedInSentEmail());
        }

        [TestMethod]
        [TestCategory(TestCategories.Gui)]
        [TestCategory(TestCategories.Firefox)]
        public void CreateNewOrderForExistingUserScenario1()
        {
            PerformTest(Constants.TestQueryString6);

            Assert.IsTrue(VerifyLinkNotIncludedInSentEmail());
        }

        [TestMethod]
        [TestCategory(TestCategories.Gui)]
        [TestCategory(TestCategories.Firefox)]
        public void CreateNewOrderForExistingUserScenario2()
        {
            PerformTest(Constants.TestQueryString7);

            Assert.IsTrue(VerifyLinkNotIncludedInSentEmail());            
        }

        private void PerformTest(string queryString)
        {
            var membershipNotificationsPage = NavigateToMembershipNotificationsPage();
            membershipNotificationsPage.ClickImportSingleOrderButton();

            var json = TestHelper.TransformQueryStringToJsonCompliantString(queryString);

            membershipNotificationsPage.SetValueFromJsonOrderTextArea(json.ToString());

            membershipNotificationsPage.ClickImportOrderButton();

            membershipNotificationsPage.Close();

            WaitOnNotificationFileCreation(notificationFileCreationTimeout);
        }

        private void WaitOnNotificationFileCreation(int timeout)
        {
            var doops = new DataOperations();

            var notificationsDirectory = GetNotificationsDirectory();

            string persistedFileName = doops.GetNameOfLatestPersistedNotification().SubstringFrom("OrderNotification");

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

            Trace.WriteLine(string.Format("{0} files", notificationFiles.Length));
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
