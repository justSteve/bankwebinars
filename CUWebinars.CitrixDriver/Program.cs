// Please visit http://docs.seleniumhq.org/docs/03_webdriver.jsp#getting-started-with-webdriver for detailed installation and instructions
//Quick start video tutorial using Visual Studio: https://www.youtube.com/watch?v=CxDkRJ1iHwE
//Step-by-step video turoial using Visual Studio: https://www.youtube.com/watch?v=uRJL0zu7U6k

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Drawing;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;

namespace SimpleCSharpSelenium2
{
    class Program
    {
        static void Main(string[] args)
        {
            DesiredCapabilities caps = new DesiredCapabilities();

            caps.SetCapability("name", "IE8 Select Radio");
            caps.SetCapability("build", "1.0");
            caps.SetCapability("browser_api_name", "IE8");
            caps.SetCapability("os_api_name", "Win7-C1");
            caps.SetCapability("screen_resolution", "1024x768");
            caps.SetCapability("record_video", "true");
            caps.SetCapability("record_network", "true");
            caps.SetCapability("record_snapshot", "false");

            caps.SetCapability("username", "steve@ttstrain.com");
            caps.SetCapability("password", "u4dd2077d8c398e7");

            IWebDriver driver = new RemoteWebDriver(new Uri("http://hub.crossbrowsertesting.com:80/wd/hub"), caps);

            Console.WriteLine("Loading Url");
            driver.Navigate().GoToUrl("http://v3.bankwebinars.com/1893/all-about-the-new-ffiec-cybersecurity-assessment-tool");
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            wait.Until((d) => { return d.Title.ToLower().StartsWith("all"); });

            driver.FindElement(By.Id("regTypeID_208")).Click();
            Thread.Sleep(8000);
            //maximize the window - DESKTOPS ONLY
            //Console.WriteLine("Maximizing window");
            //driver.Manage().Window.Size = new Size(1024,768);

            driver.Quit();

        }
    }
}