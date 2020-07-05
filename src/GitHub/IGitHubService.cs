using System.Collections.Generic;
using Api.Entities;
using GHACU.Workflow;
using GHACU.Workflow.Entities;

namespace GHACU.GitHub
{
  public interface IGitHubService
  {
    IDictionary<(string, IWorkflowFile), IEnumerable<IStep>> GetOutdated(string token, IEnumerable<IWorkflowInfo> items);
  }
}