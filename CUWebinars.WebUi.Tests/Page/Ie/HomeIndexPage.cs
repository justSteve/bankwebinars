using CUWebinars.Selenium.Core;
using CUWebinars.WebUi.Tests.Infrastructure;
using OpenQA.Selenium;

namespace CUWebinars.WebUi.Tests.Page.Ie
{
    public class HomeIndexPage : HomeIndexBasePage
    {
        public HomeIndexPage(ITestDriver seleniumTestDriver)
            : base(seleniumTestDriver)
        {
        }

        public override void LogOff()
        {
            SeleniumTestDriver.FindByPartialLinkText(Constants.LogoffLinkText).SendKeys(Keys.Enter);
        }


        public override bool ClickTopicsMenuItem()
        {
            SeleniumTestDriver.HoverOverElementUsingJavascript("$('#main_menu ul li.parent:eq(0) a').mouseenter();");

            SeleniumTestDriver.FindByXPathClick(@"//*[@id='main_menu']/ul/li[2]/ul/li[3]/a");

            return SeleniumTestDriver.FindByXPath("//*[@id='page']/div[1]/div/h1").Displayed;
        } 
        public override bool ClickUpcomingEventsMenuItem()
        {
            SeleniumTestDriver.HoverOverElementUsingJavascript("$('#main_menu ul.primary_menu li.parent:eq(1) a').mouseenter();");

            SeleniumTestDriver.FindByXPathClick(@"//*[@id='main_menu']/ul/li[3]/ul/li[3]/a");

            return SeleniumTestDriver.FindById("webinarContent").Displayed;
        }


    }
}
