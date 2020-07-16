using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Ghacu.Api.Entities;

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

    protected override void Print(string template, Step step)
    {
      string latestVersion = step.Uses.GetLatestVersion().Value;
      string latestVersion1, latestVersion2;
      ConsoleColor color;
      if (step.VersionDiffType == VersionDiffType.Major || step.VersionDiffType == VersionDiffType.None)
      {
        latestVersion1 = Regex.Replace(latestVersion, "(v*)(.*)", "$1");
        latestVersion2 = Regex.Replace(latestVersion, "(v*)(.*)", "$2");
        color = FOREGROUND_COLOR_MAJOR;
      }
      else if (step.VersionDiffType == VersionDiffType.Minor)
      {
        latestVersion1 = Regex.Replace(latestVersion, "([a-zA-Z0-9]+\\.)(.*)", "$1");
        latestVersion2 = Regex.Replace(latestVersion, "([a-zA-Z0-9]+\\.)(.*)", "$2");
        color = FOREGROUND_COLOR_MINOR;
      }
      else if (step.VersionDiffType == VersionDiffType.Patch)
      {
        latestVersion1 = Regex.Replace(latestVersion, "([a-zA-Z0-9]+\\.[a-zA-Z0-9]+\\.)(.*)", "$1");
        latestVersion2 = Regex.Replace(latestVersion, "([a-zA-Z0-9]+\\.[a-zA-Z0-9]+\\.)(.*)", "$2");
        color = FOREGROUND_COLOR_PATCH;
      }
      else if (step.VersionDiffType == VersionDiffType.Prerelease)
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
      
      Console.Write(template, step.Uses.ActionName, step.Uses.CurrentVersion.Value, ArrowChar, latestVersion1);
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