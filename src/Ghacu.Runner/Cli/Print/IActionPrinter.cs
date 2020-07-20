using System.Collections.Generic;
using Ghacu.Api.Entities;

namespace Ghacu.Runner.Cli.Print
{
  public interface IActionPrinter
  {
    void PrintHeader(string workflowName, string fileName);
    
    void Print(IEnumerable<Action> steps);

    void PrintNoUpgradeNeeded();

    void PrintRunUpgrade();
  }
}