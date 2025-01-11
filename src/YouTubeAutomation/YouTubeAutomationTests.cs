using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace YouTubeAutomation
{
    [TestFixture]
    public class YouTubeAutomationTests
    {
        private YouTubeAutomation youTubeAutomation;
        private IWebDriver driver;
        private string logFilePath = "C:\\Users\\USER\\Documents\\SeleniumLog\\test_log.txt";

        [SetUp]
        public void Setup()
        {
            Directory.CreateDirectory("C:\\Users\\USER\\Documents\\SeleniumLog");
            driver = new ChromeDriver("C:\\Users\\USER\\Downloads\\chromedriver-win64\\chromedriver-win64");
            youTubeAutomation = new YouTubeAutomation(driver);
            LogMessage("Setup completed.");
        }

        [Test]
        public void TestSearchYouTube_ValidQuery_ReturnsResults()
        {
            LogMessage("Starting TestSearchYouTube_ValidQuery_ReturnsResults");
            try
            {
                youTubeAutomation.SearchYouTube("Selenium Tutorial");

                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                wait.Until(driver => driver.FindElements(By.CssSelector("ytd-video-renderer")).Count > 0);

                TakeScreenshot("SearchYouTube_ValidQuery");

                var results = youTubeAutomation.ListResults();
                LogMessage($"Found {results.Count} results.");
                Assert.That(results, Is.Not.Null);
                Assert.That(results, Is.Not.Empty, "Expected results but found none.");
                LogMessage("TestSearchYouTube_ValidQuery_ReturnsResults completed.");
                Console.WriteLine("TestSearchYouTube_ValidQuery_ReturnsResults: PASS");
            }
            catch (Exception ex)
            {
                LogMessage($"TestSearchYouTube_ValidQuery_ReturnsResults failed: {ex.Message}");
                TakeScreenshot("SearchYouTube_ValidQuery_Failed");
                Console.WriteLine("TestSearchYouTube_ValidQuery_ReturnsResults: FAIL");
                throw;
            }
        }

        [Test]
        public void TestSearchYouTube_InvalidQuery_HandlesError()
        {
            LogMessage("Starting TestSearchYouTube_InvalidQuery_HandlesError");
            try
            {
                youTubeAutomation.SearchYouTube(""); 
                TakeScreenshot("SearchYouTube_InvalidQuery");

                var results = youTubeAutomation.ListResults();
                LogMessage($"Found {results.Count} results.");
                Assert.That(results, Is.Empty, "No results expected for empty query.");
                LogMessage("TestSearchYouTube_InvalidQuery_HandlesError completed.");
                Console.WriteLine("TestSearchYouTube_InvalidQuery_HandlesError: PASS");
            }
            catch (Exception ex)
            {
                LogMessage($"TestSearchYouTube_InvalidQuery_HandlesError failed: {ex.Message}");
                TakeScreenshot("SearchYouTube_InvalidQuery_Failed");
                Console.WriteLine("TestSearchYouTube_InvalidQuery_HandlesError: FAIL");
                throw;
            }
        }

        [Test]
        public void TestApplyFilter_Videos_FilterApplied()
        {
            LogMessage("Starting TestApplyFilter_Videos_FilterApplied");
            try
            {
                youTubeAutomation.SearchYouTube("Sri Lanka");

                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(60));
                try
                {
                    wait.Until(driver => driver.FindElement(By.XPath("//yt-formatted-string[text()='Videos']")).Displayed);
                }
                catch (WebDriverTimeoutException ex)
                {
                    LogMessage("Element not found within the timeout period. Logging page source for debugging.");
                    LogMessage(driver.PageSource);
                    LogMessage($"Exception: {ex.Message}");
                    throw;
                }
                catch (NoSuchElementException ex)
                {
                    LogMessage("Element not found. Logging page source for debugging.");
                    LogMessage(driver.PageSource);
                    LogMessage($"Exception: {ex.Message}");
                    throw;
                }

                LogMessage("Search results loaded. Applying filter...");
                youTubeAutomation.ApplyFilter("Videos");
                WebDriverWait waitForFilter = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
                waitForFilter.Until(driver => driver.FindElement(By.XPath("//yt-formatted-string[text()='Videos']")).Displayed);
                LogMessage("Filter applied successfully.");

                TakeScreenshot("ApplyFilter_Videos");

                IWebElement filterAppliedText = driver.FindElement(By.XPath("//yt-formatted-string[text()='Videos']"));
                Assert.That(filterAppliedText.Text.Contains("Videos"), Is.True, "The 'Videos' filter was not applied properly.");

                var results = youTubeAutomation.ListResults();
                Assert.That(results, Is.Not.Empty, "Expected results but found none.");
                Assert.That(results.All(r => r.Contains("Title:")), Is.True, "Not all results are video elements.");

                LogMessage("TestApplyFilter_Videos_FilterApplied completed.");
                Console.WriteLine("TestApplyFilter_Videos_FilterApplied: PASS");
            }
            catch (Exception ex)
            {
                LogMessage($"TestApplyFilter_Videos_FilterApplied failed: {ex.Message}");
                TakeScreenshot("ApplyFilter_Videos_Failed");
                Console.WriteLine("TestApplyFilter_Videos_FilterApplied: FAIL");
                throw;
            }
        }

        [Test]
        public void TestApplyFilter_InvalidFilter_HandlesError()
        {
            LogMessage("Starting TestApplyFilter_InvalidFilter_HandlesError");
            try
            {
                youTubeAutomation.SearchYouTube("Selenium Tutorial");

                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                wait.Until(driver => driver.FindElement(By.XPath("//div[@id='contents']")).Displayed);

                youTubeAutomation.ApplyFilter("Invalid Filter");

                TakeScreenshot("ApplyFilter_InvalidFilter");

                var results = youTubeAutomation.ListResults();
                Assert.That(results, Is.Not.Empty, "Expected results but found none after invalid filter.");
                LogMessage("TestApplyFilter_InvalidFilter_HandlesError completed.");
                Console.WriteLine("TestApplyFilter_InvalidFilter_HandlesError: PASS");
            }
            catch (Exception ex)
            {
                LogMessage($"TestApplyFilter_InvalidFilter_HandlesError failed: {ex.Message}");
                TakeScreenshot("ApplyFilter_InvalidFilter_Failed");
                Console.WriteLine("TestApplyFilter_InvalidFilter_HandlesError: FAIL");
                throw;
            }
        }

        [Test]
        public void TestPlayVideo_ValidSelection_VideoPlays()
        {
            LogMessage("Starting TestPlayVideo_ValidSelection_VideoPlays");
            try
            {
                youTubeAutomation.SearchYouTube("Selenium Tutorial");

                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                wait.Until(driver => driver.FindElement(By.XPath("//div[@id='contents']")).Displayed);

                var results = youTubeAutomation.ListResults();

                if (results.Count > 0)
                {
                    youTubeAutomation.PlayVideo(1);
                    TakeScreenshot("PlayVideo_ValidSelection");
                    Assert.That(youTubeAutomation.IsVideoPlaying(), Is.True);
                }
                else
                {
                    LogMessage("No results found to play.");
                }

                LogMessage("TestPlayVideo_ValidSelection_VideoPlays completed.");
                Console.WriteLine("TestPlayVideo_ValidSelection_VideoPlays: PASS");
            }
            catch (Exception ex)
            {
                LogMessage($"TestPlayVideo_ValidSelection_VideoPlays failed: {ex.Message}");
                TakeScreenshot("PlayVideo_ValidSelection_Failed");
                Console.WriteLine("TestPlayVideo_ValidSelection_VideoPlays: FAIL");
                throw;
            }
        }

        [Test]
        public void TestPlayVideo_InvalidSelection_ThrowsException()
        {
            LogMessage("Starting TestPlayVideo_InvalidSelection_ThrowsException");
            try
            {
                youTubeAutomation.SearchYouTube("Selenium Tutorial");

                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                wait.Until(driver => driver.FindElement(By.XPath("//div[@id='contents']")).Displayed);

                Assert.Throws<ArgumentOutOfRangeException>(() => youTubeAutomation.PlayVideo(999));

                TakeScreenshot("PlayVideo_InvalidSelection");
                LogMessage("TestPlayVideo_InvalidSelection_ThrowsException completed.");
                Console.WriteLine("TestPlayVideo_InvalidSelection_ThrowsException: PASS");
            }
            catch (Exception ex)
            {
                LogMessage($"TestPlayVideo_InvalidSelection_ThrowsException failed: {ex.Message}");
                TakeScreenshot("PlayVideo_InvalidSelection_Failed");
                Console.WriteLine("TestPlayVideo_InvalidSelection_ThrowsException: FAIL");
                throw;
            }
        }

        [Test]
        public void TestPlayVideo_AdSkippedIfPresent()
        {
            LogMessage("Starting TestPlayVideo_AdSkippedIfPresent");
            try
            {
                youTubeAutomation.SearchYouTube("Selenium Tutorial");

                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                wait.Until(driver => driver.FindElement(By.XPath("//div[@id='contents']")).Displayed);

                var results = youTubeAutomation.ListResults();
                if (results.Count > 0)
                {
                    youTubeAutomation.PlayVideo(1);
                    TakeScreenshot("PlayVideo_AdSkippedIfPresent");
                }

                LogMessage("TestPlayVideo_AdSkippedIfPresent completed.");
                Console.WriteLine("TestPlayVideo_AdSkippedIfPresent: PASS");
            }
            catch (Exception ex)
            {
                LogMessage($"TestPlayVideo_AdSkippedIfPresent failed: {ex.Message}");
                TakeScreenshot("PlayVideo_AdSkippedIfPresent_Failed");
                Console.WriteLine("TestPlayVideo_AdSkippedIfPresent: FAIL");
                throw;
            }
        }


        [Test]
        public void TestDisplaySearchResults_AndSelectIndex()
        {
            LogMessage("Starting TestDisplaySearchResults_AndSelectIndex");
            try
            {
                youTubeAutomation.SearchYouTube("America's Got Talent");
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(40));
                try
                {
                    wait.Until(driver => driver.FindElement(By.XPath("//ytd-video-renderer")).Displayed);
                }
                catch (WebDriverTimeoutException ex)
                {
                    LogMessage("Search results did not load within the timeout period. Logging page source for debugging.");
                    LogMessage(driver.PageSource);
                    LogMessage($"Exception: {ex.Message}");
                    throw;
                }
                var results = youTubeAutomation.ListResults();
                LogMessage($"Found {results.Count} results.");
                if (results.Count == 0)
                {
                    LogMessage("No search results found. Logging page source for debugging.");
                    LogMessage(driver.PageSource);
                }

                Assert.That(results, Is.Not.Empty, "Expected results but found none.");
                int selectedIndex = results.Count > 0 ? 0 : -1; 

                Assert.That(selectedIndex, Is.GreaterThanOrEqualTo(0), "No valid index to select.");
                youTubeAutomation.PlayVideo(selectedIndex + 1); 

                WebDriverWait videoWait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
                videoWait.Until(driver => driver.FindElement(By.CssSelector("video")).Displayed);

                TakeScreenshot("DisplayResults_AndSelectIndex");

                LogMessage("TestDisplaySearchResults_AndSelectIndex completed.");
                Console.WriteLine("TestDisplaySearchResults_AndSelectIndex: PASS");
            }
            catch (Exception ex)
            {
                LogMessage($"TestDisplaySearchResults_AndSelectIndex failed: {ex.Message}");
                TakeScreenshot("DisplayResults_AndSelectIndex_Failed");
                Console.WriteLine("TestDisplaySearchResults_AndSelectIndex: FAIL");
                throw;
            }
        }
        [TearDown]
        public void TearDown()
        {
            driver.Quit();
            LogMessage("TearDown completed.");
        }

        private void TakeScreenshot(string testName)
        {
            Screenshot screenshot = ((ITakesScreenshot)driver).GetScreenshot();
            string filePath = $"C:\\Users\\USER\\Documents\\SeleniumLog\\{testName}_{DateTime.Now:yyyyMMdd_HHmmss}.png";
            screenshot.SaveAsFile(filePath, ScreenshotImageFormat.Png);
            LogMessage($"Screenshot taken: {filePath}");
        }

        private void LogMessage(string message)
        {
            using (StreamWriter writer = new StreamWriter(logFilePath, true))
            {
                writer.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}");
            }
        }
    }
}