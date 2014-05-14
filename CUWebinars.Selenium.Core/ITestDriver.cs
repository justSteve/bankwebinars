using System.Collections.ObjectModel;
using OpenQA.Selenium;

namespace CUWebinars.Selenium.Core
{
    public interface ITestDriver
    {
        string DriverPath { set; }
        int DriverPort { set; }

        void ClearFederatedCookies();
        void ClearTextFromInput(string nameOfInputElement);
        void CloseWindow();
        bool DoesElementContainText(string nameToFind, string text);
        ReadOnlyCollection<IWebElement> FindByClassName(string classNameToFind);
        IWebElement FindByClassNameClick(string classNameToFind);
        IWebElement FindByCssSelectorClick(string cssSelectorToFind);
        IWebElement FindByCssSelector(string cssSelectorToFind);
        IWebElement FindById(string idToFind);
        IWebElement FindByIdClick(string idToFind);
        IWebElement FindByLinkText(string linkTextToFind);
        IWebElement FindByLinkTextClick(string linkTextToFind);
        IWebElement FindByPartialLinkText(string linkTextToFind);
        IWebElement FindByXPath(string xpathToFind);
        IWebElement FindByXPathClick(string xpathToFind);
        IWebElement FindByName(string nameToFind);
        IWebElement FindByNameClick(string nameToFind);
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
        void SelectANode(string classNameToFind, string attributeValue);
        void TabAwayFromInput(string idOfInput);
        IWebElement TypeTextWithEnter(string nameToFind, string text);
        void TypeText(string nameToFind, string text);
        void TypeTextAndTabAway(string nameToFind, string text);
        void Wait(int milliseconds = 1000);
        IWebDriver WebDriver { get; }
    }
}
