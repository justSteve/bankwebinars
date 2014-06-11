using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using CUWebinars.Selenium.Core.Enums;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace CUWebinars.Selenium.Core
{
    public abstract class BaseTestDriver : ITestDriver
    {
        protected const int WaitTimeout = 30;
        protected IWebDriver webDriver;
        protected string port;

        public string DriverPath { set; protected get; }
        public int DriverPort { set; protected get; }

        public abstract void Initialize();

        public void ClearFederatedCookies()
        {
            webDriver.Manage().Cookies.DeleteCookieNamed("FedAuth");
            webDriver.Manage().Cookies.DeleteCookieNamed("FedAuth1");
        }

        public virtual void CloseWindow()
        {
            webDriver.Close();
        }

        public string CurrentWindowHandle
        {
            get { return webDriver.CurrentWindowHandle; }
        }

        public virtual bool DoesElementContainText(string nameToFind, string text)
        {
            IWebElement element = webDriver.FindElement(By.Name(nameToFind));

            if (!ReferenceEquals(null, element))
            {
                return (element.Text.Contains(text));
            }

            return false;
        }

        public virtual string GetDocumentTitle()
        {
            return webDriver.Title;
        }

        public virtual void GoToUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                url = string.Format("http://localhost:{0}", port);
            }

            INavigation navigation = webDriver.Navigate();
            navigation.GoToUrl(url);
        }

        public virtual IWebElement FindByClassNameClick(string classNameToFind)
        {
            IWebElement element = FindByClassName(classNameToFind);

            if (!ReferenceEquals(null, element))
            {
                element.Click();
            }

            return element;
        }

        public virtual IWebElement FindByClassName(string classNameToFind)
        {
            var elementWait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(WaitTimeout));
            return elementWait.Until(ExpectedConditions.ElementIsVisible(By.ClassName(classNameToFind)));
        }

        public virtual IWebElement FindByCssSelectorClick(string cssSelectorToFind)
        {
            IWebElement element = FindByCssSelector(cssSelectorToFind);

            if (!ReferenceEquals(null, element))
            {
                element.Click();
            }

            return element;
        }

        public virtual IWebElement FindByCssSelector(string cssSelectorToFind)
        {
            var elementWait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(WaitTimeout));
            return elementWait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(cssSelectorToFind)));
        }

        public virtual IWebElement FindById(string idToFind)
        {
            var wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(WaitTimeout));
            return wait.Until(ExpectedConditions.ElementIsVisible(By.Id(idToFind)));
        }

        public virtual IWebElement FindByIdWithWait(string idToFind, int seconds)
        {
            var wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(seconds));
            return wait.Until(ExpectedConditions.ElementIsVisible(By.Id(idToFind)));
        }

        public virtual IWebElement FindByIdClick(string idToFind)
        {
            IWebElement element = FindById(idToFind);

            if (!ReferenceEquals(null, element))
            {
                element.Click();
            }

            return element;
        }

        public IWebElement FindByLinkText(string linkTextToFind)
        {
            var linkWait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(WaitTimeout));
            return linkWait.Until(ExpectedConditions.ElementIsVisible(By.LinkText(linkTextToFind)));
        }

        public IWebElement FindByPartialLinkText(string linkTextToFind)
        {
            var linkWait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(WaitTimeout));
            return linkWait.Until(ExpectedConditions.ElementIsVisible(By.PartialLinkText(linkTextToFind)));
        }

        public virtual IWebElement FindByLinkTextClick(string linkTextToFind)
        {
            var element = FindByLinkText(linkTextToFind);

            if (!ReferenceEquals(null, element))
            {
                element.Click();
            }

            return element;
        }

        public virtual IWebElement FindByPartialLinkTextClick(string linkTextToFind)
        {
            var element = FindByPartialLinkText(linkTextToFind);

            if (!ReferenceEquals(null, element))
            {
                element.Click();
            }

            return element;
        }

        public virtual IWebElement FindByXPath(string xpathToFind)
        {
            var elementWait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(WaitTimeout));
            return elementWait.Until(ExpectedConditions.ElementIsVisible(By.XPath(xpathToFind)));
        }

        public virtual IWebElement FindByXPathClick(string xpathToFind)
        {
            var element = FindByXPath(xpathToFind);
            
            if (!ReferenceEquals(null, element))
            {
                element.Click();
            }

            return element;
        }

        public virtual IWebElement FindByName(string nameToFind)
        {
            var wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(WaitTimeout));
            return wait.Until(ExpectedConditions.ElementIsVisible(By.Name(nameToFind)));
        }

        public virtual IWebElement FindByNameClick(string nameToFind)
        {
            var element = FindByName(nameToFind);

            if (!ReferenceEquals(null, element))
            {
                element.Click();
            }

            return element;
        }

        public IWebElement FindElementWithoutWait(SelectorStrategy selectorStrategy, string domItem)
        {
            try
            {
                switch (selectorStrategy)
                {
                    case SelectorStrategy.PartialLink:
                        return webDriver.FindElement(By.PartialLinkText(domItem));
                }
            }
            catch
            {
                
            }
            return null;
        }

        public virtual string GetElementValue(string idToFind)
        {
            IWebElement element = FindById(idToFind);

            if (!ReferenceEquals(null, element))
            {
                return element.GetAttribute("value");
            }

            return string.Empty;
        }

        public virtual ReadOnlyCollection<string> GetSelectedOptions(string idToFind)
        {
            IList<string> selectedOptions = new List<string>();

            IWebElement element = FindById(idToFind);

            if (!ReferenceEquals(null, element))
            {
                ReadOnlyCollection<IWebElement> options = element.FindElements(By.TagName("option"));

                foreach (IWebElement option in options)
                {
                    if (option.Selected)
                    {
                        selectedOptions.Add(option.Text);
                    }
                }
            }

            return new ReadOnlyCollection<string>(selectedOptions);
        }

        public virtual bool IsElementChecked(string idToFind)
        {
            IWebElement element = FindById(idToFind);

            if (!ReferenceEquals(null, element))
            {
                return element.GetAttribute("checked").Equals("checked");
            }

            return false;
        }

        public virtual bool IsElementSelected(string idToFind)
        {
            IWebElement element = FindById(idToFind);

            if (!ReferenceEquals(null, element))
            {
                return element.Selected;
            }

            return false;
        }


        public virtual bool IsElementEnabled(string idToFind)
        {
            IWebElement element = FindById(idToFind);

            if (!ReferenceEquals(null, element))
            {
                return element.Enabled;
            }

            return false;
        }

        public virtual bool IsElementPresentById(string idToFind)
        {
            return !ReferenceEquals(null, FindById(idToFind));
        }

        public virtual bool IsElementPresentByName(string nameToFind)
        {
            try
            {
                return !ReferenceEquals(null, FindByName(nameToFind));
            }
            catch
            {
                return false;
            }
        }

        public virtual bool IsOptionPresent(string nameToFind, string optionText)
        {
            IWebElement element = FindByName(nameToFind);

            if (!ReferenceEquals(null, element))
            {
                ReadOnlyCollection<IWebElement> options = element.FindElements(By.TagName("option"));

                foreach (IWebElement option in options)
                {
                    if (option.Text.Equals(optionText))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public virtual bool IsValuePresent(string nameToFind, string attribute, string attributeValue)
        {
            ReadOnlyCollection<IWebElement> elements = webDriver.FindElements(By.Name(nameToFind));

            if (!ReferenceEquals(null, elements))
            {
                return elements.Any(element => element.GetAttribute(attribute).Equals(attributeValue, StringComparison.OrdinalIgnoreCase));
            }

            return false;
        }

        public virtual IWebElement TypeTextWithEnter(string nameToFind, string text)
        {
            IWebElement element = FindByNameClick(nameToFind);

            if (!ReferenceEquals(null, element))
            {
                TypeText(element, text);

                Thread.Sleep(500);

                element.SendKeys(Keys.Return);
            }

            return element;
        }

        public virtual void TypeText(string nameToFind, string text)
        {
            IWebElement element = FindByName(nameToFind);

            if (!ReferenceEquals(null, element))
            {
                TypeText(element, text);
            }
        }

        public virtual void TypeText(By selectStrategy, string text)
        {
            IWebElement element = FindWebElementWithWait(selectStrategy, WaitTimeout);

            if (!ReferenceEquals(null, element))
            {
                TypeText(element, text);
            }
        }

        public virtual void ClearTextFromInput(string nameOfInputElement)
        {
            IWebElement element = FindByName(nameOfInputElement);

            if (!ReferenceEquals(null, element))
            {
                element.Clear();
            }
        }

        public virtual void Quit()
        {
            webDriver.Quit();
            webDriver.Dispose();
        }

        public virtual string GetAlertText()
        {
            var alert = webDriver.SwitchTo().Alert();
            var alertText = alert.Text;
            alert.Accept();

            return alertText;
        }

        public virtual void Wait(int milliseconds = 1000)
        {
            Thread.Sleep(milliseconds);
        }

        private IWebElement FindWebElementWithWait(By findBy, int seconds)
        {
            return WaitForElement(findBy, seconds);
        }

        private IWebElement WaitForElement(By by, int seconds)
        {
            var wait = new WebDriverWait(WebDriver, TimeSpan.FromSeconds(seconds));
            return wait.Until(ExpectedConditions.ElementIsVisible(by));
        }

        public void SwitchToWindow(string windowName)
        {
            webDriver.SwitchTo().Window(windowName);
        }

        public void TypeTextAndTabAway(string nameToFind, string text)
        {
            var element = FindByNameClick(nameToFind);

            if (!ReferenceEquals(null, element))
            {
                TypeText(element, text);
                element.SendKeys(Keys.Tab);
            }
        }

        private void TypeText(IWebElement element, string text)
        {
            element.Clear();
            element.SendKeys(text);
        }


        public void TabAwayFromInput(By selectStrategy)
        {
            IWebElement element = webDriver.FindElement(selectStrategy);

            element.SendKeys(Keys.Tab);
        }

        //private IWebElement RetryingFind(By by)
        //{
        //    IWebElement element = null;
        //    int attempts = 0;
        //    while (attempts < 50)
        //    {
        //        Thread.Sleep(100);
        //        try
        //        {
        //            element = webDriver.FindElement(by);
        //            break;
        //        }
        //        catch (Exception e)
        //        {
        //        }
        //        attempts++;
        //    }
        //    return element;
        //}

        //private bool RetryingFindClick(By by)
        //{
        //    bool result = false;
        //    int attempts = 0;
        //    while (attempts < 20)
        //    {
        //        try
        //        {
        //            webDriver.FindElement(by).Click();
        //            result = true;
        //            break;
        //        }
        //        catch (Exception e)
        //        {
        //        }
        //        attempts++;
        //    }
        //    return result;
        //}


        public IWebDriver WebDriver
        {
            get { return webDriver; }
        }

        public ReadOnlyCollection<string> Windows
        {
            get { return webDriver.WindowHandles; }
        }

    }
}