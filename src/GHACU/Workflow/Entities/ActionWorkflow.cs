using System.Collections.Generic;

namespace GHACU.Workflow.Entities
{
  public sealed class ActionWorkflow
  {
    public string Name { get; set; }
    public Dictionary<string, Job> Jobs { get; set; }
  }
}