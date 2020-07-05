using System.Collections.Generic;

namespace GHACU.Workflow.Entities
{
  public sealed class ActionWorkflow : IActionWorkflow
  {
    public string Name { get; set; }
    public Dictionary<string, IJob> Jobs { get; set; }
  }
}