using System.Collections.Generic;

namespace GHACU.Workflow
{
  public interface IWorkflowParser
  {
    IEnumerable<IWorkflowInfo> Parse(IEnumerable<string> files);
  }
}