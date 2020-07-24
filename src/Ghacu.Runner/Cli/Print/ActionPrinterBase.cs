using System;
using System.Collections.Generic;
using Action = Ghacu.Api.Entities.Action;

namespace Ghacu.Runner.Cli.Print
{
  public abstract class ActionPrinterBase : IActionPrinter
  {
    protected static char ArrowChar => Convert.ToChar(187);
    
    public abstract void PrintHeader(string workflowName, string fileName);

    public void Print(IEnumerable<Action> actions)
    {
      var maxWidthName = 0;
      var maxWidthCurrentVersion = 0;
      foreach (Action action in actions)
      {
        maxWidthName = Math.Max(maxWidthName, action.Repository.Length);
        maxWidthCurrentVersion = Math.Max(maxWidthCurrentVersion, action.CurrentVersion.Length);
      }

      foreach (Action action in actions)
      {
        Print("{0,-" + maxWidthName + "}  {1," + maxWidthCurrentVersion + "}  {2}  {3}", action);
      }
    }

    protected abstract void Print(string template, Action action);

    public abstract void PrintNoUpgradeNeeded();

    public abstract void PrintRunUpgrade();
  }
}