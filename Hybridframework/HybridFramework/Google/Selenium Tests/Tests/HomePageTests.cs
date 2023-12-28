using Google.Selenium_Tests.Pages;
using Google.Test_DataClasses;
using Google.Utilities;
using OpenQA.Selenium;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Google.Selenium_Tests.Tests
{
    [TestFixture]
    public class HomePageTests : BasePage
    {
        private HomePage homePage;
        string? testName;

        [SetUp]
        public void BeforeSetup()
        {
            homePage = new HomePage(Driver);
        }
        [Test]
        [Author("vishnu T","vishnu.thulaseedharanpillai@ust.com")]
        [Category("Home Page test for Google")]
        public void GoogleSearchTest()
        {

            testName = "Google Search Test";
            Log.Information(testName);
            Log.Information("************************************************");
            Test = Extent.CreateTest(testName);


            string? excelFilePath = currdir + "/TestData/GoogleSearch.xlsx";
            string? sheetName = "SearchData";

            List<SearchData> excelDataList = SearchDataRead.ReadSearchText(excelFilePath, sheetName);

            foreach (var excelData in excelDataList)
            {
                string? searchText = excelData.SearchText;


                LogTestResults(testName, "Info", "Starting the Google search test");

                LogTestResults(testName, "Info", "Opened Google homepage");

                homePage.EnterSearchText(searchText);
                Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2);
                TakeScreenShot();
                LogTestResults(testName, "Info", $"Entered search text: {searchText}");

                homePage.SearchButtonClick();
                Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2);
                TakeScreenShot();
                LogTestResults(testName, "Info", "Clicked on the Search button");

                try
                {

                    // Assume the title contains the search text for simplicity
                    Assert.That(Driver.Title.Contains(searchText), Is.True, $"Search results page title does not contain '{searchText}'");
                    var screenshot = ((ITakesScreenshot)Driver).GetScreenshot().AsBase64EncodedString;
                    Test?.AddScreenCaptureFromBase64String(screenshot);
                    LogTestResults(testName, "Info", "Google search test completed");
                    LogTestResults(testName, "pass", testName + " - Passed");
                }
                catch (Exception ex)
                {
                    var screenshot = ((ITakesScreenshot)Driver).GetScreenshot().AsBase64EncodedString;
                    Test?.AddScreenCaptureFromBase64String(screenshot);
                    LogTestResults(testName, "fail", testName + " - Failed", ex.Message);
                }
                finally
                {
                    Driver.Navigate().GoToUrl(url);
                }
                // Additional assertions or actions if needed
            }
        }

        [TearDown]
        public void AfterTest()
        {
            Driver.Navigate().GoToUrl(url);
            Log.Information("************************************************");
        }

    }
}
