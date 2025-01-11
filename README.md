# YouTube Automation Console Application

This project is a console application that automates YouTube video searching and playback using Selenium WebDriver and NUnit for testing. It provides a user-friendly interface for searching videos, applying filters, and interacting with the video playback.

## Features

- **Search YouTube**: Enter a search query to find videos.
- **Filter Options**: Apply filters for videos and shorts.
- **Display Results**: View search results with details like title, duration, channel name, and views.
- **Video Playback**: Select a video to play, with options for full-screen mode and playback controls.
- **Error Handling**: Robust error handling for various scenarios, including invalid inputs and playback issues.
- **Logging**: Structured logging of actions and errors, with screenshot capabilities.

## Project Structure

```
YouTubeAutomation
├── src
│   ├── Config
│   │   ├── AppConfig.json
│   │   └── NLog.config
│   ├── Program.cs
│   ├── UserInterface
│   │   └── UserInterface.cs
│   └── YouTubeAutomation
│       ├── YouTubeAutomation.cs
│       └── YouTubeAutomationTests.cs
├── YouTubeAutomation.csproj
├── YouTubeAutomation.sln
├── README.md
└── .gitignore
```

## Setup Instructions

1. **Install Dependencies**: Ensure you have the necessary packages installed, including Selenium WebDriver and NUnit.
2. **WebDriver Configuration**: Update the `AppConfig.json` file with the path to your WebDriver executable and other settings.
3. **Build the Project**: Use your preferred C# development environment to build the project.
4. **Run the Application**: Execute the `Program.cs` file to start the application.

## Usage

- Launch the application and follow the prompts to enter your search query.
- Select filters as needed and choose a video from the displayed results.
- Use interactive commands to navigate and control playback.

## Testing

Unit tests for the `YouTubeAutomation` class are located in `YouTubeAutomationTests.cs`. Run these tests to validate the functionality of the application.

## Configuration

The application uses a configuration file located at `src/Config/AppConfig.json`. Ensure the following settings are correctly configured:

```json
{
  "WebDriverPath": "C:\\Users\\USER\\Downloads\\chromedriver-win64\\chromedriver-win64",
  "MaxWaitTime": 30,
  "ScreenshotDirectory": "C:\\Users\\USER\\Downloads\\ScreenShots"
}
```

## Logging

Logging is configured using NLog. The configuration file is located at `src/Config/NLog.config`. Logs are saved in the directory specified in the configuration file.

## Dependencies

- [Selenium WebDriver](https://www.selenium.dev/)
- [NUnit](https://nunit.org/)
- [NLog](https://nlog-project.org/)
- [Newtonsoft.Json](https://www.newtonsoft.com/json)
