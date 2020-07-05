using System.Collections.Generic;

namespace GHACU.Workflow.Entities
{
  public interface IJob
  {
    IEnumerable<IStep> Steps { get; }
  }
}