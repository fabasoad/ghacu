using System.Collections.Generic;

namespace IO.GitHub.FabaSoad.GHWorkflow.Entities
{
  public class Workflow
  {
    public string Name { get; set; }
    public Dictionary<string, Job> Jobs { get; set; }
  }
}