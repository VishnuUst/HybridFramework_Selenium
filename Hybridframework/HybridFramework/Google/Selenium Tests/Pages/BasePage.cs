using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Google.Selenium_Tests.Pages
{
    public class BasePage
    {
        //instance variables

        protected IWebDriver Driver;
        protected ExtentReports Extent;
        protected ExtentTest Test;
        private ExtentSparkReporter SparkReporter;

        private Dictionary<string, string> properties;
        protected string? currdir;
        protected string? url;

        //overloaded constructor
        protected BasePage()
        {
            currdir = Directory.GetParent(@"../../../")?.FullName;
        }

        public BasePage(IWebDriver driver)
        {
            Driver = driver;
        }

        //common page method etc..

        private void ReadConfiguration()
        {
            
            properties = new Dictionary<string, string>();
            string fileName = currdir + "/configsettings/config.properties";
            string[] lines = File.ReadAllLines(fileName);
            foreach (string line in lines)
            {
                if (!string.IsNullOrEmpty(line) && line.Contains('='))
                {
                    string[] parts = line.Split('=');
                    string key = parts[0].Trim();
                    string value = parts[1].Trim();
                    properties[key] = value;
                }
            }
        }

        protected void TakeScreenShot()
        {
            ITakesScreenshot screenshot = (ITakesScreenshot)Driver;
            Screenshot screenshot1 = screenshot.GetScreenshot();
            string currDir = Directory.GetParent(@"../../../").FullName;
            string filepath = currDir + "/Screenshots/scs_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png";
            screenshot1.SaveAsFile(filepath);

        }
        protected static void ScrollIntoView(IWebDriver driver, IWebElement element)
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            js.ExecuteScript("arguments[0].scrollIntoView(true);", element);
        }

        protected void LogTestResults(string? testName,string? type,string? result,string? erroMessage=null)
        {
            if(type.ToLower().Equals("info"))
            {
                Log.Information(result);
                Test?.Info(result);

            }
            else if (type.ToLower().Equals("pass") && erroMessage == null)
            {
                Log.Information(testName + "Passed");
                Log.Information("************");
                Test?.Pass(result);
            }
            else
            {
                Log.Error($"Test failed for {testName}. \n Exception: {erroMessage}");
                Log.Information("************");
                var screenshot = ((ITakesScreenshot)Driver).GetScreenshot().AsBase64EncodedString;
                Test?.AddScreenCaptureFromBase64String(screenshot);

                Test?.Fail(result);
            }
        }
        protected void Intializevrowser()
        {
            
            ReadConfiguration();
            if (properties["browser"].ToLower() == "chrome")
            {
                Driver = new ChromeDriver();
            }
            else if (properties["browser"].ToLower() == "edge")
            {
                Driver = new EdgeDriver();
            }
            url = properties["baseUrl"];
            Driver.Url = url;
            Driver.Manage().Window.Maximize();

        }

        [OneTimeSetUp]
        public void SetUp()
        {
            Intializevrowser();

            //Configure seriLog
           
            string? logfilepath = currdir + "/Logs/log_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".txt";
            Log.Logger = new LoggerConfiguration().
                WriteTo.File(logfilepath, rollingInterval: RollingInterval.Day)
                .CreateLogger();

            //Configure extent reports

            Extent = new ExtentReports();
            SparkReporter = new ExtentSparkReporter(currdir + "/Reports/extent-report" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".html");
            Extent.AttachReporter(SparkReporter);
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            
            Driver.Quit();
            Extent.Flush();
            Log.CloseAndFlush();

        }
    }
}
