using System.Collections.Generic;
using Ghacu.Api.Entities;

namespace Ghacu.Runner.Cli.Print
{
  public interface IActionPrinter
  {
    void PrintHeader(string workflowName, string fileName);
    
    void Print(IEnumerable<Step> steps);

    void PrintNoUpgradeNeeded();

    void PrintRunUpgrade();
  }
}