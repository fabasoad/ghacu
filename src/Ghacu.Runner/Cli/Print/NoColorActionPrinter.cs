using System;
using Ghacu.Api.Entities;

namespace Ghacu.Runner.Cli.Print
{
  public class NoColorActionPrinter : ActionPrinterBase
  {
    public override void PrintHeader(string workflowName, string fileName) => Console.WriteLine($"> {workflowName} ({fileName})");

    protected override void Print(string template, Step step) => Console.WriteLine(
      template, step.Uses.ActionName, step.Uses.CurrentVersion.Value, ArrowChar, step.Uses.GetLatestVersion().Value);

    public override void PrintNoUpgradeNeeded() => Console.WriteLine("All GitHub Actions match the latest versions :)");

    public override void PrintRunUpgrade() => Console.WriteLine("Run ghacu --upgrade to upgrade actions.");
  }
}