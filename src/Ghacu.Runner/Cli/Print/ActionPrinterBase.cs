using System;
using System.Collections.Generic;
using Ghacu.Api.Entities;

namespace Ghacu.Runner.Cli.Print
{
  public abstract class ActionPrinterBase : IActionPrinter
  {
    protected static char ArrowChar => Convert.ToChar(187);
    
    public abstract void PrintHeader(string workflowName, string fileName);

    public void Print(IEnumerable<Step> steps)
    {
      var maxWidthName = 0;
      var maxWidthCurrentVersion = 0;
      foreach (Step step in steps)
      {
        maxWidthName = Math.Max(maxWidthName, step.Uses.ActionName.Length);
        maxWidthCurrentVersion = Math.Max(maxWidthCurrentVersion, step.Uses.CurrentVersion.Value.Length);
      }

      foreach (Step step in steps)
      {
        Print("{0,-" + maxWidthName + "}  {1," + maxWidthCurrentVersion + "}  {2}  {3}", step);
      }
    }

    protected abstract void Print(string template, Step step);

    public abstract void PrintNoUpgradeNeeded();

    public abstract void PrintRunUpgrade();
  }
}