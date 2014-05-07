using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CUWebinars.Selenium.Core;
using OpenQA.Selenium;

namespace CUWebinars.CitrixDriver
{
    public abstract class BaseTestDriver : ITestDriver
    {
        protected IWebDriver webDriver;
        protected string port;

        public BaseTestDriver()
        {

        }

        public string DriverPath { set; protected get; }
        public int DriverPort { set; protected get; }

        public abstract void Initialize();

        public virtual void ClearCookies()
        {
            //webDriver.Manage().Cookies.DeleteCookieNamed("FedAuth");
            //webDriver.Manage().Cookies.DeleteCookieNamed("FedAuth1");
        }

        public virtual void CloseWindow()
        {
            webDriver.Close();
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
                url = "http://www.gotomeeting.com/online/";
            }

            INavigation navigation = webDriver.Navigate();
            navigation.GoToUrl(url);
        }

        public virtual IWebElement FindByClassNameClick(string classNameToFind)
        {
            IWebElement element = webDriver.FindElement(By.ClassName(classNameToFind));

            if (!ReferenceEquals(null, element))
            {
                element.Click();
            }

            return element;
        }

        public virtual ReadOnlyCollection<IWebElement> FindByClassName(string classNameToFind)
        {
            ReadOnlyCollection<IWebElement> elements = webDriver.FindElements(By.ClassName(classNameToFind));

            return elements;
        }

        public virtual IWebElement FindByCssSelectorClick(string cssSelectorToFind)
        {
            IWebElement element = webDriver.FindElement(By.CssSelector(cssSelectorToFind));

            if (!ReferenceEquals(null, element))
            {
                element.Click();
            }

            return element;
        }

        public virtual IWebElement FindByCssSelector(string cssSelectorToFind)
        {
            return webDriver.FindElement(By.CssSelector(cssSelectorToFind));
        }

        public virtual IWebElement FindById(string idToFind)
        {
            return webDriver.FindElement(By.Id(idToFind));
        }

        public virtual IWebElement FindByIdClick(string idToFind)
        {
            IWebElement element = webDriver.FindElement(By.Id(idToFind));

            if (!ReferenceEquals(null, element))
            {
                element.Click();
            }

            return element;
        }

        public virtual IWebElement FindByLinkTextClick(string linkTextToFind)
        {
            IWebElement element = webDriver.FindElement(By.LinkText(linkTextToFind));

            if (!ReferenceEquals(null, element))
            {
                element.Click();
            }

            return element;
        }

        public virtual IWebElement FindByXPath(string xpathToFind)
        {
            IWebElement element = webDriver.FindElement(By.XPath(xpathToFind));

            return element;
        }

        public virtual IWebElement FindByXPathClick(string xpathToFind)
        {
            IWebElement element = webDriver.FindElement(By.XPath(xpathToFind));

            if (!ReferenceEquals(null, element))
            {
                element.Click();
            }

            return element;
        }

        public virtual IWebElement FindByName(string nameToFind)
        {
            return webDriver.FindElement(By.Name(nameToFind));
        }

        public virtual IWebElement FindByNameClick(string nameToFind)
        {
            IWebElement element = webDriver.FindElement(By.Name(nameToFind));

            if (!ReferenceEquals(null, element))
            {
                element.Click();
            }

            return element;
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
            return null != FindById(idToFind);
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
                foreach (IWebElement element in elements)
                {
                    if (element.GetAttribute(attribute).Equals(attributeValue, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public virtual IWebElement TypeTextWithEnter(string nameToFind, string text)
        {
            IWebElement element = FindByNameClick(nameToFind);

            if (!ReferenceEquals(null, element))
            {
                TypeText(element, text);

                Thread.Sleep(1000);

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
            System.Threading.Thread.Sleep(milliseconds);
        }

        public virtual void SelectANode(string classNameToFind, string attributeValue)
        {
            ReadOnlyCollection<IWebElement> elements = FindByClassName(classNameToFind);

            if (!ReferenceEquals(null, elements))
            {
                foreach (IWebElement element in elements)
                {
                    if (element.Text.Trim().Equals(attributeValue, StringComparison.OrdinalIgnoreCase))
                    {
                        element.Click();
                        break;
                    }
                }
            }
        }

        public void TypeTextAndTabAway(string nameToFind, string text)
        {
            IWebElement element = FindByName(nameToFind);

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


        public void TabAwayFromInput(string idOfInput)
        {
            IWebElement element = FindById(idOfInput);

            element.SendKeys(Keys.Tab);
        }


        public IWebDriver WebDriver
        {
            get { return webDriver; }
        }
    }

}
