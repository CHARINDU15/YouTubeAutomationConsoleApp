using System;
using NLog;
using NLogger = NLog.Logger;

namespace YouTubeAutomation
{
    class Program
    {
        private static readonly NLogger Logger = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            LogManager.Setup().LoadConfigurationFromFile(@"C:\Users\USER\Documents\Selenium Scripts\YouTubeAutomation\src\Config\NLog.config");
            Logger.Info("Starting YouTube Automation...");
            UserInterface ui = new UserInterface();
            YouTubeAutomation youTubeAutomation = new YouTubeAutomation();

            ui.DisplayWelcomeMessage();
            Logger.Info("Displayed welcome message.");

            while (true)
            {
                string query = ui.PromptForSearchQuery();
                Logger.Info($"User entered search query: {query}");
                if (query.Equals("exit", StringComparison.OrdinalIgnoreCase))
                {
                    Logger.Info("User chose to exit the application.");
                    break;
                }

                try
                {
                    youTubeAutomation.SearchYouTube(query);
                    ui.DisplayMessage($"Searching for \"{query}\"...");
                    Logger.Info($"Searching for: {query}");

                    ui.DisplayFilterOptions();
                    int filterSelection = ui.PromptForFilterSelection();
                    string filterType = filterSelection == 1 ? "Videos" : "Shorts";
                    ui.DisplayMessage($"Applying filter: {filterType}...");
                    Logger.Info($"Applying filter: {filterType}");
                    youTubeAutomation.ApplyFilter(filterType);

                    var results = youTubeAutomation.ListResults();
                    ui.DisplayResults(results);
                    Logger.Info("Displayed search results.\n");

                    int videoSelection = ui.PromptForVideoSelection(results.Count);
                     Logger.Info($"User selected video number: {videoSelection}");
                    if (videoSelection == -1)
                    {
                        Logger.Info("User chose to skip video selection.");
                        continue; 
                    }

                    ui.DisplayMessage($"Playing: {results[videoSelection - 1]}");
                    youTubeAutomation.PlayVideo(videoSelection);
                    ui.DisplayMessage("Entering Full-Screen Mode...");
                    Logger.Info("Playing video in full-screen mode.");
                }
                catch (Exception ex)
                {
                    ui.DisplayMessage("An error occurred: " + ex.Message);
                    Logger.Error(ex, "An error occurred.");
                }
            }

            youTubeAutomation.Close();
            Logger.Info("Exiting YouTube Automation...");
        }
    }
}