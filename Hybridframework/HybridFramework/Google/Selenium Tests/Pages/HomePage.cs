using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.PageObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Google.Selenium_Tests.Pages
{
    public class HomePage : BasePage
    {
        [CacheLookup]
        private IWebElement? SearchBox => Driver?.FindElement(By.Name("q")); //Finding locator in one way

        [CacheLookup]
        private IWebElement? SearchButton => Driver?.FindElement(By.Name("btnK"));

        public HomePage(IWebDriver driver) : base(driver) { }

        public void EnterSearchText(string? searchText)
        {
            SearchBox?.SendKeys(searchText);
        }
        public void SearchButtonClick()
        {
            DefaultWait<IWebDriver> fluentwait = new DefaultWait<IWebDriver>(Driver);
            fluentwait.Timeout = TimeSpan.FromSeconds(5);
            fluentwait.PollingInterval = TimeSpan.FromMilliseconds(250);
            fluentwait.IgnoreExceptionTypes(typeof(NoSuchElementException));
            fluentwait.Message = "Element not found";
            fluentwait.Until(x => x.FindElement(By.Name("btnK")));
            SearchButton?.Click();

        }

    }
}
