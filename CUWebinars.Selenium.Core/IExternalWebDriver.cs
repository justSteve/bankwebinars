namespace CUWebinars.Selenium.Core
{
    public interface IExternalWebDriver
    {
        int DriverPort { set; }
        void Initialize();
    }
}