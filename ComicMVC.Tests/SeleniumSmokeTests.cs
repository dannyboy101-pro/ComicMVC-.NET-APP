using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Net.Http;

namespace ComicMVC.Tests
{
    /*
     * SeleniumSmokeTests
     *
     * These tests check that the main pages of the web application can be reached
     * in a browser. They are smoke tests rather than deep functional tests.
     *
     * Important:
     * The ComicMVC web application must already be running before these tests start.
     * The base URL can be changed below if the application uses a different port.
     */

    [TestClass]
    public class SeleniumSmokeTests
    {
        private IWebDriver _driver = null!;
        private string _baseUrl = null!;

        [TestInitialize]
        public void Setup()
        {
            
            _baseUrl = Environment.GetEnvironmentVariable("COMICMVC_BASEURL")
                       ?? "https://localhost:7289";

            var options = new ChromeOptions();
            options.AddArgument("--headless=new");
            options.AddArgument("--disable-gpu");
            options.AddArgument("--window-size=1400,900");
            options.AddArgument("--ignore-certificate-errors");

            _driver = new ChromeDriver(options);

            
            Assert.IsTrue(IsSiteAvailable(_baseUrl),
                $"The application is not reachable at {_baseUrl}. Start ComicMVC before running Selenium tests.");
        }

        [TestCleanup]
        public void Cleanup()
        {
            try { _driver.Quit(); } catch { }
            try { _driver.Dispose(); } catch { }
        }

        [TestMethod]
        public void HomePage_Loads_And_ShowsTitle()
        {
            _driver.Navigate().GoToUrl($"{_baseUrl}/");

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

        private bool IsSiteAvailable(string url)
        {
            try
            {
                using var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(3);

                var response = client.GetAsync(url).GetAwaiter().GetResult();
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}