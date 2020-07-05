using System.Collections.Generic;

namespace GHACU.Workflow.Entities
{
  public interface IActionWorkflow
  {
    string Name { get; }
    Dictionary<string, IJob> Jobs { get; }
  }
}