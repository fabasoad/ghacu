using System.Collections.Generic;

namespace GHACU.Workflow.Entities
{
  public sealed class Job
  {
    public IEnumerable<Step> Steps { get; set; }
  }
}