using System;
using System.Text.RegularExpressions;
using Ghacu.Api.Entities;
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

    private readonly ConsoleColor _foregroundColorDefault = Console.ForegroundColor;

    public override void PrintHeader(string workflowName, string fileName)
    {
      Console.ForegroundColor = FOREGROUND_COLOR_INFO;
      Console.WriteLine($"> {workflowName} ({fileName})");
      Console.ForegroundColor = _foregroundColorDefault;
    }

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
      
      Console.Write(template, action.Repository, action.CurrentVersion, ArrowChar, latestVersion1);
      Console.ForegroundColor = color;
      Console.WriteLine(latestVersion2);
      Console.ForegroundColor = _foregroundColorDefault;
    }

    public override void PrintNoUpgradeNeeded()
    {
      Console.Write("All GitHub Actions match the latest versions ");
      Console.ForegroundColor = FOREGROUND_COLOR_INFO;
      Console.WriteLine(":)");
      Console.ForegroundColor = _foregroundColorDefault;
    }

    public override void PrintRunUpgrade()
    {
      Console.Write("Run ");
      Console.ForegroundColor = FOREGROUND_COLOR_INFO;
      Console.Write("ghacu --upgrade");
      Console.ForegroundColor = _foregroundColorDefault;
      Console.WriteLine(" to upgrade the actions.");
    }
  }
}