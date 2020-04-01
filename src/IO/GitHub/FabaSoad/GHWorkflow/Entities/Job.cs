using System.Collections.Generic;

namespace IO.GitHub.FabaSoad.GHWorkflow.Entities
{
  public class Job
  {
    public IEnumerable<Step> Steps { get; set; }
  }
}