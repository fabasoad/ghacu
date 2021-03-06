using System;
using System.Text.RegularExpressions;
using Ghacu.Api.Entities;
using Ghacu.Api.Stream;
using Microsoft.Extensions.Logging;
using Action = Ghacu.Api.Entities.Action;

namespace Ghacu.Runner.Cli.Print
{
  public class ColorActionPrinter : ActionPrinterBase
  {
    private const ConsoleColor FOREGROUND_COLOR_INFO = ConsoleColor.DarkGreen;
    private const ConsoleColor FOREGROUND_COLOR_MAJOR = ConsoleColor.DarkRed;
    private const ConsoleColor FOREGROUND_COLOR_MINOR = ConsoleColor.DarkYellow;
    private const ConsoleColor FOREGROUND_COLOR_PATCH = ConsoleColor.DarkBlue;
    private const ConsoleColor FOREGROUND_COLOR_PRERELEASE = ConsoleColor.DarkMagenta;
    private const ConsoleColor FOREGROUND_COLOR_BUILD = ConsoleColor.DarkGray;
    
    private readonly IStreamer _streamer;

    public ColorActionPrinter(IStreamer streamer)
    {
      _streamer = streamer;
    }

    public override void PrintHeader(string workflowName, string fileName) =>
      _streamer.PushLine<ColorActionPrinter>(new StreamOptions
      {
        Level = LogLevel.Information,
        Messages = new StreamMessageBuilder()
          .Add($"> {workflowName} ({fileName})", FOREGROUND_COLOR_INFO).Build()
      });

    protected override void Print(string template, Action action)
    {
      string latestVersion = action.LatestVersion;
      string latestVersion1, latestVersion2;
      ConsoleColor color;
      if (action.VersionDiffType == VersionDiffType.Major || action.VersionDiffType == VersionDiffType.None)
      {
        latestVersion1 = Regex.Replace(latestVersion, "(v*)(.*)", "$1");
        latestVersion2 = Regex.Replace(latestVersion, "(v*)(.*)", "$2");
        color = FOREGROUND_COLOR_MAJOR;
      }
      else if (action.VersionDiffType == VersionDiffType.Minor)
      {
        latestVersion1 = Regex.Replace(latestVersion, "([a-zA-Z0-9]+\\.)(.*)", "$1");
        latestVersion2 = Regex.Replace(latestVersion, "([a-zA-Z0-9]+\\.)(.*)", "$2");
        color = FOREGROUND_COLOR_MINOR;
      }
      else if (action.VersionDiffType == VersionDiffType.Patch)
      {
        latestVersion1 = Regex.Replace(latestVersion, "([a-zA-Z0-9]+\\.[a-zA-Z0-9]+\\.)(.*)", "$1");
        latestVersion2 = Regex.Replace(latestVersion, "([a-zA-Z0-9]+\\.[a-zA-Z0-9]+\\.)(.*)", "$2");
        color = FOREGROUND_COLOR_PATCH;
      }
      else if (action.VersionDiffType == VersionDiffType.Prerelease)
      {
        latestVersion1 = Regex.Replace(latestVersion, "([a-zA-Z0-9]+\\.[a-zA-Z0-9]+\\.[a-zA-Z0-9]+)(.*)", "$1");
        latestVersion2 = Regex.Replace(latestVersion, "([a-zA-Z0-9]+\\.[a-zA-Z0-9]+\\.[a-zA-Z0-9]+)(.*)", "$2");
        color = FOREGROUND_COLOR_PRERELEASE;
      }
      else
      {
        latestVersion1 = Regex.Replace(latestVersion, "([a-zA-Z0-9]+\\.[a-zA-Z0-9]+\\.[a-zA-Z0-9]+-[a-zA-Z0-9]+)(.*)", "$1");
        latestVersion2 = Regex.Replace(latestVersion, "([a-zA-Z0-9]+\\.[a-zA-Z0-9]+\\.[a-zA-Z0-9]+-[a-zA-Z0-9]+)(.*)", "$2");
        color = FOREGROUND_COLOR_BUILD;
      }
      
      _streamer.PushLine<ColorActionPrinter>(new StreamOptions
      {
        Level = LogLevel.Information,
        Messages = new StreamMessageBuilder()
          .Add(string.Format(template, action.Repository, action.CurrentVersion, ArrowChar, latestVersion1))
          .Add(latestVersion2, color)
          .Build()
      });
    }

    public override void PrintNoUpgradeNeeded() =>
      _streamer.PushLine<ColorActionPrinter>(new StreamOptions
      {
        Level = LogLevel.Information,
        Messages = new StreamMessageBuilder()
          .Add("All GitHub Actions match the latest versions ")
          .Add(":)", FOREGROUND_COLOR_INFO)
          .Build()
      });

    public override void PrintRunUpgrade()
    {
      _streamer.PushLine<ColorActionPrinter>(new StreamOptions
      {
        Level = LogLevel.Information,
        Messages = new StreamMessageBuilder()
          .Add("Run ")
          .Add("ghacu --upgrade", FOREGROUND_COLOR_INFO)
          .Add(" to upgrade the actions.")
          .Build()
      });
    }
  }
}