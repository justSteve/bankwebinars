using System;
using System.Threading;
using CUWebinars.WebUi.Tests2.Infrastructure;
using KesselRun.SeleniumCore.Enums;
using KesselRun.SeleniumCore.TestDrivers.Contracts;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace CUWebinars.WebUi.Tests2.Pages
{
    public class HomePage : BasePage
    {
        private const string ForgotPasswordLinkText = "Forgot Password";
        private const string EdgeCaseResetPasswordButton = "EdgeCaseResetPasswordButton";
        private const string NormalResetPasswordButton = "NormalResetPasswordButton";
        private const string TheSubmitButton = "TheSubmitButton";

        public HomePage(ITestDriver seleniumTestDriver) : base(seleniumTestDriver)
        {
        }

        public bool PhoneNrLinkIsPresentOnPage
        {
            get { return SeleniumTestDriver.FindByCssSelector("div.phone a.tele") != null; }
        }

        public void ClickLoginLink()
        {
            SeleniumTestDriver.FindByLinkClick(TestConstants.LoginLinkText, ExpectedCondition.ElementIsVisible, 15);
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
                var loginLink = SeleniumTestDriver.FindByPartialLinkText(TestConstants.LoginLinkText, ExpectedCondition.ElementIsVisible,5);

                return loginLink.Displayed;
            }
        }

        public bool LogoutLinkIsPresentOnPage
        {
            get
            {
                return SeleniumTestDriver.FindByPartialLinkText(
                    TestConstants.LogoffLinkText, 
                    ExpectedCondition.ElementIsVisible, 30).Displayed;
            } 
        }

        public bool PasswordResetInstructionsSentLabelPresent
        {
            get
            {
                Func<IWebDriver, IWebElement> conditionFunc =
                    (w) => SeleniumTestDriver.FindByCssSelector("#labelEmail > span")
                            .Text.Contains("Reset Instructions sent")
                            ? SeleniumTestDriver.FindByCssSelector("#labelEmail > span") 
                            : null;
                /*This may look really inefficient (that is, the repeated searches for the element in the retries) but it is necessary to avoid a Selenium StaleElementException*/
                var feedbackLabel = SeleniumTestDriver.FindWithWait(20, conditionFunc);

                return feedbackLabel.Text.Contains("Reset Instructions sent"); 
            }
        }

        public bool EmailNotValidMessageIsPresent
        {
            get
            {
                var msgSpan = SeleniumTestDriver.FindByClassName("field-validation-error");
                var msg = SeleniumTestDriver.FindByTagNameFromWebElement(msgSpan, "span", 5);
                return msg.Text.Equals("The Email field is not a valid e-mail address.", StringComparison.OrdinalIgnoreCase);
            }
        }

        public bool ManageLoggedInUserLinkIsPresentOnPage 
        {
            get { return SeleniumTestDriver.FindById("btnLogin", ExpectedCondition.ElementIsVisible, 5).Displayed; }
        }

        public bool ProceedOrEnterDifferentEmailMessagePresent
        {
            get
            {
                var label = SeleniumTestDriver.FindById("labelEmail");
                return SeleniumTestDriver.FindByTagNameFromWebElement(label,"span", 5).Text.Contains("Proceed or enter a different email address.");
            }
        }

        public bool PasswordResetInstructionsInCrunchingLabelPresent
        {
            get
            {
                var label = SeleniumTestDriver.FindByXPath(@"//*[@id='crunchingLabel']/span/i",
                    ExpectedCondition.ElementIsVisible, 15);

                return label.Displayed;
            }

        }

        public bool WebinarTitleIsDisplayedOnWebinarDetailsPage
        {
            get { return SeleniumTestDriver.FindByCssSelector("#webinarTitle > div > h1", ExpectedCondition.ElementIsVisible, 15).Displayed; } 
            
        }

        public virtual void LogOff()
        {
            SeleniumTestDriver.FindByPartialLinkTextClick(TestConstants.LogoffLinkText, ExpectedCondition.ElementIsVisible, 20);
        }

        public void ClickRegisterLinkOnLoginView()
        {
            SeleniumTestDriver.FindByIdClick("registerLinkButton");
        }

        public void EnterEmailAddressAndClickSubmit(string email)
        {
            Wait();
            var emailInput = SeleniumTestDriver.FindByName("RegisterFields.Email", ExpectedCondition.ElementIsVisible, 5);
            SeleniumTestDriver.TypeText(emailInput, email);
            ClickSubmit();
        }

        public bool ClickSubmit()
        {
            return SeleniumTestDriver.FindByIdClickWithRetries(TheSubmitButton, ExpectedCondition.ElementIsVisible, 5, 100);
        }

        public void EnterPasswordWhereUserExists(string sitTestPassword)
        {
            var passwordInput = SeleniumTestDriver.FindByName("Password1", ExpectedCondition.ElementIsVisible, 5);
            SeleniumTestDriver.TypeText(passwordInput, sitTestPassword);
            ClickSubmit();
        }

        public bool ClickResetPasswordButton(string resetPasswordButton)
        {
            Wait(1000);
            return SeleniumTestDriver.FindByIdClickWithRetries(resetPasswordButton, ExpectedCondition.ElementIsVisible, 5, 10);
        }

        public void TabAwayFromInput(string id)
        {
            var oi = SeleniumTestDriver.FindById(id);
            oi.SendKeys(Keys.Tab);
        }

        public void WaitForLabel(string message)
        {
            //var waitForWords = new WebDriverWait(SeleniumTestDriver.WebDriver, TimeSpan.FromSeconds(10));
            SeleniumTestDriver.FindWithWait(30,
                (w) =>
                    SeleniumTestDriver.FindByCssSelector("#labelEmail > span").Text.Contains(message)
                        ? SeleniumTestDriver.FindByCssSelector("#labelEmail > span")
                        : null);

            //var feedbackLabel = waitForWords.Until();
        }

        public void ClickYesUseAddressButton()
        {
            SeleniumTestDriver.FindByNameClick("YesUseAddress", ExpectedCondition.ElementIsVisible, 5);
        }

        public void ClickNoEnterDiffAddressButton()
        {
            SeleniumTestDriver.FindByNameClick("EnterDiffAddress", ExpectedCondition.ElementIsVisible, 5);
        }

        public void ClickNotInstitutionButton()
        {
            SeleniumTestDriver.FindByNameClick("NotInstitution", ExpectedCondition.ElementIsVisible, 5);
        }
        public void ClickCancelModalButton()
        {
            // This is the small cross on the top right of the modal window for canelling out of it.
            SeleniumTestDriver.FindByXPathClick("//*[@id='modalInstitution']/div[1]/button", ExpectedCondition.ElementIsVisible, 5);
        }

        public void ClickResetPasswordLink()
        {
            SeleniumTestDriver.FindByXPathClick("//*[@id='forgotPasswordLinkButton']",
                ExpectedCondition.ElementIsVisible, 5);
        }

        public bool ClickUpcomingEventsMenuItem()
        {
            SeleniumTestDriver.MouseOverElement(FinderStrategy.XPath, @"//*[@id='main_menu']/ul/li[3]/a");

            SeleniumTestDriver.FindByXPathClick(@"//*[@id='main_menu']/ul/li[3]/ul/li[3]/a", ExpectedCondition.ElementIsVisible, 2);

            return SeleniumTestDriver.FindById("webinarContent", ExpectedCondition.ElementIsVisible, 5).Displayed;

        }

        public bool ClickTopicsMenuItem()
        {
            SeleniumTestDriver.MouseOverElement(FinderStrategy.XPath, @"//*[@id='main_menu']/ul/li[2]/a");

            SeleniumTestDriver.FindByXPathClick(@"//*[@id='main_menu']/ul/li[2]/ul/li[3]/a", ExpectedCondition.ElementIsVisible, 5);

            return SeleniumTestDriver.FindByXPath("//*[@id='page']/div[3]/div[1]/h3/a", ExpectedCondition.ElementIsVisible, 5).Displayed;
        }
        public void ClickWebinarsMenuItem(int subMenuNumber)
        {
            try
            {
                SeleniumTestDriver.MouseOverElementUsingScript("$('#main_menu > ul > li:nth-child(3) > a').mouseenter();");

                Wait();
                var subMenuItem = SeleniumTestDriver.FindByXPath(
                    string.Format(@"//*[@id='main_menu']/ul/li[3]/ul/li[{0}]/a", subMenuNumber), ExpectedCondition.ElementExists, 5);
                Wait();

                subMenuItem.Click();
            }
            catch (Exception)
            {
                SeleniumTestDriver.MouseOverElementUsingScript("$('#main_menu > ul > li:nth-child(3) > a').mouseenter();");

                Wait();
                var subMenuItem = SeleniumTestDriver.FindByXPath(
                    string.Format(@"//*[@id='main_menu']/ul/li[3]/ul/li[{0}]/a", subMenuNumber), ExpectedCondition.ElementExists, 5);
                Wait();

                subMenuItem.Click();
            }
        }

        public void ClickMoreButtonOnTopicsPage()
        {
            SeleniumTestDriver.FindByCssSelectorClick("#eventBody > p:nth-child(2) > a", ExpectedCondition.ElementIsVisible, 5);
        }
    }
}
