using System;
using System.Collections.Generic;
using Ghacu.Api.Entities;

namespace Ghacu.Runner.Cli.Print
{
  public class ColorActionPrinter : IActionPrinter
  {
    private const ConsoleColor FOREGROUND_COLOR_INFO = ConsoleColor.DarkGreen;

    private readonly ConsoleColor _foregroundColorDefault = Console.ForegroundColor;

    public void PrintHeader(string workflowName, string fileName)
    {
      Console.ForegroundColor = FOREGROUND_COLOR_INFO;
      Console.WriteLine($"> {workflowName} ({fileName})");
      Console.ForegroundColor = _foregroundColorDefault;
    }

    public void Print(IEnumerable<Step> steps)
    {
      foreach (Step step in steps)
      {
        Console.WriteLine(step);
      }
    }

    public void PrintNoUpgradeNeeded()
    {
      Console.Write("All GitHub Actions match the latest versions ");
      Console.ForegroundColor = FOREGROUND_COLOR_INFO;
      Console.WriteLine(":)");
      Console.ForegroundColor = _foregroundColorDefault;
    }

    public void PrintRunUpgrade()
    {
      Console.Write("Run ");
      Console.ForegroundColor = FOREGROUND_COLOR_INFO;
      Console.Write("ghacu -u");
      Console.ForegroundColor = _foregroundColorDefault;
      Console.WriteLine(" to upgrade actions.");
    }
  }
}