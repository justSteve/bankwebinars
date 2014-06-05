using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CUWebinars.Selenium.Core;

namespace CUWebinars.WebUi.Tests.Page
{
    public abstract class MembershipNotificationsBasePage : BasePage
    {
        public MembershipNotificationsBasePage(ITestDriver seleniumTestDriver)
            : base(seleniumTestDriver)
        {
            Url = GlobalTestConfig.MembershipNotificationsUrl;
        }

        public void ClickImportOrderButton()
        {
            SeleniumTestDriver.FindByIdClick("ImportOrderButton");
            var element = SeleniumTestDriver.FindByIdWithWait("OrderSucceeded", 90);

            Wait(5000);
        }

        public void ClickImportSingleOrderButton()
        {
            SeleniumTestDriver.FindByIdClick("GetImportOrderFieldsButton");
        }

        public string GetValueFromJsonOrderTextArea()
        {
            var textArea = SeleniumTestDriver.FindById("JsonPayloadTextArea");

            return textArea.Text;
        }

        public void SetValueFromJsonOrderTextArea(string jsonText)
        {
            var textArea = SeleniumTestDriver.FindById("JsonPayloadTextArea");
            textArea.Clear();
            textArea.SendKeys(jsonText);
        }
    }
}
