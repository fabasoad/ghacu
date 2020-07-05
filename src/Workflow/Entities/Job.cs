using System.Collections.Generic;

namespace GHACU.Workflow.Entities
{
  public sealed class Job : IJob
  {
    public IEnumerable<IStep> Steps { get; set; }
  }
}