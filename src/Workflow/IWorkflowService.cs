using System.Collections.Generic;

namespace GHACU.Workflow
{
  public interface IWorkflowService
  {
    IEnumerable<IWorkflowInfo> GetWorkflows(string repositoryPath);
  }
}