using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using NLogger = NLog.Logger;
using OpenQA.Selenium.DevTools;


namespace YouTubeAutomation
{
    public class YouTubeAutomation
    {
        private IWebDriver driver;
        private WebDriverWait wait;
        private static readonly NLogger Logger = LogManager.GetCurrentClassLogger();

        static YouTubeAutomation()
        {
            LogManager.Setup().LoadConfigurationFromFile(@"C:\Users\USER\Documents\Selenium Scripts\YouTubeAutomation\src\Config\NLog.config");
        }

        public YouTubeAutomation()
        {
            var options = new ChromeOptions();
            driver = new ChromeDriver(@"C:\Users\USER\Downloads\chromedriver-win64\chromedriver-win64", options);
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

        }
        public YouTubeAutomation(IWebDriver driver)
        {
            this.driver = driver;
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        public void SearchYouTube(string query)
        {
            Logger.Info("Navigating to YouTube and searching for: " + query);
            driver.Navigate().GoToUrl("https://www.youtube.com");
            var searchBox = wait.Until(d => d.FindElement(By.Name("search_query")));
            searchBox.SendKeys(query);
            searchBox.Submit();
        }

        public void ApplyFilter(string filterType)
        {
            try
            {
                Logger.Info("Applying filter: " + filterType);
                var filterButton = wait.Until(d => d.FindElement(By.XPath($"//yt-formatted-string[text()='{filterType}']")));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", filterButton);
                wait.Until(d => filterButton.Displayed && filterButton.Enabled);
                System.Threading.Thread.Sleep(500);

                try
                {
                    filterButton.Click();
                }
                catch (ElementClickInterceptedException)
                {
                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", filterButton);
                }
            }
            catch (WebDriverTimeoutException ex)
            {
                Console.WriteLine($"Timeout while waiting for filter button: {ex.Message}");
                Logger.Error($"Timeout while waiting for filter button: {ex.Message}");
            }
            catch (ElementNotInteractableException ex)
            {
                Console.WriteLine($"Element not interactable: {ex.Message}");
                Logger.Error($"Element not interactable: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while applying filter: {ex.Message}");
                Logger.Error($"An error occurred while applying filter: {ex.Message}");
            }
        }
        public List<string> ListResults()
        {
            Logger.Info("Listing search results...");
            var results = new List<string>();
            var videoElements = wait.Until(d => d.FindElements(By.XPath("//ytd-video-renderer"))).Take(5).ToList();

            for (int index = 0; index < videoElements.Count; index++)
            {
                var video = videoElements[index];
                string title = "";
                string duration = "Duration not available";
                string channel = "";
                string views = "";
                string uploadDate = "";

                try
                {
                    title = video.FindElement(By.Id("video-title")).Text;
                }
                catch (StaleElementReferenceException)
                {
                    video = wait.Until(d => d.FindElements(By.XPath("//ytd-video-renderer"))).ElementAt(index);
                    title = video.FindElement(By.Id("video-title")).Text;
                }

                try
                {
                    var durationElement = video.FindElements(By.XPath(".//ytd-thumbnail-overlay-time-status-renderer//div[contains(@class, 'badge-shape-wiz__text')]")).FirstOrDefault();
                    if (durationElement != null)
                    {
                        duration = durationElement.Text;
                    }
                }
                catch (NoSuchElementException)
                {
                    duration = "Duration not available";
                }
                catch (WebDriverTimeoutException)
                {
                    duration = "Duration not available";
                }

                try
                {
                    
                    channel = video.FindElement(By.XPath(".//yt-formatted-string[@class='style-scope ytd-channel-name']//a")).Text;
                }
                catch (NoSuchElementException)
                {
                    channel = "Channel not available";
                }

                try
                {
                    views = video.FindElement(By.XPath(".//div[@id='metadata-line']/span[1]")).Text;
                }
                catch (NoSuchElementException)
                {
                    views = "Views not available";
                }

                try
                {
                    uploadDate = video.FindElement(By.XPath(".//div[@id='metadata-line']/span[2]")).Text;
                }
                catch (NoSuchElementException)
                {
                    uploadDate = "Upload date not available";
                }

                results.Add($"{index + 1}.\n Title: {title}\n   Duration: {duration}\n   Channel: {channel}\n   Views: {views}\n   Uploaded: {uploadDate}\n");
                Logger.Info($"Result {index + 1}: Title: {title}, Duration: {duration}, Channel: {channel}, Views: {views}, Uploaded: {uploadDate}");
                Console.WriteLine("\n");
            }

            return results;
        }

        public void PlayVideo(int selection)
        {
            Logger.Info("Playing video selection: " + selection);
            var videoElements = driver.FindElements(By.XPath("//ytd-video-renderer"));
            if (selection < 1 || selection > videoElements.Count)
            {
                Logger.Error("Selection is out of range.");
                throw new ArgumentOutOfRangeException("Selection is out of range.");

            }

            var videoTitle = videoElements[selection - 1].FindElement(By.Id("video-title"));

            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", videoTitle);
            wait.Until(d => videoTitle.Displayed && videoTitle.Enabled);

            try
            {

                videoTitle.Click();
                Logger.Info("Video clicked.");

            }
            catch (ElementClickInterceptedException)
            {
                Logger.Warn("Element click intercepted. Trying to click again...");
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", videoTitle);

            }
            catch (ElementNotInteractableException ex)
            {
                Logger.Error($"Element not interactable: {ex.Message}");
                Console.WriteLine($"Element not interactable: {ex.Message}");
                return;
            }

            wait.Until(d => d.FindElement(By.XPath("//video")).Displayed);
            try
            {
                Logger.Info("Waiting for ad to finish...");
                var skipAdButton = wait.Until(d => d.FindElement(By.XPath("//button[contains(@class, 'ytp-ad-skip-button')]")));
                Console.WriteLine("An ad is playing. Press any key to skip it...");

                Console.ReadKey();
                skipAdButton.Click();
                Logger.Info("Ad skipped.");
            }
            catch (WebDriverTimeoutException)
            {
                Logger.Info("No ad detected. Video is playing...");
                Console.WriteLine("No ad detected. Video is playing...");
            }
            try
            {
                var playButton = wait.Until(d => d.FindElement(By.XPath("//button[@class='ytp-large-play-button ytp-button']")));
                playButton.Click();
                Logger.Info("Play button clicked.");
            }
            catch (ElementNotInteractableException ex)
            {
                Logger.Error($"Play button not interactable: {ex.Message}");
                Console.WriteLine($"Play button not interactable: {ex.Message}");
            }


            EnterFullScreenMode();
        }
        private void EnterFullScreenMode()
        {
            try
            {
                Logger.Info("Entering full-screen mode...");
                var fullScreenButton = wait.Until(d => d.FindElement(By.XPath("//button[@class='ytp-fullscreen-button ytp-button']")));
                fullScreenButton.Click();
                Logger.Info("Full-screen button clicked.");

            }
            catch (ElementNotInteractableException ex)
            {
                Logger.Error($"Full-screen button not interactable: {ex.Message}");
                Console.WriteLine($"Full-screen button not interactable: {ex.Message}");
            }
            catch (NoSuchElementException ex)
            {
                Logger.Error($"Full-screen button not found: {ex.Message}");
                Console.WriteLine($"Full-screen button not found: {ex.Message}");
            }
        }
        public bool IsVideoPlaying()
        {
            try
            {
                Logger.Info("Checking if video is playing...");
                var playButton = driver.FindElement(By.XPath("//button[@class='ytp-play-button ytp-button']"));
                return playButton.GetAttribute("aria-label").Contains("Pause");
            }
            catch (NoSuchElementException)
            {
                Logger.Error("Play button not found.");
                return false;
            }
        }
        public void Close()
        {
            Logger.Info("Closing the browser...");
            driver.Quit();
        }
    }
}