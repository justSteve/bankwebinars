using System.Collections.ObjectModel;
using CUWebinars.Selenium.Core.Enums;
using OpenQA.Selenium;

namespace CUWebinars.Selenium.Core
{
    public interface ITestDriver
    {
        string CurrentWindowHandle { get; }
        string DriverPath { set; }
        int DriverPort { set; }

        void ClearFederatedCookies();
        void ClearTextFromInput(string nameOfInputElement);
        void CloseWindow();
        bool DoesElementContainText(string nameToFind, string text);
        IWebElement FindByClassName(string classNameToFind);
        IWebElement FindByClassNameClick(string classNameToFind);
        IWebElement FindByCssSelectorClick(string cssSelectorToFind);
        IWebElement FindByCssSelector(string cssSelectorToFind);
        IWebElement FindById(string idToFind);
        IWebElement FindByIdClick(string idToFind);
        IWebElement FindByIdWithWait(string idToFind, int seconds);
        IWebElement FindByLinkText(string linkTextToFind);
        IWebElement FindByLinkTextClick(string linkTextToFind);
        IWebElement FindByName(string nameToFind);
        IWebElement FindByNameClick(string nameToFind);
        IWebElement FindByPartialLinkText(string linkTextToFind);
        IWebElement FindByXPath(string xpathToFind);
        IWebElement FindByXPathClick(string xpathToFind);
        IWebElement FindElementWithoutWait(SelectorStrategy selectorStrategy, string domItem);
        void HoverOverElement(SelectorStrategy selectorStrategy, string selectorText);
        void HoverOverElementUsingJavascript(string javascriptText);
        string GetAlertText();
        string GetDocumentTitle();
        string GetElementValue(string idToFind);
        void GoToUrl(string url);
        ReadOnlyCollection<string> GetSelectedOptions(string idToFind);
        void Initialize();
        bool IsElementChecked(string idToFind);
        bool IsElementEnabled(string idToFind);
        bool IsElementPresentById(string idToFind);
        bool IsElementPresentByName(string nameToFind);
        bool IsElementSelected(string idToFind);
        bool IsOptionPresent(string nameToFind, string optionText);
        bool IsValuePresent(string nameToFind, string attributeToFind, string attributeValue);
        void Quit();
        void SwitchToWindow(string windowName);
        void TabAwayFromInput(By selectStrategy);
        void TypeText(string nameToFind, string text);
        void TypeText(By selectStrategy, string text);
        void TypeTextAndTabAway(string nameToFind, string text);
        IWebElement TypeTextWithEnter(string nameToFind, string text);
        void Wait(int milliseconds = 1000);
        IWebDriver WebDriver { get; }
        ReadOnlyCollection<string> Windows { get; }
    }
}
