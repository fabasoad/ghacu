using System.Collections.Generic;
using Ghacu.Api.Entities;

namespace Ghacu.Workflow
{
  public interface IWorkflowParser
  {
    IEnumerable<WorkflowInfo> Parse(IEnumerable<string> files);
  }
}