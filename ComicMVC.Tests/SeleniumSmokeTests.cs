using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;

namespace ComicMVC.Tests
{
    [TestClass]
    public class SeleniumSmokeTests
    {
        private IWebDriver _driver = null!;
        private string _baseUrl = null!;

        [TestInitialize]
        public void Setup()
        {
            // IMPORTANT: run the MVC app first (F5) so https://localhost:7289 is live
            _baseUrl = "https://localhost:7289";

            var options = new ChromeOptions();
            options.AddArgument("--headless=new");
            options.AddArgument("--disable-gpu");
            options.AddArgument("--window-size=1400,900");
            options.AddArgument("--ignore-certificate-errors"); // avoids dev cert issues

            _driver = new ChromeDriver(options);
        }

        [TestCleanup]
        public void Cleanup()
        {
            try { _driver.Quit(); } catch { /* ignore */ }
            try { _driver.Dispose(); } catch { /* ignore */ }
        }

        [TestMethod]
        public void HomePage_Loads_And_ShowsTitle()
        {
            _driver.Navigate().GoToUrl($"{_baseUrl}/");

            // wait for page to load (simple explicit wait)
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(5));
            wait.Until(d => !string.IsNullOrWhiteSpace(d.Title));

            StringAssert.Contains(_driver.Title, "Comic");
        }

        [TestMethod]
        public void LoginPage_Loads()
        {
            _driver.Navigate().GoToUrl($"{_baseUrl}/Identity/Account/Login");

            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(5));
            wait.Until(d => d.FindElements(By.TagName("h1")).Count > 0);

            var h1 = _driver.FindElement(By.TagName("h1")).Text;
            StringAssert.Contains(h1, "Log in");
        }

        [TestMethod]
        public void ComicsPage_Loads()
        {
            _driver.Navigate().GoToUrl($"{_baseUrl}/Comics");

            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(5));
            wait.Until(d => d.PageSource.Contains("Comic Encyclopedia"));

            StringAssert.Contains(_driver.PageSource, "Comic Encyclopedia");
        }
    }
}