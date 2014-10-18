using CUWebinars.WebUi.Tests2.Infrastructure;
using KesselRun.SeleniumCore.Enums;
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

        public void ClickLoginLink()
        {
            SeleniumTestDriver.FindByLinkClick(TestConstants.LoginLinkText);
        }

        public void LogInToSite(string sitTestEmailAddress, string sitTestPassword)
        {
            SeleniumTestDriver.TypeText(FinderStrategy.Name, TestConstants.EmailInput, sitTestEmailAddress);
            SeleniumTestDriver.TypeText(FinderStrategy.Name, TestConstants.PasswordInput, sitTestPassword);
            SeleniumTestDriver.FindByIdClick(TestConstants.SignInButtonInput);
        }

        public bool LoginLinkIsPresentOnPage
        {
            get
            {
                var loginLink = SeleniumTestDriver.FindByPartialLinkText(TestConstants.LoginLinkText);

                return loginLink.Displayed;
            }
        }

        public virtual void LogOff()
        {
            SeleniumTestDriver.FindByPartialLinkTextClick(TestConstants.LogoffLinkText);
        }

    }
}
