using System;
using Action = Ghacu.Api.Entities.Action;

namespace Ghacu.Runner.Cli.Print
{
  public class NoColorActionPrinter : ActionPrinterBase
  {
    public override void PrintHeader(string workflowName, string fileName) => Console.WriteLine($"> {workflowName} ({fileName})");

    protected override void Print(string template, Action action) => Console.WriteLine(
      template, action.Repository, action.CurrentVersion, ArrowChar, action.LatestVersion);

    public override void PrintNoUpgradeNeeded() => Console.WriteLine("All GitHub Actions match the latest versions :)");

    public override void PrintRunUpgrade() => Console.WriteLine("Run ghacu --upgrade to upgrade the actions.");
  }
}