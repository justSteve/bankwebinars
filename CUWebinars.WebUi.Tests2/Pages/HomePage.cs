using KesselRun.SeleniumCore.TestDrivers.Contracts;

namespace CUWebinars.WebUi.Tests2.Pages
{
    public class HomePage : BasePage
    {
        private const string ForgotPasswordLinkText = "Forgot Password";

        public HomePage(ITestDriver seleniumTestDriver) : base(seleniumTestDriver)
        {
        }

        public bool PhoneNrLinkIsPresentOnPage
        {
            get { return SeleniumTestDriver.FindByCssSelector("div.phone a.tele") != null; }
        }
    }
}
