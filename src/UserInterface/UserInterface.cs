using System;
using System.Collections.Generic;

namespace YouTubeAutomation
{
    public class UserInterface
    {
        public void DisplayWelcomeMessage()
        {
            Console.WriteLine("Welcome to YouTube Video Player!");
        }

        public string PromptForSearchQuery()
        {
           Console.Write("Enter your search query (or type 'exit' to quit): ");
            string query = Console.ReadLine();
            while (string.IsNullOrWhiteSpace(query))
            {
                Console.WriteLine("Search query cannot be empty. Please try again.");
                Console.Write("Enter your search query (or type 'exit' to quit): ");
                query = Console.ReadLine();
            }
            return query;
        }

        public void DisplayFilterOptions()
        {
            Console.WriteLine("Filter Options:");
            Console.WriteLine("1. Videos");
            Console.WriteLine("2. Shorts");
        }

        public int PromptForFilterSelection()
        {
            Console.Write("Select a filter: ");
            if (int.TryParse(Console.ReadLine(), out int filter) && (filter == 1 || filter == 2))
            {
                return filter;
            }
            Console.WriteLine("Invalid selection. Please try again.");
            return PromptForFilterSelection();
        }

        public void DisplayResults(List<string> results)
        {
            Console.WriteLine("Top 5 Results:");
            for (int i = 0; i < results.Count; i++)
            {
                Console.WriteLine($"[{results[i]}]");
            }
        }

        public int PromptForVideoSelection(int maxSelection)
        {
            Console.Write($"Select a video (1-{maxSelection}) or enter 'r' to search again: ");
            string input = Console.ReadLine();
            if (input.ToLower() == "r")
            {
                return -1; 
            }

            if (int.TryParse(input, out int selection) && selection >= 1 && selection <= maxSelection)
            {
                return selection;
            }

            Console.WriteLine("Invalid selection. Please try again.");
            return PromptForVideoSelection(maxSelection);
        }

        public void DisplayMessage(string message)
        {
            Console.WriteLine(message);
        }
    }
}