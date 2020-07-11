using System;
using System.Collections.Generic;
using Ghacu.Api.Entities;

namespace Ghacu.Runner.Cli.Print
{
  public class NoColorActionPrinter : IActionPrinter
  {
    private readonly char _arrowChar = Convert.ToChar(187);

    public void PrintHeader(string workflowName, string fileName)
    {
      Console.WriteLine($"> {workflowName} ({fileName})");
    }

    public void Print(IEnumerable<Step> steps)
    {
      var maxWidthName = 0;
      var maxWidthCurrentVersion = 0;
      var maxWidthLatestVersion = 0;
      foreach (Step step in steps)
      {
        maxWidthName = Math.Max(maxWidthName, step.Uses.ActionName.Length);
        maxWidthCurrentVersion = Math.Max(maxWidthCurrentVersion, step.Uses.CurrentVersion.Value.Length);
        maxWidthLatestVersion = Math.Max(maxWidthLatestVersion, step.Uses.GetLatestVersion().Value.Length);
      }

      foreach (Step step in steps)
      {
        string template = "{0,-" + maxWidthName + "}  {1," + maxWidthCurrentVersion + "}  {2}  {3," +
                          maxWidthLatestVersion + "}";
        Console.WriteLine(template, step.Uses.ActionName, step.Uses.CurrentVersion.Value, _arrowChar,
          step.Uses.GetLatestVersion().Value);
      }
    }

    public void PrintNoUpgradeNeeded()
    {
      Console.WriteLine("All GitHub Actions match the latest versions :)");
    }

    public void PrintRunUpgrade()
    {
      Console.WriteLine("Run ghacu -u to upgrade actions.");
    }
  }
}