using System.Collections.Generic;
using Ghacu.Api.Entities;

namespace Ghacu.Workflow
{
  public interface IWorkflowService
  {
    IEnumerable<WorkflowInfo> GetWorkflows(string repositoryPath);
  }
}