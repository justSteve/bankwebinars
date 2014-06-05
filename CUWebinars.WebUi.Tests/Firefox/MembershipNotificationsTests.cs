using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CUWebinars.WebUi.Tests.Infrastructure;
using CUWebinars.WebUi.Tests.Page.Firefox;
using HtmlAgilityPack;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace CUWebinars.WebUi.Tests.Firefox
{
    [TestClass]
    public class MembershipNotificationsTests : FirefoxBaseTest
    {
        [TestMethod]
        [TestCategory(TestCategories.Gui)]
        [TestCategory(TestCategories.Firefox)]
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

            var createOrderTask = Task.Factory.StartNew(
                () => membershipNotificationsPage.ClickImportOrderButton(),
                TaskCreationOptions.LongRunning
                ).ContinueWith((t) => Assert.IsTrue(VerifyLinkIncludedInSentEmail()));

            Task.WaitAll(createOrderTask);
        }

        [TestMethod]
        [TestCategory(TestCategories.Gui)]
        [TestCategory(TestCategories.Firefox)]
        //public void CreateNewOrderForExistingUser()
        //{
        //    var firstName = WebUiTestHelpers.RandomString(4);
        //    var lastName = WebUiTestHelpers.RandomString(4);
        //    var email = string.Concat(firstName, "_", lastName, Constants.SitTestEmailAddressDomain);

        //    var membershipNotificationsPage = NavigateToMembershipNotificationsPage();

        //    IMembershipService membershipService = BusinessTestHelper.CreateMembershipService();
        //    membershipService.CreateUser(WebUiTestGlobals.Tenant, firstName, lastName, string.Empty, Constants.SitTestPassword,
        //        email);


        //    membershipNotificationsPage.ClickImportSingleOrderButton();
        //    var json = membershipNotificationsPage.GetValueFromJsonOrderTextArea();

        //    JObject parsedJsonObject = JObject.Parse(json);

        //    parsedJsonObject["Email"] = Constants.SitTestEmailAddress;
        //    membershipNotificationsPage.SetValueFromJsonOrderTextArea(parsedJsonObject.ToString());

        //    var createOrderTask = Task.Factory.StartNew(
        //        () => membershipNotificationsPage.ClickImportOrderButton(),
        //        TaskCreationOptions.LongRunning
        //        ).ContinueWith((t) => Assert.IsFalse(VerifyLinkIncludedInSentEmail()));

        //    Task.WaitAll(createOrderTask);
        //}



        private bool VerifyLinkIncludedInSentEmail()
        {
            var notificationFiles =
                new DirectoryInfo(@"E:\TTS\BankWebinars\CUWebinars.Web\App_Data\Notifications\").GetFiles();
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

        private MembershipNotificationsPage NavigateToMembershipNotificationsPage()
        {
            var membershipNotificationsPage = new MembershipNotificationsPage(TestDriver);

            membershipNotificationsPage.Open();

            return membershipNotificationsPage;
        }


    }

}
