using CUWebinars.Selenium.Core;
using CUWebinars.Selenium.Core.Enums;
using CUWebinars.WebUi.Tests.Infrastructure;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;

namespace CUWebinars.WebUi.Tests.Page
{
    public class HomeIndexBasePage : BasePage
    {
        private const string ForgotPasswordLinkText = "Forgot Password";

        public HomeIndexBasePage(ITestDriver seleniumTestDriver) : base(seleniumTestDriver)
        {

        }


        public void EnterEmailAddress(string email, string domItem)
        {
            SeleniumTestDriver.TypeText(By.Id(domItem), email);
        }


        public void EnterEmailAddressAndEnter(string email)
        {
            SeleniumTestDriver.TypeText(By.Name("RegisterFields.Email"), email);

            var theSubmitButtonWait = new WebDriverWait(SeleniumTestDriver.WebDriver, TimeSpan.FromSeconds(WaitTimeout));
            var theSubmitButton = theSubmitButtonWait.Until(ExpectedConditions.ElementExists(By.Id("TheSubmitButton")));
            theSubmitButton.Click();
        }


        public void EnterPassword(string password, string domItem)
        {
            SeleniumTestDriver.TypeText(By.Id(domItem), password);
        }


        public void EnterPasswordWhereUserExists(string password)
        {
            SeleniumTestDriver.TypeText(By.Name("Password1"), password);

            var theSubmitButtonWait = new WebDriverWait(SeleniumTestDriver.WebDriver, TimeSpan.FromSeconds(WaitTimeout));
            var theSubmitButton = theSubmitButtonWait.Until(ExpectedConditions.ElementExists(By.Id("TheSubmitButton")));
            theSubmitButton.Click();
        }

        public void LogOff()
        {
            SeleniumTestDriver.FindByPartialLinkText(Constants.LogoffLinkText).Click();
        }

        public bool NoNotceExistsErrorTextIsPresent
        {
            get
            {
                var errorMessageElement =
                    SeleniumTestDriver.FindByXPath(@"//*[@id=""innerContent""]/div[5]/div[2]/span");

                return errorMessageElement != null &&
                       errorMessageElement.Text.Equals("No Notice exists with supplied criteria",
                           StringComparison.OrdinalIgnoreCase);
            }
        }

        public bool ManageLoggedInUserLinkIsPresentOnPage
        {
            get
            {
                return SeleniumTestDriver.FindById("btnLogin") != null;
            }
        }
        public bool PasswordResetInstructionsSentLabelPresent
        {
            get
            {
                return SeleniumTestDriver.FindById("labelEmail") != null;
            }
        }

        public bool PasswordResetInstructionsInCrunchingLabelPresent
        {
            get
            {
                return SeleniumTestDriver.FindByXPath(@"//*[@id='crunchingLabel']/span[contains(text(),'Reset Instructions sent!')]") != null;
            }
        }


        public void ClickLoginLink()
        {
            SeleniumTestDriver.FindByLinkTextClick(Constants.LoginLinkText);
        }

        public void LogInToSite(string email, string password)
        {
            SeleniumTestDriver.TypeTextWithEnter(Constants.EmailInput, email);
            SeleniumTestDriver.TypeText(By.Name(Constants.PasswordInput), password);
            SeleniumTestDriver.FindByIdClick(Constants.SignInButtonInput);
        }

        public void ClickRegisterLinkOnLoginView()
        {
            var newAccountWait = new WebDriverWait(SeleniumTestDriver.WebDriver, TimeSpan.FromSeconds(WaitTimeout));
            var element = newAccountWait.Until(ExpectedConditions.ElementExists(By.XPath("/html/body/div/div/div/form/fieldset/div[3]/input")));

            element.Click();
        }

        public void ClickResetPasswordLink()
        {
            SeleniumTestDriver.FindByXPathClick("//*[@id='frmSignIn']/fieldset/div[3]/span/input");
        }

        public void ClickResetPasswordButton(string buttonId)
        {
            SeleniumTestDriver.FindByIdClick(buttonId);
        }

        public void ClickSubmitButton()
        {
            SeleniumTestDriver.FindByIdClick("TheSubmitButton");
        }


        public void ClickYesUseAddressButton()
        {
            SeleniumTestDriver.FindByNameClick("YesUseAddress");
        }

        public void ClickNoEnterDiffAddressButton()
        {
            SeleniumTestDriver.FindByNameClick("EnterDiffAddress");
        }


        public void ClickNotInstitutionButton()
        {
            SeleniumTestDriver.FindByNameClick("NotInstitution");
        }
        
        public bool PhoneNrLinkIsPresentOnPage
        {
            get { return SeleniumTestDriver.FindByCssSelector("div.phone a.tele") != null; }
        }

        public bool ProceedOrEnterDifferentEmailMessagePresent
        {
            get
            {
                var label = SeleniumTestDriver.FindById("labelEmail");
                return label.FindElement(By.TagName("span")).Text.Contains("Proceed or enter a different email address.");
            }
        }


        public void TabAwayFromInput(string domItem, SelectorStrategy selectorStrategy)
        {
            switch (selectorStrategy)
            {
                case SelectorStrategy.Id:
                    SeleniumTestDriver.TabAwayFromInput(By.Id(domItem));
                    break;
                case SelectorStrategy.Name:
                    SeleniumTestDriver.TabAwayFromInput(By.Name(domItem));
                    break;
                case SelectorStrategy.CssSelector:
                    SeleniumTestDriver.TabAwayFromInput(By.CssSelector(domItem));
                    break;
                case SelectorStrategy.ClassName:
                    SeleniumTestDriver.TabAwayFromInput(By.ClassName(domItem));
                    break;
                case SelectorStrategy.XPath:
                    SeleniumTestDriver.TabAwayFromInput(By.XPath(domItem));
                    break;
                default:
                    throw new NotSupportedException(string.Format("{0} is not a supported SelectorStrategy for this operation", selectorStrategy));
            }
        }

        public bool LoginLinkIsPresentOnPage
        {
            get
            {
                var logoutLinkWait = new WebDriverWait(SeleniumTestDriver.WebDriver, TimeSpan.FromSeconds(WaitTimeout));

                var logInLink = logoutLinkWait.Until(ExpectedConditions.ElementIsVisible(By.PartialLinkText(Constants.LoginLinkText)));

                return logInLink != null;
            }
        }

        public bool LogoutLinkIsPresentOnPage
        {
            get
            {
                var logoutLinkWait = new WebDriverWait(SeleniumTestDriver.WebDriver, TimeSpan.FromSeconds(WaitTimeout));

                var logOffLink = logoutLinkWait.Until(ExpectedConditions.ElementIsVisible(By.PartialLinkText(Constants.LogoffLinkText)));

                return logOffLink != null;
            }
        }

        public bool EmailNotValidMessageIsPresent
        {
            get
            {
                var msgSpan = SeleniumTestDriver.FindByClassName("field-validation-error");
                var msg = msgSpan.FindElement(By.TagName("span"));
                return msg.Text.Equals("The Email field is not a valid e-mail address.",
                    StringComparison.OrdinalIgnoreCase);
            } 
        }
    }
}
